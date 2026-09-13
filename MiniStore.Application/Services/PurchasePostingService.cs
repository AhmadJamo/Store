using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class PurchasePostingService(IPurchaseRepository purchaseRepository, ISupplierRepository supplierRepository, IWarehouseRepository warehouseRepository, ITaxRateRepository taxRateRepository, IAccountingSettingsRepository accountingSettingsRepository, IJournalEntryRepository journalEntryRepository, IUnitOfWork unitOfWork)
{
    private const string PurchaseSourceType = "Purchase";
    public Task<bool> IsPostedAsync(string invoiceNumber) => journalEntryRepository.ExistsForSourceAsync(PurchaseSourceType, invoiceNumber);

    public async Task PostAsync(int purchaseId)
    {
        var purchase = await purchaseRepository.GetByIdAsync(purchaseId) ?? throw new InvalidOperationException("Purchase not found.");
        var supplier = await supplierRepository.GetByIdAsync(purchase.SupplierId) ?? throw new InvalidOperationException("Supplier not found.");
        var warehouse = await warehouseRepository.GetByIdAsync(purchase.WarehouseId) ?? throw new InvalidOperationException("Warehouse not found.");
        var settings = await accountingSettingsRepository.GetAsync() ?? throw new InvalidOperationException("Configure accounting posting accounts before posting purchases.");
        if (!supplier.AccountId.HasValue) throw new InvalidOperationException("Assign a payable account to the supplier before posting this purchase.");
        if (!warehouse.InventoryAccountId.HasValue) throw new InvalidOperationException("Assign an inventory account to the warehouse before posting this purchase.");

        var taxRates = (await taxRateRepository.GetAllAsync()).ToDictionary(x => x.Id);
        var entry = new JournalEntry($"JE-PUR-{purchase.InvoiceNumber}", purchase.Date, $"Purchase invoice {purchase.InvoiceNumber}", PurchaseSourceType, purchase.InvoiceNumber);
        decimal payableTotal = 0;
        foreach (var item in purchase.Items)
        {
            var gross = item.Quantity * item.PurchasePrice;
            var taxableAmount = gross - item.DiscountAmount;
            var taxAmount = 0m;
            TaxRate? taxRate = null;
            if (item.TaxRateId.HasValue)
            {
                if (!taxRates.TryGetValue(item.TaxRateId.Value, out taxRate)) throw new InvalidOperationException("A tax rate selected on this purchase no longer exists.");
                taxAmount = taxRate.IsPriceInclusive ? Round(taxableAmount * taxRate.Rate / (100m + taxRate.Rate)) : Round(taxableAmount * taxRate.Rate / 100m);
            }
            var inventoryAmount = taxRate?.IsPriceInclusive == true ? gross - taxAmount : gross;
            entry.AddLine(new JournalEntryLine(warehouse.InventoryAccountId.Value, inventoryAmount, 0, warehouse.BranchId, warehouse.Id, $"Inventory from {purchase.InvoiceNumber}"));
            if (item.DiscountAmount > 0)
            {
                if (!settings.PurchaseDiscountAccountId.HasValue) throw new InvalidOperationException("Configure a purchase discount account before posting a discounted purchase.");
                entry.AddLine(new JournalEntryLine(settings.PurchaseDiscountAccountId.Value, 0, item.DiscountAmount, warehouse.BranchId, warehouse.Id, $"Purchase discount on {purchase.InvoiceNumber}"));
            }
            if (taxRate is not null && taxAmount > 0)
                entry.AddLine(new JournalEntryLine(taxRate.InputAccountId, taxAmount, 0, warehouse.BranchId, warehouse.Id, $"Input tax on {purchase.InvoiceNumber}"));
            payableTotal += taxRate?.IsPriceInclusive == true ? taxableAmount : taxableAmount + taxAmount;
        }
        entry.AddLine(new JournalEntryLine(supplier.AccountId.Value, 0, payableTotal, warehouse.BranchId, warehouse.Id, $"Supplier payable for {purchase.InvoiceNumber}"));
        entry.Post();
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (await journalEntryRepository.ExistsForSourceAsync(PurchaseSourceType, purchase.InvoiceNumber)) throw new InvalidOperationException("This purchase has already been posted.");
            await journalEntryRepository.AddAsync(entry);
        });
    }

    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
