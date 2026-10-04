using MiniStore.Application.DTOs.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SalesReturnService(
    ISalesReturnRepository salesReturnRepository,
    ISaleRepository saleRepository,
    IProductRepository productRepository,
    IProductStockRepository productStockRepository,
    IStockTransactionRepository stockTransactionRepository,
    IWarehouseRepository warehouseRepository,
    IPaymentMethodRepository paymentMethodRepository,
    IAccountingSettingsRepository accountingSettingsRepository,
    IBranchRepository branchRepository,
    JournalPostingService journalPosting,
    DocumentNumberService documentNumbers,
    InventoryTrackingService inventoryTracking,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
{
    private const string SaleSourceType = "Sale";
    private const string ReturnSourceType = "SalesReturn";

    public async Task<List<SalesReturnDto>> GetAllAsync()
    {
        var returns = await salesReturnRepository.GetAllAsync();
        var sales = (await saleRepository.GetAllAsync()).ToDictionary(x => x.Id);
        return returns.Select(x => Map(x, sales.GetValueOrDefault(x.SaleId)?.InvoiceNumber ?? "-")).ToList();
    }

    public async Task<SalesReturnDto?> GetByIdAsync(int id)
    {
        var salesReturn = await salesReturnRepository.GetByIdAsync(id);
        if (salesReturn is null) return null;
        var sale = await saleRepository.GetByIdAsync(salesReturn.SaleId);
        var dto = Map(salesReturn, sale?.InvoiceNumber ?? "-");
        var names = (await productRepository.GetAllAsync(null)).ToDictionary(x => x.Id, x => x.Name);
        foreach (var item in dto.Items)
            item.ProductName = names.GetValueOrDefault(item.ProductId, $"Product #{item.ProductId}");
        return dto;
    }

    public async Task<Dictionary<int, decimal>> GetReturnedQuantitiesAsync(int saleId) =>
        (await salesReturnRepository.GetBySaleIdAsync(saleId))
            .SelectMany(x => x.Items)
            .GroupBy(x => x.SaleItemId)
            .ToDictionary(x => x.Key, x => x.Sum(i => i.Quantity));

    public async Task<int> CreateAsync(CreateSalesReturnDto dto)
    {
        if (dto.Items.Count == 0 || dto.Items.All(x => x.Quantity <= 0))
            throw new ArgumentException("Select at least one quantity to return.");
        if (string.IsNullOrWhiteSpace(dto.Reason))
            throw new ArgumentException("A return reason is required.");

        var sale = await saleRepository.GetByIdAsync(dto.SaleId)
            ?? throw new InvalidOperationException("Sale not found.");
        if (!await journalPosting.IsPostedAsync(SaleSourceType, sale.InvoiceNumber))
            throw new InvalidOperationException("Post the original sale before creating a return.");
        if (dto.Date.Date < sale.Date.Date)
            throw new ArgumentException("Return date cannot be earlier than the sale date.");

        var warehouse = await warehouseRepository.GetByIdAsync(sale.WarehouseId)
            ?? throw new InvalidOperationException("Sale warehouse was not found.");
        var paymentMethod = sale.PaymentMethodId.HasValue
            ? await paymentMethodRepository.GetByIdAsync(sale.PaymentMethodId.Value)
            : null;
        if (paymentMethod is null || !warehouse.InventoryAccountId.HasValue || !warehouse.BranchId.HasValue)
            throw new InvalidOperationException("The original sale accounting mappings are incomplete.");
        var settings = await accountingSettingsRepository.GetAsync()
            ?? throw new InvalidOperationException("Configure accounting posting accounts before creating a return.");
        var branch = await branchRepository.GetByIdAsync(warehouse.BranchId.Value)
            ?? throw new InvalidOperationException("Sale warehouse branch was not found.");
        var revenueAccountId = branch.SalesRevenueAccountId ?? settings.SalesRevenueAccountId
            ?? throw new InvalidOperationException("Configure a sales revenue account before creating a return.");

        SalesReturn? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var alreadyReturned = await GetReturnedQuantitiesAsync(sale.Id);
            created = new SalesReturn(
                await documentNumbers.GenerateAsync(DocumentNumberType.SalesReturn, dto.Date),
                sale.Id,
                sale.WarehouseId,
                paymentMethod.Id,
                dto.Date,
                dto.Reason);

            foreach (var request in dto.Items.Where(x => x.Quantity > 0))
            {
                var saleItem = sale.Items.SingleOrDefault(x => x.Id == request.SaleItemId)
                    ?? throw new ArgumentException("A selected sale item was not found on the original sale.");
                var remaining = saleItem.Quantity - alreadyReturned.GetValueOrDefault(saleItem.Id);
                if (request.Quantity > remaining)
                    throw new InvalidOperationException("Return quantity cannot exceed the remaining sold quantity.");

                var product = await productRepository.GetByIdAsync(saleItem.ProductId)
                    ?? throw new InvalidOperationException("Returned product was not found.");
                var ratio = request.Quantity / saleItem.Quantity;
                var lineSubtotal = Round(saleItem.Total * ratio);
                var allocatedInvoiceDiscount = sale.Subtotal == 0
                    ? 0
                    : Round(sale.InvoiceDiscountAmount * lineSubtotal / sale.Subtotal);
                var discountedAmount = Round(lineSubtotal - allocatedInvoiceDiscount);
                var taxOnLineSubtotal = sale.TaxRateId.HasValue && sale.IsTaxInclusive
                    ? Round(lineSubtotal * sale.TaxRatePercent / (100m + sale.TaxRatePercent))
                    : 0;
                var tax = !sale.TaxRateId.HasValue || sale.TaxRatePercent <= 0
                    ? 0
                    : sale.IsTaxInclusive
                        ? Round(discountedAmount * sale.TaxRatePercent / (100m + sale.TaxRatePercent))
                        : Round(discountedAmount * sale.TaxRatePercent / 100m);
                var revenue = Round(lineSubtotal - taxOnLineSubtotal);
                var refund = sale.IsTaxInclusive ? discountedAmount : Round(discountedAmount + tax);
                var discount = Round(revenue + tax - refund);
                var restocked = product.InventoryBehavior != ProductInventoryBehavior.PreparedToOrder;
                var restockedCost = restocked
                    ? Math.Round(saleItem.UnitCost * request.Quantity, 8, MidpointRounding.AwayFromZero)
                    : 0;

                created.AddItem(new SalesReturnItem(
                    saleItem.Id, saleItem.ProductId, request.Quantity,
                    revenue, discount, tax, refund, restocked, restockedCost));

                if (restocked)
                {
                    var stock = await productStockRepository.GetByProductAndWarehouseAsync(
                        saleItem.ProductId, sale.WarehouseId);
                    if (stock is null)
                    {
                        stock = new ProductStock(saleItem.ProductId, sale.WarehouseId);
                        await productStockRepository.AddAsync(stock);
                    }
                    var movement = stock.Receive(request.Quantity, saleItem.UnitCost);
                    await stockTransactionRepository.AddAsync(new StockTransaction(
                        saleItem.ProductId, sale.WarehouseId, request.Quantity,
                        StockTransactionType.SalesReturn, created.ReturnNumber, movement));
                    await inventoryTracking.ReturnSaleAsync(product, sale.WarehouseId,
                        request.Quantity, request.TrackingAllocations, sale.InvoiceNumber,
                        created.ReturnNumber, currentUser.UserId);
                }
            }

            if (created.Items.Count == 0)
                throw new ArgumentException("Select at least one quantity to return.");
            if (created.DiscountAmount > 0 && !settings.SalesDiscountAccountId.HasValue)
                throw new InvalidOperationException("Configure a sales discount account before creating a discounted return.");
            if (created.TaxAmount > 0 && !sale.TaxOutputAccountId.HasValue)
                throw new InvalidOperationException("The original sale has no output tax account snapshot.");
            if (created.RestockedCostAmount > 0 && !settings.CostOfSalesAccountId.HasValue)
                throw new InvalidOperationException("Configure a cost of sales account before restocking a return.");

            var lines = new List<JournalEntryLine>
            {
                new(revenueAccountId, created.RevenueAmount, 0,
                    warehouse.BranchId, warehouse.Id, $"Revenue reversal for {created.ReturnNumber}")
            };
            if (created.TaxAmount > 0)
                lines.Add(new JournalEntryLine(sale.TaxOutputAccountId!.Value, created.TaxAmount, 0,
                    warehouse.BranchId, warehouse.Id, $"Output tax reversal for {created.ReturnNumber}"));
            lines.Add(new JournalEntryLine(paymentMethod.AccountId, 0, created.RefundAmount,
                warehouse.BranchId, warehouse.Id, $"Refund for {created.ReturnNumber}"));
            if (created.DiscountAmount > 0)
                lines.Add(new JournalEntryLine(settings.SalesDiscountAccountId!.Value, 0, created.DiscountAmount,
                    warehouse.BranchId, warehouse.Id, $"Sales discount reversal for {created.ReturnNumber}"));
            if (created.RestockedCostAmount > 0)
            {
                lines.Add(new JournalEntryLine(warehouse.InventoryAccountId.Value, created.RestockedCostAmount, 0,
                    warehouse.BranchId, warehouse.Id, $"Returned inventory for {created.ReturnNumber}"));
                lines.Add(new JournalEntryLine(settings.CostOfSalesAccountId!.Value, 0, created.RestockedCostAmount,
                    warehouse.BranchId, warehouse.Id, $"COGS reversal for {created.ReturnNumber}"));
            }
            await salesReturnRepository.AddAsync(created);
            await journalPosting.PostAsync(new JournalPostingRequest(
                dto.Date,
                $"Sales return {created.ReturnNumber} for {sale.InvoiceNumber}",
                ReturnSourceType,
                created.ReturnNumber,
                lines,
                "This sales return has already been posted."));
        });

        return created!.Id;
    }

    private static SalesReturnDto Map(SalesReturn value, string invoiceNumber) => new()
    {
        Id = value.Id,
        ReturnNumber = value.ReturnNumber,
        SaleId = value.SaleId,
        SaleInvoiceNumber = invoiceNumber,
        Date = value.Date,
        Reason = value.Reason,
        RefundAmount = value.RefundAmount,
        TaxAmount = value.TaxAmount,
        RestockedCostAmount = value.RestockedCostAmount,
        Items = value.Items.Select(x => new SalesReturnItemDto
        {
            SaleItemId = x.SaleItemId,
            ProductId = x.ProductId,
            Quantity = x.Quantity,
            RefundAmount = x.RefundAmount,
            Restocked = x.Restocked,
            RestockedCostAmount = x.RestockedCostAmount
        }).ToList()
    };

    private static decimal Round(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
