using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class PurchaseOrderService(
    IPurchaseOrderRepository orders, IPurchaseAgreementRepository agreements, ISupplierQuotationRepository quotations,
    IPurchaseSourcingRepository sourcing, ISupplierRepository suppliers, IWarehouseRepository warehouses,
    DocumentNumberService numbers, PurchaseOrderLifecycleService lifecycle, CurrencyExchangeRateService exchangeRates,
    IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    public async Task<List<PurchaseOrderDto>> GetAllAsync(PurchaseOrderStatus? status, string? search)
    {
        var supplierNames = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var warehouseNames = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return (await orders.GetAllAsync(status, search)).Select(x => Map(x,
            supplierNames.GetValueOrDefault(x.SupplierId, $"#{x.SupplierId}"),
            warehouseNames.GetValueOrDefault(x.WarehouseId, $"#{x.WarehouseId}"))).ToList();
    }

    public async Task<int> CreateFromAwardAsync(int sourcingEventId)
    {
        PurchaseOrder? order = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var award = await quotations.GetAwardAsync(sourcingEventId) ?? throw new InvalidOperationException("A quotation award is required before creating a purchase order.");
            var quote = await quotations.GetByIdAsync(award.SupplierQuotationId) ?? throw new InvalidOperationException("Awarded supplier quotation was not found.");
            var source = await sourcing.GetByIdAsync(sourcingEventId) ?? throw new InvalidOperationException("Sourcing event was not found.");
            if (await orders.GetByQuotationIdAsync(quote.Id) is not null) throw new InvalidOperationException("A purchase order already exists for this quotation.");
            if (await agreements.GetByQuotationIdAsync(quote.Id) is not null) throw new InvalidOperationException("A purchase agreement already exists for this quotation.");
            var today = DateOnly.FromDateTime(DateTime.Today);
            order = new PurchaseOrder(await numbers.GenerateAsync(DocumentNumberType.PurchaseOrder, DateTime.Today),
                source.Id, quote.Id, quote.SupplierId, source.WarehouseId, today, today.AddDays(quote.LeadTimeDays),
                quote.CurrencyCode, quote.PaymentTermsSnapshot, currentUser.UserId);
            foreach (var line in quote.Lines)
            {
                var sourcingLine = source.Lines.Single(x => x.Id == line.PurchaseSourcingLineId);
                order.AddLine(new PurchaseOrderLine(line.Id, sourcingLine.ProductId, sourcingLine.MeasurementUnitId,
                    line.QuotedQuantity, sourcingLine.UnitFactorToBase, line.QuotedQuantity * sourcingLine.UnitFactorToBase,
                    line.UnitPrice, line.DiscountPercent, line.TaxPercent, line.ProductCodeSnapshot,
                    line.ProductNameSnapshot, line.UnitNameSnapshot));
            }
            await orders.AddAsync(order);
        });
        return order!.Id;
    }

    public async Task ApproveAsync(int id, byte[] rowVersion)
    {
        var order = await GetTracked(id, rowVersion);
        await unitOfWork.ExecuteInTransactionAsync(() => { order.Approve(currentUser.UserId); return Task.CompletedTask; });
    }

    public async Task ConfirmAsync(int id, byte[] rowVersion)
    {
        var order = await GetTracked(id, rowVersion);
        var rate = await exchangeRates.ResolveAsync(order.CurrencyCode, order.OrderDate);
        await unitOfWork.ExecuteInTransactionAsync(() =>
        {
            order.SetExchangeRate(rate.CompanyCurrencyCode, rate.Rate, rate.RateDate, rate.Source);
            order.Confirm(currentUser.UserId);
            return Task.CompletedTask;
        });
    }

    public async Task CancelAsync(int id, string reason, byte[] rowVersion)
    {
        var order = await GetTracked(id, rowVersion);
        await unitOfWork.ExecuteInTransactionAsync(() => { order.Cancel(currentUser.UserId, reason); return Task.CompletedTask; });
    }

    public async Task<PurchaseOrderDto?> GetAsync(int id)
    {
        var order = await orders.GetByIdAsync(id); if (order is null) return null;
        var supplier = await suppliers.GetByIdAsync(order.SupplierId);
        var warehouse = await warehouses.GetByIdAsync(order.WarehouseId);
        return Map(order, supplier?.Name ?? $"#{order.SupplierId}", warehouse?.Name ?? $"#{order.WarehouseId}", await lifecycle.GetProgressAsync(order));
    }

    public Task CloseRemainingAsync(int id, string reason, byte[] rowVersion) => lifecycle.CloseRemainingAsync(id, reason, rowVersion);
    public Task ReopenAsync(int id, string reason, byte[] rowVersion) => lifecycle.ReopenAsync(id, reason, rowVersion);

    private static PurchaseOrderDto Map(PurchaseOrder order, string supplierName, string warehouseName, PurchaseOrderProgressDto? progress = null)
    {
        progress ??= new();
        var progressMap = progress.Lines.ToDictionary(x => x.PurchaseOrderLineId);
        return new PurchaseOrderDto
        {
            Id = order.Id, OrderNumber = order.OrderNumber, PurchaseAgreementId = order.PurchaseAgreementId,
            SupplierName = supplierName, WarehouseName = warehouseName, Status = order.Status,
            CurrencyCode = order.CurrencyCode, CompanyCurrencyCode = order.CompanyCurrencyCode,
            ExchangeRate = order.ExchangeRate, ExchangeRateDate = order.ExchangeRateDate, ExchangeRateSource = order.ExchangeRateSource,
            OrderDate = order.OrderDate, ExpectedDate = order.ExpectedDate, ClosureType = order.ClosureType,
            ClosureReason = order.ClosureReason, ClosedByUserId = order.ClosedByUserId, ClosedAtUtc = order.ClosedAtUtc,
            RowVersion = order.RowVersion, Progress = progress,
            Lines = order.Lines.Select(line =>
            {
                var lineProgress = progressMap.GetValueOrDefault(line.Id);
                return new PurchaseOrderLineDto
                {
                    Id = line.Id, ProductCode = line.ProductCodeSnapshot, ProductName = line.ProductNameSnapshot,
                    UnitName = line.UnitNameSnapshot, OrderedQuantity = line.OrderedQuantity, UnitPrice = line.UnitPrice,
                    GrossAmount = line.GrossAmount, ReceivedQuantity = lineProgress?.ReceivedQuantity ?? 0,
                    ReturnedQuantity = lineProgress?.ReturnedQuantity ?? 0, BilledQuantity = lineProgress?.BilledQuantity ?? 0,
                    RemainingToReceive = lineProgress?.RemainingToReceive ?? line.OrderedQuantity,
                    UnsettledReceivedQuantity = lineProgress?.UnsettledReceivedQuantity ?? 0
                };
            }).ToList(),
            History = order.History.OrderByDescending(x => x.CreatedAtUtc).Select(x => new PurchaseOrderLifecycleHistoryDto
            {
                FromStatus = x.FromStatus, ToStatus = x.ToStatus, Action = x.Action,
                UserId = x.UserId, Reason = x.Reason, CreatedAtUtc = x.CreatedAtUtc
            }).ToList()
        };
    }

    private async Task<PurchaseOrder> GetTracked(int id, byte[] rowVersion)
    {
        var order = await orders.GetByIdAsync(id) ?? throw new InvalidOperationException("Purchase order was not found.");
        if (!order.RowVersion.SequenceEqual(rowVersion)) throw new InvalidOperationException("Purchase order was modified by another user. Reload and try again.");
        return order;
    }
}
