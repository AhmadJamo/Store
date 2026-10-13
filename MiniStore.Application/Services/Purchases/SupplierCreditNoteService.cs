using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SupplierCreditNoteService(
    ISupplierCreditNoteRepository creditNotes, IVendorBillRepository bills, IGoodsReceiptRepository receipts,
    IPurchaseOrderRepository orders, ISupplierRepository suppliers, IWarehouseRepository warehouses, IProductRepository products,
    IProductStockRepository stocks, IStockTransactionRepository transactions, IStockMovementRepository movements,
    IAccountingSettingsRepository accountingSettings, InventoryTrackingService tracking, UntrackedInventoryRemovalService untrackedRemoval,
    DocumentNumberService numbers, JournalPostingService journalPosting, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    public async Task<List<SupplierCreditNoteDto>> GetAllAsync()
    {
        var billMap = (await bills.GetAllAsync(null)).ToDictionary(x => x.Id);
        var supplierMap = (await suppliers.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var warehouseMap = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return (await creditNotes.GetAllAsync()).Select(x => Map(x, billMap.GetValueOrDefault(x.VendorBillId),
            supplierMap.GetValueOrDefault(x.SupplierId, "-"), warehouseMap.GetValueOrDefault(x.WarehouseId, "-"))).ToList();
    }

    public async Task<SupplierCreditNoteDto?> GetAsync(int id)
    {
        var note = await creditNotes.GetByIdAsync(id); if (note is null) return null;
        return Map(note, await bills.GetByIdAsync(note.VendorBillId),
            (await suppliers.GetByIdAsync(note.SupplierId))?.Name ?? "-", (await warehouses.GetByIdAsync(note.WarehouseId))?.Name ?? "-");
    }

    public async Task<SupplierCreditNoteCreatePageDto> GetCreatePageAsync(int vendorBillId, CreateSupplierCreditNoteDto? form = null)
    {
        var bill = await bills.GetByIdAsync(vendorBillId);
        if (bill is null || bill.Status != VendorBillStatus.Posted) return new() { VendorBillId = vendorBillId, Form = form ?? new() { VendorBillId = vendorBillId } };
        var order = await orders.GetByIdAsync(bill.PurchaseOrderId);
        var receiptList = (await receipts.GetByPurchaseOrderIdAsync(bill.PurchaseOrderId)).Where(x => x.Status == GoodsReceiptStatus.Posted).ToList();
        var receiptLineMap = receiptList.SelectMany(x => x.Lines.Select(line => new { Receipt = x, Line = line })).ToDictionary(x => x.Line.Id);
        var prior = (await creditNotes.GetByVendorBillIdAsync(bill.Id)).Where(x => x.Status == SupplierCreditNoteStatus.Posted)
            .SelectMany(x => x.Lines).GroupBy(x => x.VendorBillLineId).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));
        var lines = bill.Lines.Select(x =>
        {
            var credited = prior.GetValueOrDefault(x.Id);
            var receiptLine = receiptLineMap.GetValueOrDefault(x.GoodsReceiptLineId)?.Line;
            return new SupplierCreditNoteAvailableLineDto { VendorBillLineId = x.Id, ProductCode = x.ProductCodeSnapshot,
                ProductName = x.ProductNameSnapshot, UnitName = x.UnitNameSnapshot, BilledQuantity = x.Quantity,
                CreditedQuantity = credited, RemainingQuantity = Math.Max(0, x.ReceiptClearingQuantity - credited),
                ReceivedTracking = receiptLine?.LotNumber ?? receiptLine?.SerialNumbers };
        }).Where(x => x.RemainingQuantity > 0).ToList();
        return new()
        {
            VendorBillId = bill.Id, VendorBillNumber = bill.BillNumber, SupplierInvoiceNumber = bill.SupplierInvoiceNumber,
            SupplierName = (await suppliers.GetByIdAsync(bill.SupplierId))?.Name ?? "-", CurrencyCode = bill.CurrencyCode, Lines = lines,
            Form = form ?? new() { VendorBillId = bill.Id, Lines = lines.Select(x => new CreateSupplierCreditNoteLineDto { VendorBillLineId = x.VendorBillLineId }).ToList() }
        };
    }

    public async Task<int> CreateAndPostAsync(CreateSupplierCreditNoteDto dto)
    {
        var submitted = dto.Lines?.Where(x => x.Quantity > 0).ToList() ?? [];
        if (submitted.Count == 0) throw new ArgumentException("Enter at least one positive credited return quantity.");
        if (submitted.GroupBy(x => x.VendorBillLineId).Any(x => x.Count() > 1)) throw new ArgumentException("Each vendor bill line can be credited only once per supplier credit note.");
        SupplierCreditNote? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var bill = await bills.GetByIdAsync(dto.VendorBillId) ?? throw new InvalidOperationException("Vendor bill was not found.");
            if (bill.Status != VendorBillStatus.Posted) throw new InvalidOperationException("Only posted vendor bills can receive a supplier credit note.");
            if (dto.CreditDate < bill.BillDate) throw new ArgumentException("Supplier credit note date cannot be earlier than the vendor bill date.");
            var order = await orders.GetByIdAsync(bill.PurchaseOrderId) ?? throw new InvalidOperationException("Purchase order was not found.");
            var supplier = await suppliers.GetByIdAsync(bill.SupplierId) ?? throw new InvalidOperationException("Supplier was not found.");
            if (!supplier.AccountId.HasValue) throw new InvalidOperationException("Assign a payable account to the supplier before posting a supplier credit note.");
            var warehouse = await warehouses.GetByIdAsync(order.WarehouseId) ?? throw new InvalidOperationException("Purchase order warehouse was not found.");
            if (!warehouse.InventoryAccountId.HasValue) throw new InvalidOperationException("Configure the purchase order warehouse inventory account before posting a supplier credit note.");
            var settings = await accountingSettings.GetAsync() ?? throw new InvalidOperationException("Configure accounting posting accounts before posting a supplier credit note.");
            var receiptList = (await receipts.GetByPurchaseOrderIdAsync(order.Id)).Where(x => x.Status == GoodsReceiptStatus.Posted).ToList();
            var receiptLineMap = receiptList.SelectMany(x => x.Lines.Select(line => new { Receipt = x, Line = line })).ToDictionary(x => x.Line.Id);
            var billLineMap = bill.Lines.ToDictionary(x => x.Id);
            var priorLines = (await creditNotes.GetByVendorBillIdAsync(bill.Id)).Where(x => x.Status == SupplierCreditNoteStatus.Posted).SelectMany(x => x.Lines).ToList();
            created = new SupplierCreditNote(await numbers.GenerateAsync(DocumentNumberType.SupplierCreditNote, dto.CreditDate.ToDateTime(TimeOnly.MinValue)),
                bill.Id, bill.PurchaseOrderId, bill.SupplierId, order.WarehouseId, dto.CreditDate, bill.CurrencyCode,
                dto.SupplierReference, dto.Reason, currentUser.UserId);

            foreach (var input in submitted)
            {
                if (!billLineMap.TryGetValue(input.VendorBillLineId, out var source)) throw new ArgumentException("One or more credit lines do not belong to the vendor bill.");
                var priorForLine = priorLines.Where(x => x.VendorBillLineId == source.Id).ToList();
                var creditedQuantity = priorForLine.Sum(x => x.Quantity);
                var remainingQuantity = source.ReceiptClearingQuantity - creditedQuantity;
                if (input.Quantity > remainingQuantity) throw new InvalidOperationException("Credited return quantity cannot exceed the remaining billed quantity.");
                if (!receiptLineMap.TryGetValue(source.GoodsReceiptLineId, out var receiptSource)) throw new InvalidOperationException("The billed goods receipt line was not found.");
                var product = await products.GetByIdAsync(source.ProductId) ?? throw new InvalidOperationException("Returned product was not found.");
                var stockQuantity = input.Quantity * receiptSource.Line.UnitFactorToBase;
                var stock = await stocks.GetByProductAndWarehouseAsync(source.ProductId, order.WarehouseId) ?? throw new InvalidOperationException("Returned receipt stock was not found.");
                if (stockQuantity > stock.Quantity) throw new InvalidOperationException("Credited return quantity cannot exceed currently available warehouse stock.");
                await untrackedRemoval.RemoveAsync(product, order.WarehouseId, stockQuantity);
                var costMovement = stock.RemoveQuantity(stockQuantity);
                await tracking.ReturnPurchaseAsync(product, order.WarehouseId, stockQuantity, input.TrackingAllocations,
                    receiptSource.Receipt.ReceiptNumber, created.CreditNoteNumber, currentUser.UserId);
                var maximumNet = Round(source.NetAmount * source.ReceiptClearingQuantity / source.Quantity);
                var maximumTax = Round(source.TaxAmount * source.ReceiptClearingQuantity / source.Quantity);
                var finalRemainder = input.Quantity == remainingQuantity;
                var net = finalRemainder ? maximumNet - priorForLine.Sum(x => x.NetAmount) : Round(source.NetAmount * input.Quantity / source.Quantity);
                var tax = finalRemainder ? maximumTax - priorForLine.Sum(x => x.TaxAmount) : Round(source.TaxAmount * input.Quantity / source.Quantity);
                var gross = Round(net + tax);
                var companyNet = finalRemainder ? source.CompanyNetAmount - priorForLine.Sum(x => x.CompanyNetAmount) : Round(source.CompanyNetAmount * input.Quantity / source.Quantity);
                var companyTax = finalRemainder ? source.CompanyTaxAmount - priorForLine.Sum(x => x.CompanyTaxAmount) : Round(source.CompanyTaxAmount * input.Quantity / source.Quantity);
                created.AddLine(new SupplierCreditNoteLine(source.Id, source.GoodsReceiptLineId, source.ProductId, source.TaxInputAccountId,
                    input.Quantity, receiptSource.Line.UnitFactorToBase, net, tax, gross, Math.Abs(costMovement.TransactionValue),
                    source.ProductCodeSnapshot, source.ProductNameSnapshot, source.UnitNameSnapshot, input.TrackingAllocations,
                    companyNet, companyTax, companyNet + companyTax));
                await transactions.AddAsync(new StockTransaction(source.ProductId, order.WarehouseId, -stockQuantity,
                    StockTransactionType.SupplierCreditNoteReturn, created.CreditNoteNumber, costMovement));
            }

            await creditNotes.AddAsync(created); await unitOfWork.FlushAsync();
            foreach (var line in created.Lines) await movements.AddAsync(StockMovement.PostSupplierCreditNoteReturn(created.Id, line.Id,
                line.ProductId, created.WarehouseId, line.StockQuantity, currentUser.UserId, created.CreditNoteNumber));
            var journalLines = new List<JournalEntryLine>
            {
                new(supplier.AccountId.Value, created.Lines.Sum(x => x.CompanyGrossAmount), 0, warehouse.BranchId, warehouse.Id, $"Supplier credit {created.CreditNoteNumber}")
            };
            foreach (var taxGroup in created.Lines.Where(x => x.TaxAmount > 0).GroupBy(x => x.TaxInputAccountId!.Value))
                journalLines.Add(new JournalEntryLine(taxGroup.Key, 0, taxGroup.Sum(x => x.CompanyTaxAmount), warehouse.BranchId, warehouse.Id, $"Input tax reversal on {created.CreditNoteNumber}"));
            if (created.RemovedInventoryCost > 0)
                journalLines.Add(new JournalEntryLine(warehouse.InventoryAccountId.Value, 0, created.RemovedInventoryCost, warehouse.BranchId, warehouse.Id, $"Inventory returned on {created.CreditNoteNumber}"));
            var variance = Round(created.Lines.Sum(x => x.CompanyNetAmount) - created.RemovedInventoryCost);
            if (variance != 0)
            {
                if (!settings.PurchasePriceVarianceAccountId.HasValue) throw new InvalidOperationException("Configure a purchase price variance account before posting a supplier credit note with a cost difference.");
                journalLines.Add(variance > 0
                    ? new JournalEntryLine(settings.PurchasePriceVarianceAccountId.Value, 0, variance, warehouse.BranchId, warehouse.Id, $"Purchase price variance on {created.CreditNoteNumber}")
                    : new JournalEntryLine(settings.PurchasePriceVarianceAccountId.Value, Math.Abs(variance), 0, warehouse.BranchId, warehouse.Id, $"Purchase price variance on {created.CreditNoteNumber}"));
            }
            await journalPosting.PostAsync(new JournalPostingRequest(dto.CreditDate.ToDateTime(TimeOnly.MinValue),
                $"Supplier credit note {created.CreditNoteNumber}", "SupplierCreditNote", created.CreditNoteNumber, journalLines,
                "This supplier credit note has already been posted to accounting."));
            created.Post(currentUser.UserId);
        });
        return created!.Id;
    }

    private static SupplierCreditNoteDto Map(SupplierCreditNote note, VendorBill? bill, string supplier, string warehouse) => new()
    {
        Id = note.Id, VendorBillId = note.VendorBillId, CreditNoteNumber = note.CreditNoteNumber,
        VendorBillNumber = bill?.BillNumber ?? "-", SupplierInvoiceNumber = bill?.SupplierInvoiceNumber ?? "-",
        SupplierName = supplier, WarehouseName = warehouse, CreditDate = note.CreditDate, CurrencyCode = note.CurrencyCode,
        SupplierReference = note.SupplierReference, Reason = note.Reason, Status = note.Status, NetAmount = note.NetAmount,
        TaxAmount = note.TaxAmount, TotalAmount = note.TotalAmount, RemovedInventoryCost = note.RemovedInventoryCost,
        Lines = note.Lines.Select(x => new SupplierCreditNoteLineDto { ProductCode = x.ProductCodeSnapshot,
            ProductName = x.ProductNameSnapshot, UnitName = x.UnitNameSnapshot, Quantity = x.Quantity,
            StockQuantity = x.StockQuantity, NetAmount = x.NetAmount, TaxAmount = x.TaxAmount, GrossAmount = x.GrossAmount,
            RemovedInventoryCost = x.RemovedInventoryCost, TrackingAllocations = x.TrackingAllocations }).ToList()
    };

    private static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
