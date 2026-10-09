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

    public PurchaseQuotationAward(int purchaseSourcingEventId, int supplierQuotationId, string reason, string awardedByUserId)
    {
        if (purchaseSourcingEventId <= 0 || supplierQuotationId <= 0) throw new ArgumentException("Sourcing event and quotation are required.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 1000) throw new ArgumentException("Award reason is required and cannot exceed 1000 characters.");
        if (string.IsNullOrWhiteSpace(awardedByUserId)) throw new ArgumentException("User is required.");
        PurchaseSourcingEventId = purchaseSourcingEventId; SupplierQuotationId = supplierQuotationId; Reason = reason.Trim(); AwardedByUserId = awardedByUserId; AwardedAtUtc = DateTime.UtcNow;
    }
}
