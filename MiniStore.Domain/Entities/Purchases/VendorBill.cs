namespace MiniStore.Domain.Entities;

public enum VendorBillStatus { Draft = 1, Posted = 2, Cancelled = 3 }

public sealed class VendorBill
{
    private VendorBill() { BillNumber = SupplierInvoiceNumber = NormalizedSupplierInvoiceNumber = CurrencyCode = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string BillNumber { get; private set; }
    public int PurchaseOrderId { get; private set; }
    public int SupplierId { get; private set; }
    public string SupplierInvoiceNumber { get; private set; }
    public string NormalizedSupplierInvoiceNumber { get; private set; }
    public DateOnly BillDate { get; private set; }
    public string CurrencyCode { get; private set; }
    public string? Notes { get; private set; }
    public VendorBillStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? PostedByUserId { get; private set; }
    public DateTime? PostedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<VendorBillLine> Lines { get; private set; } = [];
    public decimal NetAmount => Lines.Sum(x => x.NetAmount);
    public decimal TaxAmount => Lines.Sum(x => x.TaxAmount);
    public decimal TotalAmount => Lines.Sum(x => x.GrossAmount);
    public decimal ReceiptClearingAmount => Lines.Sum(x => x.ReceiptClearingAmount);

    public VendorBill(string billNumber, int purchaseOrderId, int supplierId, string supplierInvoiceNumber,
        DateOnly billDate, string currencyCode, string? notes, string userId)
    {
        if (string.IsNullOrWhiteSpace(billNumber) || billNumber.Trim().Length > 50) throw new ArgumentException("Vendor bill number is required and cannot exceed 50 characters.");
        if (purchaseOrderId <= 0 || supplierId <= 0) throw new ArgumentException("Purchase order and supplier are required.");
        if (string.IsNullOrWhiteSpace(supplierInvoiceNumber) || supplierInvoiceNumber.Trim().Length > 100) throw new ArgumentException("Supplier invoice number is required and cannot exceed 100 characters.");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3) throw new ArgumentException("Currency code must contain exactly three letters.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Vendor bill notes cannot exceed 1000 characters.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        BillNumber = billNumber.Trim(); PurchaseOrderId = purchaseOrderId; SupplierId = supplierId;
        SupplierInvoiceNumber = supplierInvoiceNumber.Trim(); NormalizedSupplierInvoiceNumber = NormalizeInvoiceNumber(supplierInvoiceNumber);
        if (NormalizedSupplierInvoiceNumber.Length == 0) throw new ArgumentException("Supplier invoice number must contain at least one letter or number.");
        BillDate = billDate; CurrencyCode = currencyCode.Trim().ToUpperInvariant(); Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow; Status = VendorBillStatus.Draft;
    }

    public void AddLine(VendorBillLine line)
    {
        EnsureDraft();
        if (Lines.Any(x => x.GoodsReceiptLineId == line.GoodsReceiptLineId)) throw new InvalidOperationException("Each goods receipt line can be billed only once per vendor bill.");
        Lines.Add(line);
    }

    public void Post(string userId)
    {
        EnsureDraft();
        if (Lines.Count == 0) throw new InvalidOperationException("A vendor bill must contain at least one line.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = VendorBillStatus.Posted; PostedByUserId = userId.Trim(); PostedAtUtc = DateTime.UtcNow;
    }

    public static string NormalizeInvoiceNumber(string value) => string.Concat((value ?? string.Empty).Trim().ToUpperInvariant().Where(char.IsLetterOrDigit));
    private void EnsureDraft() { if (Status != VendorBillStatus.Draft) throw new InvalidOperationException("Only draft vendor bills can be changed."); }
}

public sealed class VendorBillLine
{
    private VendorBillLine() { ProductCodeSnapshot = ProductNameSnapshot = UnitNameSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int VendorBillId { get; private set; }
    public int GoodsReceiptLineId { get; private set; }
    public int PurchaseOrderLineId { get; private set; }
    public int ProductId { get; private set; }
    public int? TaxRateId { get; private set; }
    public int? TaxInputAccountId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal ReceiptUnitCost { get; private set; }
    public decimal TaxPercent { get; private set; }
    public bool IsTaxInclusive { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal ReceiptClearingAmount { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string UnitNameSnapshot { get; private set; }

    public VendorBillLine(int goodsReceiptLineId, int purchaseOrderLineId, int productId, decimal quantity, decimal unitPrice,
        decimal receiptUnitCost, int? taxRateId, decimal taxPercent, int? taxInputAccountId, bool isTaxInclusive,
        string productCode, string productName, string unitName)
    {
        if (goodsReceiptLineId <= 0 || purchaseOrderLineId <= 0 || productId <= 0) throw new ArgumentException("Vendor bill line references are required.");
        if (quantity <= 0 || unitPrice < 0 || receiptUnitCost < 0) throw new ArgumentException("Vendor bill quantity and prices are invalid.");
        if (taxPercent is < 0 or > 100) throw new ArgumentException("Vendor bill tax percentage must be between 0 and 100.");
        if (taxPercent > 0 && (!taxRateId.HasValue || !taxInputAccountId.HasValue)) throw new ArgumentException("A tax rate and input tax account are required for a taxed vendor bill line.");
        if (string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(unitName)) throw new ArgumentException("Vendor bill line snapshots are required.");
        GoodsReceiptLineId = goodsReceiptLineId; PurchaseOrderLineId = purchaseOrderLineId; ProductId = productId; Quantity = quantity;
        UnitPrice = unitPrice; ReceiptUnitCost = receiptUnitCost; TaxRateId = taxRateId; TaxPercent = taxPercent;
        TaxInputAccountId = taxInputAccountId; IsTaxInclusive = isTaxInclusive; ProductCodeSnapshot = productCode.Trim();
        ProductNameSnapshot = productName.Trim(); UnitNameSnapshot = unitName.Trim();
        var enteredAmount = Round(quantity * unitPrice);
        TaxAmount = taxPercent <= 0 ? 0 : isTaxInclusive ? Round(enteredAmount * taxPercent / (100m + taxPercent)) : Round(enteredAmount * taxPercent / 100m);
        NetAmount = isTaxInclusive ? enteredAmount - TaxAmount : enteredAmount;
        GrossAmount = isTaxInclusive ? enteredAmount : enteredAmount + TaxAmount;
        ReceiptClearingAmount = Round(quantity * receiptUnitCost);
    }
    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
