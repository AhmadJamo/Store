using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class PurchaseReturnService(
    IPurchaseReturnRepository returns, IPurchaseRepository purchases, ISupplierRepository suppliers,
    IProductRepository products, IWarehouseRepository warehouses, IProductStockRepository stocks,
    IStockTransactionRepository transactions, ITaxRateRepository taxRates,
    IAccountingSettingsRepository accountingSettings, JournalPostingService journalPosting,
    DocumentNumberService documentNumbers, InventoryTrackingService inventoryTracking,
    ICurrentUserService currentUser, IUnitOfWork unitOfWork)
{
    public async Task<List<PurchaseReturnDto>> GetAllAsync()
    {
        var values = await returns.GetAllAsync();
        var purchaseMap = (await purchases.GetAllAsync()).ToDictionary(x => x.Id);
        var supplierMap = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return values.Select(x => Map(x, purchaseMap.GetValueOrDefault(x.PurchaseId)?.InvoiceNumber ?? "-", supplierMap.GetValueOrDefault(x.SupplierId, "-"))).ToList();
    }

    public async Task<PurchaseReturnDto?> GetByIdAsync(int id)
    {
        var value = await returns.GetByIdAsync(id);
        if (value is null) return null;
        var purchase = await purchases.GetByIdAsync(value.PurchaseId);
        var supplier = await suppliers.GetByIdAsync(value.SupplierId);
        var dto = Map(value, purchase?.InvoiceNumber ?? "-", supplier?.Name ?? "-");
        var productMap = (await products.GetAllAsync(null)).ToDictionary(x => x.Id, x => x.Name);
        var warehouseMap = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        foreach (var item in dto.Items)
        {
            item.ProductName = productMap.GetValueOrDefault(item.ProductId, $"Product #{item.ProductId}");
            var source = value.Items.Single(x => x.PurchaseItemId == item.PurchaseItemId);
            item.WarehouseName = warehouseMap.GetValueOrDefault(source.WarehouseId, "-");
        }
        return dto;
    }

    public async Task<Dictionary<int, decimal>> GetReturnedQuantitiesAsync(int purchaseId) =>
        (await returns.GetByPurchaseIdAsync(purchaseId)).SelectMany(x => x.Items)
            .GroupBy(x => x.PurchaseItemId).ToDictionary(x => x.Key, x => x.Sum(i => i.Quantity));

    public async Task<int> CreateAsync(CreatePurchaseReturnDto dto)
    {
        if (dto.Items.Count == 0 || dto.Items.All(x => x.Quantity <= 0)) throw new ArgumentException("Select at least one quantity to return.");
        if (string.IsNullOrWhiteSpace(dto.Reason)) throw new ArgumentException("A return reason is required.");
        var purchase = await purchases.GetByIdAsync(dto.PurchaseId) ?? throw new InvalidOperationException("Purchase not found.");
        if (!await journalPosting.IsPostedAsync("Purchase", purchase.InvoiceNumber)) throw new InvalidOperationException("Post the original purchase before creating a return.");
        if (dto.Date.Date < purchase.Date.Date) throw new ArgumentException("Return date cannot be earlier than the purchase date.");
        var supplier = await suppliers.GetByIdAsync(purchase.SupplierId) ?? throw new InvalidOperationException("Supplier not found.");
        if (!supplier.AccountId.HasValue) throw new InvalidOperationException("Assign a payable account to the supplier before creating a return.");
        var settings = await accountingSettings.GetAsync() ?? throw new InvalidOperationException("Configure accounting posting accounts before creating a return.");
        var taxes = (await taxRates.GetAllAsync()).ToDictionary(x => x.Id);
        PurchaseReturn? created = null;

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var returned = await GetReturnedQuantitiesAsync(purchase.Id);
            created = new PurchaseReturn(await documentNumbers.GenerateAsync(DocumentNumberType.PurchaseReturn, dto.Date), purchase.Id, purchase.SupplierId, dto.Date, dto.Reason);
            var lines = new List<JournalEntryLine>();
            foreach (var request in dto.Items.Where(x => x.Quantity > 0))
            {
                var original = purchase.Items.SingleOrDefault(x => x.Id == request.PurchaseItemId) ?? throw new ArgumentException("A selected purchase item was not found on the original purchase.");
                if (request.Quantity > original.Quantity - returned.GetValueOrDefault(original.Id)) throw new InvalidOperationException("Return quantity cannot exceed the remaining purchased quantity.");
                var warehouseId = original.WarehouseId ?? purchase.WarehouseId;
                var warehouse = await warehouses.GetByIdAsync(warehouseId) ?? throw new InvalidOperationException("Purchase item warehouse was not found.");
                if (!warehouse.InventoryAccountId.HasValue) throw new InvalidOperationException("Configure the purchase return warehouse inventory account.");
                var stock = await stocks.GetByProductAndWarehouseAsync(original.ProductId, warehouseId) ?? throw new InvalidOperationException("Returned purchase stock was not found.");
                if (request.Quantity > stock.Quantity) throw new InvalidOperationException("Purchase return quantity cannot exceed currently available stock.");

                var ratio = request.Quantity / original.Quantity;
                var gross = Round(original.Quantity * original.PurchasePrice * ratio);
                var discount = Round(original.DiscountAmount * ratio);
                TaxRate? taxRate = null;
                decimal tax = 0;
                if (original.TaxRateId.HasValue)
                {
                    if (!taxes.TryGetValue(original.TaxRateId.Value, out taxRate)) throw new InvalidOperationException("The original purchase tax rate no longer exists.");
                    var taxable = gross - discount;
                    tax = taxRate.IsPriceInclusive ? Round(taxable * taxRate.Rate / (100m + taxRate.Rate)) : Round(taxable * taxRate.Rate / 100m);
                }
                var originalInventory = taxRate?.IsPriceInclusive == true ? gross - tax : gross;
                var payable = taxRate?.IsPriceInclusive == true ? gross - discount : gross - discount + tax;
                var movement = stock.RemoveQuantity(request.Quantity);
                var removedCost = Math.Abs(movement.TransactionValue);
                var removedCostPosting = Round(removedCost);
                created.AddItem(new PurchaseReturnItem(original.Id, original.ProductId, warehouseId, request.Quantity, originalInventory, discount, tax, payable, removedCost));
                await transactions.AddAsync(new StockTransaction(original.ProductId, warehouseId, -request.Quantity, StockTransactionType.PurchaseReturn, created.ReturnNumber, movement));
                var product = await products.GetByIdAsync(original.ProductId)
                    ?? throw new InvalidOperationException("Returned purchase product was not found.");
                await inventoryTracking.ReturnPurchaseAsync(product, warehouseId,
                    request.Quantity, request.TrackingAllocations, purchase.InvoiceNumber,
                    created.ReturnNumber, currentUser.UserId);

                lines.Add(new JournalEntryLine(supplier.AccountId.Value, payable, 0, warehouse.BranchId, warehouse.Id, $"Supplier credit for {created.ReturnNumber}"));
                if (discount > 0)
                {
                    if (!settings.PurchaseDiscountAccountId.HasValue) throw new InvalidOperationException("Configure a purchase discount account before returning a discounted purchase.");
                    lines.Add(new JournalEntryLine(settings.PurchaseDiscountAccountId.Value, discount, 0, warehouse.BranchId, warehouse.Id, $"Purchase discount reversal for {created.ReturnNumber}"));
                }
                if (tax > 0) lines.Add(new JournalEntryLine(taxRate!.InputAccountId, 0, tax, warehouse.BranchId, warehouse.Id, $"Input tax reversal for {created.ReturnNumber}"));
                lines.Add(new JournalEntryLine(warehouse.InventoryAccountId.Value, 0, removedCostPosting, warehouse.BranchId, warehouse.Id, $"Inventory returned for {created.ReturnNumber}"));
                var variance = Round(originalInventory - removedCostPosting);
                if (variance != 0)
                {
                    if (!settings.CostOfSalesAccountId.HasValue) throw new InvalidOperationException("Configure a cost of sales account before posting purchase return cost variance.");
                    lines.Add(variance > 0
                        ? new JournalEntryLine(settings.CostOfSalesAccountId.Value, 0, variance, warehouse.BranchId, warehouse.Id, $"Purchase return cost variance for {created.ReturnNumber}")
                        : new JournalEntryLine(settings.CostOfSalesAccountId.Value, Math.Abs(variance), 0, warehouse.BranchId, warehouse.Id, $"Purchase return cost variance for {created.ReturnNumber}"));
                }
            }
            await returns.AddAsync(created);
            await journalPosting.PostAsync(new JournalPostingRequest(dto.Date, $"Purchase return {created.ReturnNumber} for {purchase.InvoiceNumber}", "PurchaseReturn", created.ReturnNumber, lines, "This purchase return has already been posted."));
        });
        return created!.Id;
    }

    private static PurchaseReturnDto Map(PurchaseReturn x, string invoice, string supplier) => new()
    {
        Id=x.Id, ReturnNumber=x.ReturnNumber, PurchaseId=x.PurchaseId, PurchaseInvoiceNumber=invoice, SupplierName=supplier, Date=x.Date, Reason=x.Reason,
        PayableAmount=x.PayableAmount, TaxAmount=x.TaxAmount, RemovedInventoryCost=x.RemovedInventoryCost,
        Items=x.Items.Select(i => new PurchaseReturnItemDto { PurchaseItemId=i.PurchaseItemId, ProductId=i.ProductId, Quantity=i.Quantity, PayableAmount=i.PayableAmount, RemovedInventoryCost=i.RemovedInventoryCost }).ToList()
    };
    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
