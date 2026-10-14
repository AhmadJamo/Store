using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class PurchaseOrderService(
    IPurchaseOrderRepository orders, IPurchaseAgreementRepository agreements, ISupplierQuotationRepository quotations, IPurchaseRequestRepository requests,
    IPurchaseSourcingRepository sourcing, ISupplierRepository suppliers, IWarehouseRepository warehouses, IProductRepository products,
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
        var ids = await CreateOrdersFromAwardAsync(sourcingEventId);
        return ids[0];
    }

    public async Task<List<int>> CreateOrdersFromAwardAsync(int sourcingEventId)
    {
        var created = new List<PurchaseOrder>();
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var award = await quotations.GetAwardAsync(sourcingEventId) ?? throw new InvalidOperationException("A quotation award is required before creating a purchase order.");
            var source = await sourcing.GetByIdAsync(sourcingEventId) ?? throw new InvalidOperationException("Sourcing event was not found.");
            var request = source.PurchaseRequestId.HasValue ? await requests.GetByIdAsync(source.PurchaseRequestId.Value) : null;
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (award.Allocations.Count == 0)
            {
                var quote = await quotations.GetByIdAsync(award.SupplierQuotationId)
                    ?? throw new InvalidOperationException("Awarded supplier quotation was not found.");
                await EnsurePurchaseDocumentNotCreatedAsync(quote.Id);
                var legacyOrder = new PurchaseOrder(
                    await numbers.GenerateAsync(DocumentNumberType.PurchaseOrder, DateTime.Today), source.Id,
                    quote.Id, quote.SupplierId, source.WarehouseId, today, today.AddDays(quote.LeadTimeDays),
                    quote.CurrencyCode, quote.PaymentTermsSnapshot, currentUser.UserId);
                foreach (var line in quote.Lines)
                {
                    var sourcingLine = source.Lines.Single(x => x.Id == line.PurchaseSourcingLineId);
                    var product = await products.GetByIdAsync(sourcingLine.ProductId)
                        ?? throw new InvalidOperationException("Purchase order product was not found.");
                    var orderLine = new PurchaseOrderLine(line.Id, sourcingLine.ProductId,
                        sourcingLine.MeasurementUnitId, line.QuotedQuantity, sourcingLine.UnitFactorToBase,
                        line.QuotedQuantity * sourcingLine.UnitFactorToBase, line.UnitPrice, line.DiscountPercent,
                        line.TaxPercent, line.ProductCodeSnapshot, line.ProductNameSnapshot, line.UnitNameSnapshot,
                        billingPolicy: product.PurchaseBillingPolicy, purchaseExpenseAccountId: product.PurchaseExpenseAccountId);
                    AddRequestAllocation(orderLine, sourcingLine, request);
                    legacyOrder.AddLine(orderLine);
                }
                await orders.AddAsync(legacyOrder);
                created.Add(legacyOrder);
                return;
            }

            foreach (var group in award.Allocations.GroupBy(x => x.SupplierQuotationId))
            {
                var first = group.First();
                await EnsurePurchaseDocumentNotCreatedAsync(group.Key);
                var order = new PurchaseOrder(
                    await numbers.GenerateAsync(DocumentNumberType.PurchaseOrder, DateTime.Today), source.Id,
                    group.Key, first.SupplierId, source.WarehouseId, today,
                    today.AddDays(group.Max(x => x.LeadTimeDaysSnapshot)), first.CurrencyCodeSnapshot,
                    first.PaymentTermsSnapshot, currentUser.UserId);
                foreach (var allocation in group)
                {
                    var sourcingLine = source.Lines.SingleOrDefault(x => x.Id == allocation.PurchaseSourcingLineId)
                        ?? throw new InvalidOperationException("Awarded sourcing line was not found.");
                    var product = await products.GetByIdAsync(sourcingLine.ProductId)
                        ?? throw new InvalidOperationException("Purchase order product was not found.");
                    var orderLine = new PurchaseOrderLine(allocation.SupplierQuotationLineId,
                        sourcingLine.ProductId, sourcingLine.MeasurementUnitId, allocation.AwardedQuantity,
                        sourcingLine.UnitFactorToBase, allocation.AwardedQuantity * sourcingLine.UnitFactorToBase,
                        allocation.UnitPriceSnapshot, allocation.DiscountPercentSnapshot,
                        allocation.TaxPercentSnapshot, allocation.ProductCodeSnapshot,
                        allocation.ProductNameSnapshot, allocation.UnitNameSnapshot,
                        billingPolicy: product.PurchaseBillingPolicy, purchaseExpenseAccountId: product.PurchaseExpenseAccountId);
                    AddRequestAllocation(orderLine, sourcingLine, request);
                    order.AddLine(orderLine);
                }
                await orders.AddAsync(order);
                created.Add(order);
            }
        });
        return created.Select(x => x.Id).ToList();

        async Task EnsurePurchaseDocumentNotCreatedAsync(int quotationId)
        {
            if (await orders.GetByQuotationIdAsync(quotationId) is not null)
                throw new InvalidOperationException("A purchase order already exists for one or more awarded quotations.");
            if (await agreements.GetByQuotationIdAsync(quotationId) is not null)
                throw new InvalidOperationException("A purchase agreement already exists for one or more awarded quotations.");
        }
        static void AddRequestAllocation(PurchaseOrderLine orderLine, PurchaseSourcingLine sourcingLine, PurchaseRequest? request)
        {
            if (request is null || !sourcingLine.PurchaseRequestLineId.HasValue) return;
            orderLine.AddDemandAllocation(new PurchaseDemandAllocation(PurchaseDemandSourceType.PurchaseRequest,
                request.Id, sourcingLine.PurchaseRequestLineId, request.RequestNumber, orderLine.StockQuantity));
        }
    }

    public async Task ConfigureDropshipAsync(ConfigurePurchaseDropshipDto dto)
    {
        var order = await GetTracked(dto.PurchaseOrderId, dto.RowVersion);
        var accounts = new Dictionary<int,int>();
        foreach (var productId in order.Lines.Select(x=>x.ProductId).Distinct())
        {
            var product=await products.GetByIdAsync(productId)??throw new InvalidOperationException("Purchase order product was not found.");
            if (!product.PurchaseExpenseAccountId.HasValue) throw new InvalidOperationException("Configure a purchase expense account for every direct-delivery product before continuing.");
            accounts[productId]=product.PurchaseExpenseAccountId.Value;
        }
        await unitOfWork.ExecuteInTransactionAsync(()=>{order.ConfigureDropship(dto.RecipientName,dto.Address,dto.CustomerReference,accounts);return Task.CompletedTask;});
    }

    public async Task RecordDirectDeliveryAsync(RecordPurchaseDirectDeliveryDto dto)
    {
        var order=await GetTracked(dto.PurchaseOrderId,dto.RowVersion);
        if(order.Status!=PurchaseOrderStatus.Confirmed||order.FulfillmentRoute!=PurchaseFulfillmentRoute.Dropship)throw new InvalidOperationException("Only confirmed direct-delivery purchase orders can record customer delivery evidence.");
        var line=order.Lines.SingleOrDefault(x=>x.Id==dto.PurchaseOrderLineId)??throw new ArgumentException("Purchase order line was not found.");
        await unitOfWork.ExecuteInTransactionAsync(async()=>{line.RecordDirectDelivery(dto.Quantity,dto.DeliveryDate,dto.ProofReference,dto.Carrier,dto.TrackingNumber,currentUser.UserId);await unitOfWork.FlushAsync();await lifecycle.TryCloseAutomaticallyAsync(order.Id);});
    }
    public async Task RecordSupplierConfirmationAsync(RecordPurchaseOrderSupplierConfirmationDto dto){var order=await GetTracked(dto.PurchaseOrderId,dto.RowVersion);await unitOfWork.ExecuteInTransactionAsync(()=>{order.RecordSupplierConfirmation(dto.ConfirmedExpectedDate,dto.ConfirmationReference,dto.Note,currentUser.UserId);return Task.CompletedTask;});}

    public async Task ApproveAsync(int id, byte[] rowVersion)
    {
        var order = await GetTracked(id, rowVersion);
        await unitOfWork.ExecuteInTransactionAsync(() => { order.Approve(currentUser.UserId); return Task.CompletedTask; });
    }

    public async Task<PurchaseFulfillmentRoute> ConfirmAsync(int id, byte[] rowVersion)
    {
        var order = await GetTracked(id, rowVersion);
        var rate = await exchangeRates.ResolveAsync(order.CurrencyCode, order.OrderDate);
        await unitOfWork.ExecuteInTransactionAsync(() =>
        {
            order.SetExchangeRate(rate.CompanyCurrencyCode, rate.Rate, rate.RateDate, rate.Source);
            order.Confirm(currentUser.UserId);
            return Task.CompletedTask;
        });
        return order.FulfillmentRoute;
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
            Id = order.Id, SupplierId=order.SupplierId, OrderNumber = order.OrderNumber, PurchaseAgreementId = order.PurchaseAgreementId,
            FulfillmentRoute=order.FulfillmentRoute,DropshipRecipientName=order.DropshipRecipientName,DropshipAddress=order.DropshipAddress,DropshipCustomerReference=order.DropshipCustomerReference,
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
                    UnitName = line.UnitNameSnapshot, BillingPolicy = line.BillingPolicySnapshot,
                    OrderedQuantity = line.OrderedQuantity, UnitPrice = line.UnitPrice,
                    GrossAmount = line.GrossAmount, ReceivedQuantity = lineProgress?.ReceivedQuantity ?? 0,
                    DirectDeliveredQuantity=lineProgress?.DirectDeliveredQuantity??line.DirectDeliveredQuantity,
                    ReturnedQuantity = lineProgress?.ReturnedQuantity ?? 0, BilledQuantity = lineProgress?.BilledQuantity ?? 0,
                    RemainingToReceive = lineProgress?.RemainingToReceive ?? line.OrderedQuantity,
                    RemainingToDeliver=lineProgress?.RemainingToDeliver??0,
                    ServiceCompletedQuantity = lineProgress?.ServiceCompletedQuantity ?? 0,
                    RemainingToComplete = lineProgress?.RemainingToComplete ?? 0,
                    AvailableToBill = lineProgress?.AvailableToBill ?? 0,
                    UnsettledReceivedQuantity = lineProgress?.UnsettledReceivedQuantity ?? 0,
                    DemandAllocations=line.DemandAllocations.Select(x=>new PurchaseDemandAllocationDto{SourceType=x.SourceType,SourceReference=x.SourceReference,AllocatedStockQuantity=x.AllocatedStockQuantity}).ToList(),
                    DirectDeliveryEvents=line.DirectDeliveryEvents.OrderByDescending(x=>x.RecordedAtUtc).Select(x=>new PurchaseDirectDeliveryEventDto{Quantity=x.Quantity,DeliveryDate=x.DeliveryDate,ProofReference=x.ProofReference,Carrier=x.Carrier,TrackingNumber=x.TrackingNumber}).ToList()
                };
            }).ToList(),
            SupplierConfirmations=order.SupplierConfirmations.OrderByDescending(x=>x.RecordedAtUtc).Select(x=>new PurchaseOrderSupplierConfirmationDto{PreviousExpectedDate=x.PreviousExpectedDate,ConfirmedExpectedDate=x.ConfirmedExpectedDate,ConfirmationReference=x.ConfirmationReference,Note=x.Note,RecordedAtUtc=x.RecordedAtUtc}).ToList(),
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
