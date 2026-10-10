using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class VendorBillService(
    IVendorBillRepository bills, IPurchaseOrderRepository orders, IGoodsReceiptRepository receipts,
    IGoodsReceiptReturnRepository receiptReturns, ISupplierRepository suppliers, IWarehouseRepository warehouses,
    ITaxRateRepository taxRates, IAccountingSettingsRepository accountingSettings, DocumentNumberService numbers,
    JournalPostingService journalPosting, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    public async Task<List<VendorBillDto>> GetAllAsync(string? search)
    {
        var orderMap = (await orders.GetAllAsync(null, null)).ToDictionary(x => x.Id, x => x.OrderNumber);
        var supplierMap = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return (await bills.GetAllAsync(search)).Select(x => Map(x, orderMap.GetValueOrDefault(x.PurchaseOrderId, "-"), supplierMap.GetValueOrDefault(x.SupplierId, "-"))).ToList();
    }

    public async Task<VendorBillDto?> GetAsync(int id)
    {
        var bill = await bills.GetByIdAsync(id); if (bill is null) return null;
        var order = await orders.GetByIdAsync(bill.PurchaseOrderId); var supplier = await suppliers.GetByIdAsync(bill.SupplierId);
        return Map(bill, order?.OrderNumber ?? "-", supplier?.Name ?? "-");
    }

    public async Task<VendorBillCreatePageDto> GetCreatePageAsync(int purchaseOrderId, CreateVendorBillDto? form = null)
    {
        var order = await orders.GetByIdAsync(purchaseOrderId);
        if (order is null) return new() { PurchaseOrderId = purchaseOrderId, Form = form ?? new() { PurchaseOrderId = purchaseOrderId } };
        var supplier = await suppliers.GetByIdAsync(order.SupplierId);
        var taxes = await taxRates.GetAllAsync();
        var available = await GetAvailableLinesAsync(order, taxes);
        return new()
        {
            PurchaseOrderId = order.Id, PurchaseOrderNumber = order.OrderNumber, SupplierName = supplier?.Name ?? "-", CurrencyCode = order.CurrencyCode,
            Lines = available, TaxRates = taxes.Select(x => new VendorBillTaxRateDto { Id = x.Id, Name = x.Name, Rate = x.Rate, IsPriceInclusive = x.IsPriceInclusive }).ToList(),
            Form = form ?? new CreateVendorBillDto
            {
                PurchaseOrderId = order.Id,
                Lines = available.Select(x => new CreateVendorBillLineDto { GoodsReceiptLineId = x.GoodsReceiptLineId, UnitPrice = x.SuggestedUnitPrice, TaxRateId = x.SuggestedTaxRateId }).ToList()
            }
        };
    }

    public async Task<int> CreateAndPostAsync(CreateVendorBillDto dto)
    {
        var submitted = dto.Lines?.Where(x => x.Quantity > 0).ToList() ?? [];
        if (submitted.Count == 0) throw new ArgumentException("Enter at least one positive billed quantity.");
        if (submitted.GroupBy(x => x.GoodsReceiptLineId).Any(x => x.Count() > 1)) throw new ArgumentException("Each goods receipt line can be billed only once per vendor bill.");
        VendorBill? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var order = await orders.GetByIdAsync(dto.PurchaseOrderId) ?? throw new InvalidOperationException("Purchase order was not found.");
            if (order.Status is not (PurchaseOrderStatus.Confirmed or PurchaseOrderStatus.Closed)) throw new InvalidOperationException("Only confirmed purchase orders can be billed.");
            var supplier = await suppliers.GetByIdAsync(order.SupplierId) ?? throw new InvalidOperationException("Supplier was not found.");
            if (!supplier.AccountId.HasValue) throw new InvalidOperationException("Assign a payable account to the supplier before posting a vendor bill.");
            var normalizedInvoice = VendorBill.NormalizeInvoiceNumber(dto.SupplierInvoiceNumber);
            if (normalizedInvoice.Length == 0) throw new ArgumentException("Supplier invoice number must contain at least one letter or number.");
            if (await bills.ExistsActiveSupplierInvoiceAsync(order.SupplierId, normalizedInvoice)) throw new InvalidOperationException("This supplier invoice number has already been used.");
            var warehouse = await warehouses.GetByIdAsync(order.WarehouseId) ?? throw new InvalidOperationException("Purchase order warehouse was not found.");
            var settings = await accountingSettings.GetAsync() ?? throw new InvalidOperationException("Configure accounting posting accounts before posting a vendor bill.");
            if (!settings.GoodsReceivedNotInvoicedAccountId.HasValue) throw new InvalidOperationException("Configure a goods received not invoiced account before posting a vendor bill.");

            var taxMap = (await taxRates.GetAllAsync()).ToDictionary(x => x.Id);
            var receiptList = (await receipts.GetByPurchaseOrderIdAsync(order.Id)).Where(x => x.Status == GoodsReceiptStatus.Posted).ToList();
            var receiptLineMap = receiptList.SelectMany(x => x.Lines).ToDictionary(x => x.Id);
            var returned = await GetReturnedQuantitiesAsync(receiptList);
            var billed = await GetBilledQuantitiesAsync(order.Id);
            created = new VendorBill(await numbers.GenerateAsync(DocumentNumberType.VendorBill, dto.BillDate.ToDateTime(TimeOnly.MinValue)),
                order.Id, order.SupplierId, dto.SupplierInvoiceNumber, dto.BillDate, order.CurrencyCode, dto.Notes, currentUser.UserId);

            foreach (var input in submitted)
            {
                if (!receiptLineMap.TryGetValue(input.GoodsReceiptLineId, out var source)) throw new ArgumentException("One or more vendor bill lines do not belong to a posted receipt for this purchase order.");
                var remaining = source.ReceivedQuantity - returned.GetValueOrDefault(source.Id) - billed.GetValueOrDefault(source.Id);
                if (input.Quantity > remaining) throw new InvalidOperationException("Billed quantity cannot exceed the received quantity available for billing.");
                if (input.UnitPrice < 0) throw new ArgumentException("Vendor bill unit price cannot be negative.");
                TaxRate? tax = null;
                if (input.TaxRateId.HasValue && !taxMap.TryGetValue(input.TaxRateId.Value, out tax)) throw new InvalidOperationException("The selected purchase tax rate was not found.");
                created.AddLine(new VendorBillLine(source.Id, source.PurchaseOrderLineId, source.ProductId, input.Quantity, input.UnitPrice,
                    source.UnitCost, tax?.Id, tax?.Rate ?? 0, tax?.InputAccountId, tax?.IsPriceInclusive ?? false,
                    source.ProductCodeSnapshot, source.ProductNameSnapshot, source.UnitNameSnapshot));
            }

            if (created.TotalAmount <= 0) throw new InvalidOperationException("Vendor bill total must be greater than zero.");
            await bills.AddAsync(created); await unitOfWork.FlushAsync();
            var journalLines = new List<JournalEntryLine>();
            if (created.ReceiptClearingAmount > 0) journalLines.Add(new JournalEntryLine(settings.GoodsReceivedNotInvoicedAccountId.Value,
                created.ReceiptClearingAmount, 0, warehouse.BranchId, warehouse.Id, $"GRNI clearing for {created.BillNumber}"));
            var variance = Round(created.NetAmount - created.ReceiptClearingAmount);
            if (variance != 0)
            {
                if (!settings.PurchasePriceVarianceAccountId.HasValue) throw new InvalidOperationException("Configure a purchase price variance account before posting a vendor bill with a price difference.");
                journalLines.Add(variance > 0
                    ? new JournalEntryLine(settings.PurchasePriceVarianceAccountId.Value, variance, 0, warehouse.BranchId, warehouse.Id, $"Purchase price variance on {created.BillNumber}")
                    : new JournalEntryLine(settings.PurchasePriceVarianceAccountId.Value, 0, Math.Abs(variance), warehouse.BranchId, warehouse.Id, $"Purchase price variance on {created.BillNumber}"));
            }
            foreach (var taxGroup in created.Lines.Where(x => x.TaxAmount > 0).GroupBy(x => x.TaxInputAccountId!.Value))
                journalLines.Add(new JournalEntryLine(taxGroup.Key, taxGroup.Sum(x => x.TaxAmount), 0, warehouse.BranchId, warehouse.Id, $"Input tax on {created.BillNumber}"));
            journalLines.Add(new JournalEntryLine(supplier.AccountId.Value, 0, created.TotalAmount, warehouse.BranchId, warehouse.Id, $"Supplier payable for {created.BillNumber}"));
            await journalPosting.PostAsync(new JournalPostingRequest(dto.BillDate.ToDateTime(TimeOnly.MinValue), $"Vendor bill {created.BillNumber}",
                "VendorBill", created.BillNumber, journalLines, "This vendor bill has already been posted to accounting."));
            created.Post(currentUser.UserId);
        });
        return created!.Id;
    }

    private async Task<List<VendorBillAvailableLineDto>> GetAvailableLinesAsync(PurchaseOrder order, List<TaxRate> taxes)
    {
        var receiptList = (await receipts.GetByPurchaseOrderIdAsync(order.Id)).Where(x => x.Status == GoodsReceiptStatus.Posted).ToList();
        var returned = await GetReturnedQuantitiesAsync(receiptList); var billed = await GetBilledQuantitiesAsync(order.Id);
        var orderLines = order.Lines.ToDictionary(x => x.Id);
        return receiptList.SelectMany(receipt => receipt.Lines.Select(line => new { receipt, line }))
            .Select(x =>
            {
                var orderLine = orderLines[x.line.PurchaseOrderLineId];
                return new VendorBillAvailableLineDto
                {
                    GoodsReceiptLineId = x.line.Id, ReceiptNumber = x.receipt.ReceiptNumber, ReceiptDate = x.receipt.ReceiptDate,
                    ProductCode = x.line.ProductCodeSnapshot, ProductName = x.line.ProductNameSnapshot, UnitName = x.line.UnitNameSnapshot,
                    RemainingQuantity = Math.Max(0, x.line.ReceivedQuantity - returned.GetValueOrDefault(x.line.Id) - billed.GetValueOrDefault(x.line.Id)),
                    SuggestedTaxRateId = taxes.FirstOrDefault(t => t.Rate == orderLine.TaxPercent)?.Id,
                    SuggestedUnitPrice = taxes.FirstOrDefault(t => t.Rate == orderLine.TaxPercent) is { IsPriceInclusive: true } inclusiveTax
                        ? decimal.Round(x.line.UnitCost * (1m + inclusiveTax.Rate / 100m), 8, MidpointRounding.AwayFromZero)
                        : x.line.UnitCost
                };
            }).Where(x => x.RemainingQuantity > 0).ToList();
    }

    private async Task<Dictionary<int, decimal>> GetReturnedQuantitiesAsync(List<GoodsReceipt> receiptList)
    {
        var all = new List<GoodsReceiptReturn>();
        foreach (var receipt in receiptList) all.AddRange(await receiptReturns.GetByGoodsReceiptIdAsync(receipt.Id));
        return all.Where(x => x.Status == GoodsReceiptReturnStatus.Posted).SelectMany(x => x.Lines).GroupBy(x => x.GoodsReceiptLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.ReturnQuantity));
    }

    private async Task<Dictionary<int, decimal>> GetBilledQuantitiesAsync(int orderId) => (await bills.GetByPurchaseOrderIdAsync(orderId))
        .Where(x => x.Status == VendorBillStatus.Posted).SelectMany(x => x.Lines).GroupBy(x => x.GoodsReceiptLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));

    private static VendorBillDto Map(VendorBill x, string orderNumber, string supplierName) => new()
    {
        Id = x.Id, BillNumber = x.BillNumber, PurchaseOrderNumber = orderNumber, SupplierName = supplierName,
        SupplierInvoiceNumber = x.SupplierInvoiceNumber, BillDate = x.BillDate, CurrencyCode = x.CurrencyCode, Notes = x.Notes,
        Status = x.Status, NetAmount = x.NetAmount, TaxAmount = x.TaxAmount, TotalAmount = x.TotalAmount, ReceiptClearingAmount = x.ReceiptClearingAmount,
        Lines = x.Lines.Select(line => new VendorBillLineDto { ProductCode = line.ProductCodeSnapshot, ProductName = line.ProductNameSnapshot,
            UnitName = line.UnitNameSnapshot, Quantity = line.Quantity, UnitPrice = line.UnitPrice, TaxPercent = line.TaxPercent,
            IsTaxInclusive = line.IsTaxInclusive, NetAmount = line.NetAmount, TaxAmount = line.TaxAmount, GrossAmount = line.GrossAmount,
            ReceiptClearingAmount = line.ReceiptClearingAmount }).ToList()
    };
    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
