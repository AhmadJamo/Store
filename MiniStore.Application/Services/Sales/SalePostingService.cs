using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SalePostingService(
    ISaleRepository saleRepository,
    IWarehouseRepository warehouseRepository,
    IBranchRepository branchRepository,
    IPaymentMethodRepository paymentMethodRepository,
    IAccountingSettingsRepository accountingSettingsRepository,
    IJournalEntryRepository journalEntryRepository,
    IUnitOfWork unitOfWork,
    DocumentNumberService documentNumbers)
{
    private const string SaleSourceType = "Sale";

    public Task<bool> IsPostedAsync(string invoiceNumber) =>
        journalEntryRepository.ExistsForSourceAsync(SaleSourceType, invoiceNumber);

    public async Task PostAsync(int saleId)
    {
        var sale = await saleRepository.GetByIdAsync(saleId)
            ?? throw new InvalidOperationException("Sale not found.");
        var warehouse = await warehouseRepository.GetByIdAsync(sale.WarehouseId)
            ?? throw new InvalidOperationException("Sale warehouse was not found.");
        var paymentMethod = sale.PaymentMethodId.HasValue
            ? await paymentMethodRepository.GetByIdAsync(sale.PaymentMethodId.Value)
            : null;
        if (paymentMethod is null)
            throw new InvalidOperationException("Assign a valid payment method before posting this sale.");
        if (!warehouse.InventoryAccountId.HasValue || !warehouse.BranchId.HasValue)
            throw new InvalidOperationException("Assign the sale warehouse to a branch and inventory account before posting.");

        var settings = await accountingSettingsRepository.GetAsync()
            ?? throw new InvalidOperationException("Configure accounting posting accounts before posting sales.");
        var branch = await branchRepository.GetByIdAsync(warehouse.BranchId.Value)
            ?? throw new InvalidOperationException("Sale warehouse branch was not found.");
        var revenueAccountId = branch.SalesRevenueAccountId ?? settings.SalesRevenueAccountId
            ?? throw new InvalidOperationException("Configure a sales revenue account before posting sales.");

        var taxOnSubtotal = sale.TaxRateId.HasValue && sale.IsTaxInclusive
            ? Round(sale.Subtotal * sale.TaxRatePercent / (100m + sale.TaxRatePercent))
            : 0;
        var revenue = Round(sale.Subtotal - taxOnSubtotal);
        var settlement = Round(sale.TotalAmount);
        var invoiceDiscount = sale.IsTaxInclusive
            ? Round(sale.InvoiceDiscountAmount - (taxOnSubtotal - sale.TaxAmount))
            : Round(sale.InvoiceDiscountAmount);
        var outputTax = Round(sale.TaxAmount);
        var cogs = Round(sale.Items.Sum(x => x.CostOfGoodsSold));
        if (revenue <= 0 || settlement < 0)
            throw new InvalidOperationException("A sale must have a positive posting amount.");
        if (invoiceDiscount > 0 && !settings.SalesDiscountAccountId.HasValue)
            throw new InvalidOperationException("Configure a sales discount account before posting a discounted sale.");
        if (outputTax > 0 && !sale.TaxOutputAccountId.HasValue)
            throw new InvalidOperationException("The sale does not have a valid output tax account snapshot.");
        if (cogs > 0 && !settings.CostOfSalesAccountId.HasValue)
            throw new InvalidOperationException("Configure a cost of sales account before posting inventory cost.");

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (await journalEntryRepository.ExistsForSourceAsync(SaleSourceType, sale.InvoiceNumber))
                throw new InvalidOperationException("This sale has already been posted.");

            var entry = new JournalEntry(
                await documentNumbers.GenerateAsync(DocumentNumberType.JournalEntry, sale.Date),
                sale.Date,
                $"Sale invoice {sale.InvoiceNumber}",
                SaleSourceType,
                sale.InvoiceNumber);

            if (settlement > 0)
                entry.AddLine(new JournalEntryLine(
                    paymentMethod.AccountId, settlement, 0,
                    warehouse.BranchId, warehouse.Id,
                    $"Settlement for {sale.InvoiceNumber}"));
            if (invoiceDiscount > 0)
                entry.AddLine(new JournalEntryLine(
                    settings.SalesDiscountAccountId!.Value, invoiceDiscount, 0,
                    warehouse.BranchId, warehouse.Id,
                    $"Sales discount on {sale.InvoiceNumber}"));
            entry.AddLine(new JournalEntryLine(
                revenueAccountId, 0, revenue,
                warehouse.BranchId, warehouse.Id,
                $"Revenue from {sale.InvoiceNumber}"));
            if (outputTax > 0)
                entry.AddLine(new JournalEntryLine(
                    sale.TaxOutputAccountId!.Value, 0, outputTax,
                    warehouse.BranchId, warehouse.Id,
                    $"Output tax on {sale.InvoiceNumber}"));

            if (cogs > 0)
            {
                entry.AddLine(new JournalEntryLine(
                    settings.CostOfSalesAccountId!.Value, cogs, 0,
                    warehouse.BranchId, warehouse.Id,
                    $"Cost of sales for {sale.InvoiceNumber}"));
                entry.AddLine(new JournalEntryLine(
                    warehouse.InventoryAccountId.Value, 0, cogs,
                    warehouse.BranchId, warehouse.Id,
                    $"Inventory issued for {sale.InvoiceNumber}"));
            }

            entry.Post();
            await journalEntryRepository.AddAsync(entry);
        });
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
