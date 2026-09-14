namespace MiniStore.Domain.Entities;

public class PromotionCode
{
    public int Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public decimal DiscountPercentage { get; private set; }
    public DateTime StartsAt { get; private set; }
    public DateTime EndsAt { get; private set; }
    public int? MaximumRedemptions { get; private set; }
    public int RedemptionCount { get; private set; }
    public int? PlanId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public byte[] RowVersion { get; private set; } = [];
    private PromotionCode() { }
    public PromotionCode(string code, decimal percentage, DateTime startsAt, DateTime endsAt, int? maximumRedemptions, int? planId)
    {
        code = code.Trim().ToUpperInvariant();
        if (!System.Text.RegularExpressions.Regex.IsMatch(code, "^[A-Z0-9][A-Z0-9-]{2,39}$")) throw new ArgumentException("Invalid promotion code.");
        if (percentage <= 0 || percentage > 100) throw new ArgumentException("Discount percentage must be between 0 and 100.");
        if (endsAt <= startsAt || maximumRedemptions <= 0) throw new ArgumentException("Invalid promotion validity or redemption limit.");
        Code = code; DiscountPercentage = percentage; StartsAt = startsAt; EndsAt = endsAt; MaximumRedemptions = maximumRedemptions; PlanId = planId;
    }
    public bool CanRedeem(DateTime utcNow, int planId) => IsActive && utcNow >= StartsAt && utcNow < EndsAt && (PlanId is null || PlanId == planId) && (MaximumRedemptions is null || RedemptionCount < MaximumRedemptions);
    public void Redeem(DateTime utcNow, int planId) { if (!CanRedeem(utcNow, planId)) throw new InvalidOperationException("Promotion code is expired, unavailable or not valid for this plan."); RedemptionCount++; }
    public void SetActive(bool active) => IsActive = active;
}

public class PromotionRedemption
{
    public int Id { get; private set; }
    public int PromotionCodeId { get; private set; }
    public int TenantId { get; private set; }
    public int SubscriptionId { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public DateTime RedeemedAt { get; private set; } = DateTime.UtcNow;
    private PromotionRedemption() { }
    public PromotionRedemption(int promotionCodeId, int tenantId, int subscriptionId, decimal percentage) { PromotionCodeId = promotionCodeId; TenantId = tenantId; SubscriptionId = subscriptionId; DiscountPercentage = percentage; }
}
