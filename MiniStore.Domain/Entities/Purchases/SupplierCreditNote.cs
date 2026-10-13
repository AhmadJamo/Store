namespace MiniStore.Domain.Entities;

public enum SupplierCreditNoteStatus { Draft = 1, Posted = 2 }

public sealed class SupplierCreditNote
{
    private SupplierCreditNote() { CreditNoteNumber = CurrencyCode = Reason = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string CreditNoteNumber { get; private set; }
    public int VendorBillId { get; private set; }
    public int PurchaseOrderId { get; private set; }
    public int SupplierId { get; private set; }
    public int WarehouseId { get; private set; }
    public DateOnly CreditDate { get; private set; }
    public string CurrencyCode { get; private set; }
    public string? SupplierReference { get; private set; }
    public string Reason { get; private set; }
    public SupplierCreditNoteStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? PostedByUserId { get; private set; }
    public DateTime? PostedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<SupplierCreditNoteLine> Lines { get; private set; } = [];
    public decimal NetAmount => Lines.Sum(x => x.NetAmount);
    public decimal TaxAmount => Lines.Sum(x => x.TaxAmount);
    public decimal TotalAmount => Lines.Sum(x => x.GrossAmount);
    public decimal RemovedInventoryCost => Lines.Sum(x => x.RemovedInventoryCost);

    public SupplierCreditNote(string number, int vendorBillId, int purchaseOrderId, int supplierId, int warehouseId,
        DateOnly creditDate, string currencyCode, string? supplierReference, string reason, string userId)
    {
        if (string.IsNullOrWhiteSpace(number) || number.Trim().Length > 50) throw new ArgumentException("Supplier credit note number is required and cannot exceed 50 characters.");
        if (vendorBillId <= 0 || purchaseOrderId <= 0 || supplierId <= 0 || warehouseId <= 0) throw new ArgumentException("Supplier credit note references are required.");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3) throw new ArgumentException("Currency code must contain exactly three letters.");
        if (supplierReference?.Trim().Length > 100) throw new ArgumentException("Supplier credit note reference cannot exceed 100 characters.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 1000) throw new ArgumentException("Supplier credit note reason is required and cannot exceed 1000 characters.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        CreditNoteNumber = number.Trim(); VendorBillId = vendorBillId; PurchaseOrderId = purchaseOrderId;
        SupplierId = supplierId; WarehouseId = warehouseId; CreditDate = creditDate;
        CurrencyCode = currencyCode.Trim().ToUpperInvariant(); SupplierReference = Normalize(supplierReference);
        Reason = reason.Trim(); CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow; Status = SupplierCreditNoteStatus.Draft;
    }

    public void AddLine(SupplierCreditNoteLine line)
    {
        if (Status != SupplierCreditNoteStatus.Draft) throw new InvalidOperationException("Only draft supplier credit notes can be changed.");
        if (Lines.Any(x => x.VendorBillLineId == line.VendorBillLineId)) throw new InvalidOperationException("Each vendor bill line can be credited only once per supplier credit note.");
        Lines.Add(line);
    }

    public void Post(string userId)
    {
        if (Status != SupplierCreditNoteStatus.Draft || Lines.Count == 0 || TotalAmount <= 0) throw new InvalidOperationException("A draft supplier credit note needs at least one positive line before posting.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        Status = SupplierCreditNoteStatus.Posted; PostedByUserId = userId.Trim(); PostedAtUtc = DateTime.UtcNow;
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class SupplierCreditNoteLine
{
    private SupplierCreditNoteLine() { ProductCodeSnapshot = ProductNameSnapshot = UnitNameSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int SupplierCreditNoteId { get; private set; }
    public int VendorBillLineId { get; private set; }
    public int GoodsReceiptLineId { get; private set; }
    public int ProductId { get; private set; }
    public int? TaxInputAccountId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitFactorToBase { get; private set; }
    public decimal StockQuantity { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal CompanyNetAmount { get; private set; }
    public decimal CompanyTaxAmount { get; private set; }
    public decimal CompanyGrossAmount { get; private set; }
    public decimal RemovedInventoryCost { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string UnitNameSnapshot { get; private set; }
    public string? TrackingAllocations { get; private set; }

    public SupplierCreditNoteLine(int vendorBillLineId, int goodsReceiptLineId, int productId, int? taxInputAccountId,
        decimal quantity, decimal unitFactorToBase, decimal netAmount, decimal taxAmount, decimal grossAmount,
        decimal removedInventoryCost, string productCode, string productName, string unitName, string? trackingAllocations,
        decimal companyNetAmount = 0m, decimal companyTaxAmount = 0m, decimal companyGrossAmount = 0m)
    {
        if (vendorBillLineId <= 0 || goodsReceiptLineId <= 0 || productId <= 0) throw new ArgumentException("Supplier credit note line references are required.");
        if (quantity <= 0 || unitFactorToBase <= 0 || netAmount < 0 || taxAmount < 0 || grossAmount <= 0 || removedInventoryCost < 0) throw new ArgumentException("Supplier credit note line values are invalid.");
        if (Round(netAmount + taxAmount) != Round(grossAmount)) throw new ArgumentException("Supplier credit note line total must equal net amount plus tax.");
        if (taxAmount > 0 && !taxInputAccountId.HasValue) throw new ArgumentException("An input tax account is required for a taxed supplier credit note line.");
        if (string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(unitName)) throw new ArgumentException("Supplier credit note line snapshots are required.");
        if (trackingAllocations?.Trim().Length > 2000) throw new ArgumentException("Tracking allocations are too long.");
        VendorBillLineId = vendorBillLineId; GoodsReceiptLineId = goodsReceiptLineId; ProductId = productId;
        TaxInputAccountId = taxInputAccountId; Quantity = quantity; UnitFactorToBase = unitFactorToBase;
        StockQuantity = quantity * unitFactorToBase; NetAmount = Round(netAmount); TaxAmount = Round(taxAmount);
        GrossAmount = Round(grossAmount); RemovedInventoryCost = Round(removedInventoryCost);
        CompanyNetAmount = companyNetAmount == 0 ? NetAmount : Round(companyNetAmount);
        CompanyTaxAmount = companyTaxAmount == 0 ? TaxAmount : Round(companyTaxAmount);
        CompanyGrossAmount = companyGrossAmount == 0 ? GrossAmount : Round(companyGrossAmount);
        ProductCodeSnapshot = productCode.Trim(); ProductNameSnapshot = productName.Trim(); UnitNameSnapshot = unitName.Trim();
        TrackingAllocations = string.IsNullOrWhiteSpace(trackingAllocations) ? null : trackingAllocations.Trim();
    }

    private static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
