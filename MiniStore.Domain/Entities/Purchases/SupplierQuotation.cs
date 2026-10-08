namespace MiniStore.Domain.Entities;

public enum SupplierQuotationStatus { Draft = 1, Submitted = 2, Withdrawn = 3 }

public sealed class SupplierQuotation
{
    private SupplierQuotation() { QuotationNumber = CurrencyCode = CreatedByUserId = string.Empty; }

    public int Id { get; private set; }
    public string QuotationNumber { get; private set; }
    public int PurchaseSourcingEventId { get; private set; }
    public int SupplierId { get; private set; }
    public string? SupplierReference { get; private set; }
    public DateOnly QuotationDate { get; private set; }
    public DateOnly ValidUntilDate { get; private set; }
    public int LeadTimeDays { get; private set; }
    public string CurrencyCode { get; private set; }
    public string? PaymentTermsSnapshot { get; private set; }
    public string? Notes { get; private set; }
    public SupplierQuotationStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? SubmittedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<SupplierQuotationLine> Lines { get; private set; } = [];

    public SupplierQuotation(string quotationNumber, int purchaseSourcingEventId, int supplierId,
        string? supplierReference, DateOnly quotationDate, DateOnly validUntilDate, int leadTimeDays,
        string currencyCode, string? paymentTermsSnapshot, string? notes, string createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(quotationNumber) || quotationNumber.Trim().Length > 50)
            throw new ArgumentException("Quotation number is required and cannot exceed 50 characters.");
        if (purchaseSourcingEventId <= 0 || supplierId <= 0)
            throw new ArgumentException("Sourcing event and supplier are required.");
        if (validUntilDate < quotationDate) throw new ArgumentException("Quotation validity cannot end before its date.");
        if (leadTimeDays is < 0 or > 3650) throw new ArgumentException("Lead time must be between 0 and 3650 days.");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3 ||
            !currencyCode.Trim().All(char.IsLetter)) throw new ArgumentException("Currency code must contain exactly three letters.");
        if (supplierReference?.Trim().Length > 100) throw new ArgumentException("Supplier quotation reference cannot exceed 100 characters.");
        if (paymentTermsSnapshot?.Trim().Length > 500) throw new ArgumentException("Payment terms cannot exceed 500 characters.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Quotation notes cannot exceed 1000 characters.");
        if (string.IsNullOrWhiteSpace(createdByUserId)) throw new ArgumentException("User is required.");
        QuotationNumber = quotationNumber.Trim();
        PurchaseSourcingEventId = purchaseSourcingEventId;
        SupplierId = supplierId;
        SupplierReference = Normalize(supplierReference);
        QuotationDate = quotationDate;
        ValidUntilDate = validUntilDate;
        LeadTimeDays = leadTimeDays;
        CurrencyCode = currencyCode.Trim().ToUpperInvariant();
        PaymentTermsSnapshot = Normalize(paymentTermsSnapshot);
        Notes = Normalize(notes);
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = DateTime.UtcNow;
        Status = SupplierQuotationStatus.Draft;
    }

    public void AddLine(SupplierQuotationLine line)
    {
        EnsureDraft();
        if (Lines.Any(x => x.PurchaseSourcingLineId == line.PurchaseSourcingLineId))
            throw new InvalidOperationException("Each sourcing line can be quoted only once.");
        Lines.Add(line);
    }

    public void Submit()
    {
        EnsureDraft();
        if (Lines.Count == 0) throw new InvalidOperationException("A quotation must contain at least one line.");
        Status = SupplierQuotationStatus.Submitted;
        SubmittedAtUtc = DateTime.UtcNow;
    }

    public decimal NetAmount => Lines.Sum(x => x.NetAmount);
    public decimal TaxAmount => Lines.Sum(x => x.TaxAmount);
    public decimal GrossAmount => NetAmount + TaxAmount;

    private void EnsureDraft()
    {
        if (Status != SupplierQuotationStatus.Draft)
            throw new InvalidOperationException("Only draft quotations can be modified.");
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class SupplierQuotationLine
{
    private SupplierQuotationLine() { ProductCodeSnapshot = ProductNameSnapshot = UnitNameSnapshot = string.Empty; }

    public int Id { get; private set; }
    public int SupplierQuotationId { get; private set; }
    public int PurchaseSourcingLineId { get; private set; }
    public decimal QuotedQuantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public decimal TaxPercent { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string UnitNameSnapshot { get; private set; }
    public decimal NetAmount => decimal.Round(QuotedQuantity * UnitPrice * (1m - DiscountPercent / 100m), 6);
    public decimal TaxAmount => decimal.Round(NetAmount * TaxPercent / 100m, 6);
    public decimal GrossAmount => NetAmount + TaxAmount;

    public SupplierQuotationLine(int purchaseSourcingLineId, decimal quotedQuantity, decimal unitPrice,
        decimal discountPercent, decimal taxPercent, string productCodeSnapshot, string productNameSnapshot,
        string unitNameSnapshot)
    {
        if (purchaseSourcingLineId <= 0) throw new ArgumentException("Sourcing line is required.");
        if (quotedQuantity <= 0) throw new ArgumentException("Quoted quantity must be greater than zero.");
        if (unitPrice < 0) throw new ArgumentException("Quoted unit price cannot be negative.");
        if (discountPercent is < 0 or > 100 || taxPercent is < 0 or > 100)
            throw new ArgumentException("Quotation discount and tax percentages must be between 0 and 100.");
        if (string.IsNullOrWhiteSpace(productCodeSnapshot) || string.IsNullOrWhiteSpace(productNameSnapshot) ||
            string.IsNullOrWhiteSpace(unitNameSnapshot)) throw new ArgumentException("Quotation line snapshots are required.");
        PurchaseSourcingLineId = purchaseSourcingLineId;
        QuotedQuantity = quotedQuantity;
        UnitPrice = unitPrice;
        DiscountPercent = discountPercent;
        TaxPercent = taxPercent;
        ProductCodeSnapshot = productCodeSnapshot.Trim();
        ProductNameSnapshot = productNameSnapshot.Trim();
        UnitNameSnapshot = unitNameSnapshot.Trim();
    }
}
