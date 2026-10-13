using System.Globalization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class PurchaseDocumentReversalService(
    IPurchaseDocumentReversalRepository reversals, IGoodsReceiptRepository receipts, IGoodsReceiptReturnRepository receiptReturns,
    IVendorBillRepository bills, ISupplierPaymentRepository payments, ISupplierCreditNoteRepository credits, ILandedCostRepository landedCosts,
    IPurchaseOrderRepository orders, IWarehouseRepository warehouses, IProductRepository products, IProductStockRepository stocks,
    IStockTransactionRepository transactions, IStockMovementRepository movements, InventoryTrackingService tracking,
    UntrackedInventoryRemovalService untrackedRemoval, DocumentNumberService numbers, JournalPostingService journalPosting,
    IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    private const decimal Tolerance = 0.000001m;

    public async Task<List<PurchaseDocumentReversalDto>> GetAllAsync() =>
        (await reversals.GetAllAsync()).Select(Map).ToList();

    public async Task<PurchaseDocumentReversalDto?> GetAsync(int id) =>
        await reversals.GetByIdAsync(id) is { } value ? Map(value) : null;

    public async Task<PurchaseDocumentReversalDto?> GetByOriginalAsync(PurchaseDocumentType type, int id) =>
        await reversals.GetByOriginalAsync(type, id) is { } value ? Map(value) : null;

    public async Task<PurchaseDocumentReversalCreatePageDto> GetCreatePageAsync(PurchaseDocumentType type, int originalDocumentId,
        CreatePurchaseDocumentReversalDto? form = null)
    {
        var source = await GetSourceAsync(type, originalDocumentId);
        var existing = await reversals.GetByOriginalAsync(type, originalDocumentId);
        return new PurchaseDocumentReversalCreatePageDto
        {
            Form = form ?? new CreatePurchaseDocumentReversalDto { DocumentType = type, OriginalDocumentId = originalDocumentId },
            OriginalDocumentNumber = source.Number, OriginalDocumentDate = source.Date,
            OriginalController = Controller(type), ExistingReversalId = existing?.Id
        };
    }

    public async Task<int> ReverseAsync(CreatePurchaseDocumentReversalDto dto)
    {
        var key = (dto.IdempotencyKey ?? string.Empty).Trim().ToUpperInvariant();
        if (await reversals.GetByRequestKeyAsync(key) is { } retry)
        {
            if (retry.DocumentType != dto.DocumentType || retry.OriginalDocumentId != dto.OriginalDocumentId)
                throw new InvalidOperationException("This reversal request key was already used for another document.");
            return retry.Id;
        }

        PurchaseDocumentReversal? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (await reversals.GetByOriginalAsync(dto.DocumentType, dto.OriginalDocumentId) is not null)
                throw new InvalidOperationException("This purchase document has already been reversed.");
            var source = await GetSourceAsync(dto.DocumentType, dto.OriginalDocumentId);
            if (dto.ReversalDate < source.Date) throw new ArgumentException("Reversal date cannot be earlier than the original document date.");
            created = new PurchaseDocumentReversal(
                await numbers.GenerateAsync(DocumentNumberType.PurchaseDocumentReversal, dto.ReversalDate.ToDateTime(TimeOnly.MinValue)),
                dto.DocumentType, dto.OriginalDocumentId, source.Number, dto.ReversalDate, dto.Reason, key, currentUser.UserId);
            await reversals.AddAsync(created); await unitOfWork.FlushAsync();

            var journal = dto.DocumentType switch
            {
                PurchaseDocumentType.GoodsReceipt => await ReverseReceiptAsync(created, dto.OriginalDocumentId),
                PurchaseDocumentType.VendorBill => await ReverseBillAsync(created, dto.OriginalDocumentId),
                PurchaseDocumentType.SupplierPayment => await ReversePaymentAsync(created, dto.OriginalDocumentId),
                PurchaseDocumentType.LandedCost => await ReverseLandedCostAsync(created, dto.OriginalDocumentId),
                _ => throw new ArgumentException("Unsupported purchase document type.")
            };
            created.Post(journal.EntryNumber);
        });
        return created!.Id;
    }

    private async Task<JournalEntry> ReversePaymentAsync(PurchaseDocumentReversal reversal, int id)
    {
        var payment = await payments.GetByIdAsync(id) ?? throw new InvalidOperationException("Supplier payment was not found.");
        if (payment.Status != SupplierPaymentStatus.Posted) throw new InvalidOperationException("Only posted supplier payments can be reversed.");
        var journal = await ReverseJournalAsync("SupplierPayment", payment.PaymentNumber, reversal);
        payment.Reverse(currentUser.UserId); return journal;
    }

    private async Task<JournalEntry> ReverseBillAsync(PurchaseDocumentReversal reversal, int id)
    {
        var bill = await bills.GetByIdAsync(id) ?? throw new InvalidOperationException("Vendor bill was not found.");
        if (bill.Status != VendorBillStatus.Posted) throw new InvalidOperationException("Only posted vendor bills can be reversed.");
        if ((await payments.GetAllAsync(null)).Any(x => x.Status == SupplierPaymentStatus.Posted && x.Lines.Any(line => line.VendorBillId == id)))
            throw new InvalidOperationException("Reverse supplier payments allocated to this vendor bill before reversing the bill.");
        if ((await credits.GetByVendorBillIdAsync(id)).Any(x => x.Status == SupplierCreditNoteStatus.Posted))
            throw new InvalidOperationException("A vendor bill with posted supplier credit notes cannot be reversed.");
        var journal = await ReverseJournalAsync("VendorBill", bill.BillNumber, reversal);
        bill.Reverse(currentUser.UserId);
        (await orders.GetByIdAsync(bill.PurchaseOrderId))?.ReopenAfterDownstreamReversal(currentUser.UserId, $"Reversal {reversal.ReversalNumber}");
        return journal;
    }

    private async Task<JournalEntry> ReverseReceiptAsync(PurchaseDocumentReversal reversal, int id)
    {
        var receipt = await receipts.GetByIdAsync(id) ?? throw new InvalidOperationException("Goods receipt was not found.");
        if (receipt.Status != GoodsReceiptStatus.Posted) throw new InvalidOperationException("Only posted goods receipts can be reversed.");
        if ((await receiptReturns.GetByGoodsReceiptIdAsync(id)).Any(x => x.Status == GoodsReceiptReturnStatus.Posted))
            throw new InvalidOperationException("A goods receipt with posted returns cannot be reversed.");
        var receiptLineIds = receipt.Lines.Select(x => x.Id).ToHashSet();
        if ((await bills.GetByPurchaseOrderIdAsync(receipt.PurchaseOrderId)).Any(x => x.Status != VendorBillStatus.Cancelled && x.Lines.Any(line => line.GoodsReceiptLineId.HasValue && receiptLineIds.Contains(line.GoodsReceiptLineId.Value))))
            throw new InvalidOperationException("Cancel draft or approved vendor bills and reverse posted vendor bills linked to this goods receipt before reversing the receipt.");
        if ((await landedCosts.GetAllAsync()).Any(x => x.GoodsReceiptId == id && x.Status == LandedCostStatus.Posted))
            throw new InvalidOperationException("Reverse landed costs linked to this goods receipt before reversing the receipt.");
        var warehouse = await warehouses.GetByIdAsync(receipt.WarehouseId) ?? throw new InvalidOperationException("Receipt warehouse was not found.");
        foreach (var group in receipt.Lines.GroupBy(x => x.ProductId))
        {
            var stock = await stocks.GetByProductAndWarehouseAsync(group.Key, receipt.WarehouseId)
                ?? throw new InvalidOperationException("Received product stock was not found.");
            var quantity = group.Sum(x => x.StockQuantity);
            var originalValue = group.Sum(x => decimal.Round(x.ReceivedQuantity * x.UnitCost * receipt.ExchangeRate, 8, MidpointRounding.AwayFromZero));
            var originalUnitCost = originalValue / quantity;
            if (stock.Quantity + Tolerance < quantity) throw new InvalidOperationException("The goods receipt cannot be reversed because some received stock is no longer available.");
            if (Math.Abs(stock.AverageUnitCost - originalUnitCost) > Tolerance)
                throw new InvalidOperationException("The goods receipt cannot be reversed after later inventory valuation changed its cost.");
        }
        foreach (var line in receipt.Lines)
        {
            var product = await products.GetByIdAsync(line.ProductId) ?? throw new InvalidOperationException("Purchase order product was not found.");
            await untrackedRemoval.RemoveAsync(product, receipt.WarehouseId, line.StockQuantity);
            var stock = await stocks.GetByProductAndWarehouseAsync(line.ProductId, receipt.WarehouseId)
                ?? throw new InvalidOperationException("Received product stock was not found.");
            var cost = stock.RemoveQuantity(line.StockQuantity);
            await tracking.ReturnPurchaseAsync(product, receipt.WarehouseId, line.StockQuantity, TrackingAllocation(line),
                receipt.ReceiptNumber, reversal.ReversalNumber, currentUser.UserId);
            await transactions.AddAsync(new StockTransaction(line.ProductId, receipt.WarehouseId, -line.StockQuantity,
                StockTransactionType.GoodsReceiptReversal, reversal.ReversalNumber, cost));
            await movements.AddAsync(StockMovement.PostGoodsReceiptReversal(reversal.Id, line.Id, line.ProductId,
                receipt.WarehouseId, line.StockQuantity, currentUser.UserId, reversal.ReversalNumber));
        }
        var journal = await ReverseJournalAsync("GoodsReceipt", receipt.ReceiptNumber, reversal);
        receipt.Reverse(currentUser.UserId);
        (await orders.GetByIdAsync(receipt.PurchaseOrderId))?.ReopenAfterDownstreamReversal(currentUser.UserId, $"Reversal {reversal.ReversalNumber}");
        return journal;
    }

    private async Task<JournalEntry> ReverseLandedCostAsync(PurchaseDocumentReversal reversal, int id)
    {
        var cost = await landedCosts.GetByIdAsync(id) ?? throw new InvalidOperationException("Landed cost was not found.");
        if (cost.Status != LandedCostStatus.Posted) throw new InvalidOperationException("Only posted landed costs can be reversed.");
        foreach (var group in cost.Allocations.Where(x => x.CapitalizedAmount > 0).GroupBy(x => x.ProductId))
        {
            var stock = await stocks.GetByProductAndWarehouseAsync(group.Key, cost.WarehouseId)
                ?? throw new InvalidOperationException("Received product stock was not found.");
            var allocation = group.First();
            if (Math.Abs(stock.AverageUnitCost - allocation.AverageUnitCostAfter) > Tolerance)
                throw new InvalidOperationException("The landed cost cannot be reversed after later inventory valuation changed the product cost.");
            var delta = allocation.AverageUnitCostAfter - allocation.AverageUnitCostBefore;
            if (delta <= 0) throw new InvalidOperationException("The landed cost capitalization history cannot be reversed safely.");
            var originalQuantity = group.Sum(x => x.CapitalizedAmount) / delta;
            if (stock.Quantity + Tolerance < originalQuantity)
                throw new InvalidOperationException("The landed cost cannot be reversed after related inventory was consumed.");
            var movement = stock.RemoveLandedCost(group.Sum(x => x.CapitalizedAmount));
            await transactions.AddAsync(new StockTransaction(group.Key, cost.WarehouseId, 0,
                StockTransactionType.LandedCostReversal, reversal.ReversalNumber, movement));
        }
        var journal = await ReverseJournalAsync("LandedCost", cost.CostNumber, reversal);
        cost.Reverse(currentUser.UserId); return journal;
    }

    private Task<JournalEntry> ReverseJournalAsync(string sourceType, string sourceNumber, PurchaseDocumentReversal reversal) =>
        journalPosting.ReverseAsync(sourceType, sourceNumber, reversal.ReversalNumber,
            reversal.ReversalDate.ToDateTime(TimeOnly.MinValue), reversal.Reason);

    private async Task<(string Number, DateOnly Date)> GetSourceAsync(PurchaseDocumentType type, int id) => type switch
    {
        PurchaseDocumentType.GoodsReceipt => await receipts.GetByIdAsync(id) is { } x ? (x.ReceiptNumber, x.ReceiptDate) : throw new InvalidOperationException("Goods receipt was not found."),
        PurchaseDocumentType.VendorBill => await bills.GetByIdAsync(id) is { } x ? (x.BillNumber, x.BillDate) : throw new InvalidOperationException("Vendor bill was not found."),
        PurchaseDocumentType.SupplierPayment => await payments.GetByIdAsync(id) is { } x ? (x.PaymentNumber, x.PaymentDate) : throw new InvalidOperationException("Supplier payment was not found."),
        PurchaseDocumentType.LandedCost => await landedCosts.GetByIdAsync(id) is { } x ? (x.CostNumber, x.CostDate) : throw new InvalidOperationException("Landed cost was not found."),
        _ => throw new ArgumentException("Unsupported purchase document type.")
    };

    private static string? TrackingAllocation(GoodsReceiptLine line) => line.LotNumber is not null
        ? $"{line.LotNumber}:{line.StockQuantity.ToString(CultureInfo.InvariantCulture)}"
        : line.SerialNumbers;

    private static PurchaseDocumentReversalDto Map(PurchaseDocumentReversal x) => new()
    {
        Id = x.Id, ReversalNumber = x.ReversalNumber, DocumentType = x.DocumentType,
        OriginalDocumentId = x.OriginalDocumentId, OriginalDocumentNumber = x.OriginalDocumentNumber,
        ReversalDate = x.ReversalDate, Reason = x.Reason, Status = x.Status, CreatedByUserId = x.CreatedByUserId,
        CreatedAtUtc = x.CreatedAtUtc, ReversalJournalEntryNumber = x.ReversalJournalEntryNumber,
        OriginalController = Controller(x.DocumentType)
    };

    private static string Controller(PurchaseDocumentType type) => type switch
    {
        PurchaseDocumentType.GoodsReceipt => "GoodsReceipts",
        PurchaseDocumentType.VendorBill => "VendorBills",
        PurchaseDocumentType.SupplierPayment => "SupplierPayments",
        PurchaseDocumentType.LandedCost => "LandedCosts",
        _ => string.Empty
    };
}
