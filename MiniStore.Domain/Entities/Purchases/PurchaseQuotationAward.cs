namespace MiniStore.Domain.Entities;

public sealed class PurchaseQuotationAward
{
    private PurchaseQuotationAward() { AwardedByUserId = Reason = string.Empty; }
    public int Id { get; private set; }
    public int PurchaseSourcingEventId { get; private set; }
    public int SupplierQuotationId { get; private set; }
    public string Reason { get; private set; }
    public string AwardedByUserId { get; private set; }
    public DateTime AwardedAtUtc { get; private set; }
    public List<PurchaseQuotationAwardAllocation> Allocations { get; private set; } = [];

    public PurchaseQuotationAward(int purchaseSourcingEventId, int supplierQuotationId, string reason, string awardedByUserId)
    {
        if (purchaseSourcingEventId <= 0 || supplierQuotationId <= 0) throw new ArgumentException("Sourcing event and quotation are required.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 1000) throw new ArgumentException("Award reason is required and cannot exceed 1000 characters.");
        if (string.IsNullOrWhiteSpace(awardedByUserId)) throw new ArgumentException("User is required.");
        PurchaseSourcingEventId = purchaseSourcingEventId; SupplierQuotationId = supplierQuotationId; Reason = reason.Trim(); AwardedByUserId = awardedByUserId; AwardedAtUtc = DateTime.UtcNow;
    }

    public void AddAllocation(PurchaseQuotationAwardAllocation allocation)
    {
        if (Allocations.Any(x => x.SupplierQuotationLineId == allocation.SupplierQuotationLineId))
            throw new InvalidOperationException("Each supplier quotation line can be awarded only once.");
        Allocations.Add(allocation);
    }
}

public sealed class PurchaseQuotationAwardAllocation
{
    private PurchaseQuotationAwardAllocation()
    {
        CurrencyCodeSnapshot = Reason = ProductCodeSnapshot =
            ProductNameSnapshot = UnitNameSnapshot = string.Empty;
    }

    public int Id { get; private set; }
    public int PurchaseQuotationAwardId { get; private set; }
    public int PurchaseSourcingLineId { get; private set; }
    public int SupplierQuotationId { get; private set; }
    public int SupplierQuotationLineId { get; private set; }
    public int SupplierId { get; private set; }
    public decimal AwardedQuantity { get; private set; }
    public decimal UnitPriceSnapshot { get; private set; }
    public decimal DiscountPercentSnapshot { get; private set; }
    public decimal TaxPercentSnapshot { get; private set; }
    public string CurrencyCodeSnapshot { get; private set; }
    public int LeadTimeDaysSnapshot { get; private set; }
    public string? PaymentTermsSnapshot { get; private set; }
    public string Reason { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string UnitNameSnapshot { get; private set; }
    public decimal NetAmount => decimal.Round(AwardedQuantity * UnitPriceSnapshot *
        (1m - DiscountPercentSnapshot / 100m), 6);
    public decimal TaxAmount => decimal.Round(NetAmount * TaxPercentSnapshot / 100m, 6);
    public decimal GrossAmount => NetAmount + TaxAmount;

    public PurchaseQuotationAwardAllocation(int purchaseSourcingLineId, int supplierQuotationId,
        int supplierQuotationLineId, int supplierId, decimal awardedQuantity, decimal unitPrice,
        decimal discountPercent, decimal taxPercent, string currencyCode, int leadTimeDays,
        string? paymentTerms, string reason, string productCode, string productName, string unitName)
    {
        if (purchaseSourcingLineId <= 0 || supplierQuotationId <= 0 || supplierQuotationLineId <= 0 || supplierId <= 0)
            throw new ArgumentException("Award allocation references are required.");
        if (awardedQuantity <= 0) throw new ArgumentException("Awarded quantity must be greater than zero.");
        if (unitPrice < 0) throw new ArgumentException("Awarded unit price cannot be negative.");
        if (discountPercent is < 0 or > 100 || taxPercent is < 0 or > 100)
            throw new ArgumentException("Award discount and tax percentages must be between 0 and 100.");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3)
            throw new ArgumentException("Currency code must contain exactly three letters.");
        if (leadTimeDays is < 0 or > 3650) throw new ArgumentException("Lead time must be between 0 and 3650 days.");
        if (paymentTerms?.Trim().Length > 500) throw new ArgumentException("Payment terms cannot exceed 500 characters.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 1000)
            throw new ArgumentException("Each award allocation requires a reason up to 1000 characters.");
        if (string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(productName) ||
            string.IsNullOrWhiteSpace(unitName)) throw new ArgumentException("Award product snapshots are required.");
        PurchaseSourcingLineId = purchaseSourcingLineId;
        SupplierQuotationId = supplierQuotationId;
        SupplierQuotationLineId = supplierQuotationLineId;
        SupplierId = supplierId;
        AwardedQuantity = awardedQuantity;
        UnitPriceSnapshot = unitPrice;
        DiscountPercentSnapshot = discountPercent;
        TaxPercentSnapshot = taxPercent;
        CurrencyCodeSnapshot = currencyCode.Trim().ToUpperInvariant();
        LeadTimeDaysSnapshot = leadTimeDays;
        PaymentTermsSnapshot = string.IsNullOrWhiteSpace(paymentTerms) ? null : paymentTerms.Trim();
        Reason = reason.Trim();
        ProductCodeSnapshot = productCode.Trim();
        ProductNameSnapshot = productName.Trim();
        UnitNameSnapshot = unitName.Trim();
    }
}
