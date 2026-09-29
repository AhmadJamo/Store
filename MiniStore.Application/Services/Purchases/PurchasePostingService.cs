using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class PurchasePostingService(IPurchaseRepository purchaseRepository, ISupplierRepository supplierRepository, IWarehouseRepository warehouseRepository, ITaxRateRepository taxRateRepository, IAccountingSettingsRepository accountingSettingsRepository, IJournalEntryRepository journalEntryRepository, IStockTransactionRepository stockTransactionRepository, IUnitOfWork unitOfWork, DocumentNumberService documentNumbers)
{
    private const string PurchaseSourceType = "Purchase";
    public Task<bool> IsPostedAsync(string invoiceNumber) => journalEntryRepository.ExistsForSourceAsync(PurchaseSourceType, invoiceNumber);

    public async Task PostAsync(int purchaseId)
    {
        var purchase = await purchaseRepository.GetByIdAsync(purchaseId) ?? throw new InvalidOperationException("Purchase not found.");
        var supplier = await supplierRepository.GetByIdAsync(purchase.SupplierId) ?? throw new InvalidOperationException("Supplier not found.");
        var settings = await accountingSettingsRepository.GetAsync() ?? throw new InvalidOperationException("Configure accounting posting accounts before posting purchases.");
        if (!supplier.AccountId.HasValue) throw new InvalidOperationException("Assign a payable account to the supplier before posting this purchase.");

        var taxRates = (await taxRateRepository.GetAllAsync()).ToDictionary(x => x.Id);
        var lines = new List<JournalEntryLine>();
        decimal payableTotal = 0;
        foreach (var item in purchase.Items)
        {
            var warehouse = await warehouseRepository.GetByIdAsync(item.WarehouseId ?? purchase.WarehouseId) ?? throw new InvalidOperationException("Item warehouse not found.");
            if (!warehouse.InventoryAccountId.HasValue) throw new InvalidOperationException($"Assign an inventory account to warehouse '{warehouse.Name}' before posting this purchase.");
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
            lines.Add(new JournalEntryLine(warehouse.InventoryAccountId.Value, inventoryAmount, 0, warehouse.BranchId, warehouse.Id, $"Inventory from {purchase.InvoiceNumber}"));
            if (item.DiscountAmount > 0)
            {
                if (!settings.PurchaseDiscountAccountId.HasValue) throw new InvalidOperationException("Configure a purchase discount account before posting a discounted purchase.");
                lines.Add(new JournalEntryLine(settings.PurchaseDiscountAccountId.Value, 0, item.DiscountAmount, warehouse.BranchId, warehouse.Id, $"Purchase discount on {purchase.InvoiceNumber}"));
            }
            if (taxRate is not null && taxAmount > 0)
                lines.Add(new JournalEntryLine(taxRate.InputAccountId, taxAmount, 0, warehouse.BranchId, warehouse.Id, $"Input tax on {purchase.InvoiceNumber}"));
            payableTotal += taxRate?.IsPriceInclusive == true ? taxableAmount : taxableAmount + taxAmount;
        }

        var receiptTransactions = await stockTransactionRepository.GetByReferenceAndTypeAsync(
            purchase.InvoiceNumber,
            StockTransactionType.Purchase);
        foreach (var varianceGroup in receiptTransactions
            .Where(x => x.CostVariance != 0)
            .GroupBy(x => x.WarehouseId))
        {
            if (!settings.CostOfSalesAccountId.HasValue)
                throw new InvalidOperationException(
                    "Configure a cost of sales account before posting inventory cost variance.");

            var warehouse = await warehouseRepository.GetByIdAsync(varianceGroup.Key)
                ?? throw new InvalidOperationException("Variance warehouse was not found.");
            if (!warehouse.InventoryAccountId.HasValue)
                throw new InvalidOperationException(
                    $"Assign an inventory account to warehouse '{warehouse.Name}' before posting this purchase.");

            var variance = Round(varianceGroup.Sum(x => x.CostVariance));
            if (variance > 0)
            {
                lines.Add(new JournalEntryLine(
                    settings.CostOfSalesAccountId.Value, variance, 0,
                    warehouse.BranchId, warehouse.Id,
                    $"Negative-stock cost variance on {purchase.InvoiceNumber}"));
                lines.Add(new JournalEntryLine(
                    warehouse.InventoryAccountId.Value, 0, variance,
                    warehouse.BranchId, warehouse.Id,
                    $"Negative-stock settlement on {purchase.InvoiceNumber}"));
            }
            else if (variance < 0)
            {
                var reversal = Math.Abs(variance);
                lines.Add(new JournalEntryLine(
                    warehouse.InventoryAccountId.Value, reversal, 0,
                    warehouse.BranchId, warehouse.Id,
                    $"Negative-stock settlement on {purchase.InvoiceNumber}"));
                lines.Add(new JournalEntryLine(
                    settings.CostOfSalesAccountId.Value, 0, reversal,
                    warehouse.BranchId, warehouse.Id,
                    $"Negative-stock cost variance on {purchase.InvoiceNumber}"));
            }
        }
        lines.Add(new JournalEntryLine(supplier.AccountId.Value, 0, payableTotal, null, null, $"Supplier payable for {purchase.InvoiceNumber}"));
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (await journalEntryRepository.ExistsForSourceAsync(PurchaseSourceType, purchase.InvoiceNumber)) throw new InvalidOperationException("This purchase has already been posted.");
            var entry = new JournalEntry(
                await documentNumbers.GenerateAsync(DocumentNumberType.JournalEntry, purchase.Date),
                purchase.Date,
                $"Purchase invoice {purchase.InvoiceNumber}",
                PurchaseSourceType,
                purchase.InvoiceNumber);
            foreach (var line in lines) entry.AddLine(line);
            entry.Post();
            await journalEntryRepository.AddAsync(entry);
        });
    }

    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
