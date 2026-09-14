namespace MiniStore.Domain.Entities;

public enum CheckoutStatus { Pending = 1, Paid = 2, Cancelled = 3, Expired = 4 }

public class BillingCheckoutSession
{
    public Guid Id { get; private set; }
    public int TenantId { get; private set; }
    public int PlanId { get; private set; }
    public BillingCycle BillingCycle { get; private set; }
    public int? PromotionCodeId { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal Total { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public CheckoutStatus Status { get; private set; } = CheckoutStatus.Pending;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? ProviderReference { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    private BillingCheckoutSession() { }
    public BillingCheckoutSession(int tenantId, int planId, BillingCycle cycle, int? promotionCodeId, decimal subtotal, decimal discount, string currency)
    {
        var normalizedCurrency = currency?.Trim().ToUpperInvariant() ?? string.Empty;
        if (tenantId <= 0 || planId <= 0 || !Enum.IsDefined(cycle) || subtotal < 0 || discount < 0 || discount > subtotal ||
            !System.Text.RegularExpressions.Regex.IsMatch(normalizedCurrency, "^[A-Z]{3}$"))
            throw new ArgumentException("Invalid checkout.");
        Id = Guid.NewGuid(); TenantId = tenantId; PlanId = planId; BillingCycle = cycle; PromotionCodeId = promotionCodeId;
        Subtotal = subtotal; DiscountAmount = discount; Total = subtotal - discount; Currency = normalizedCurrency;
        ExpiresAt = DateTime.UtcNow.AddMinutes(30);
    }
    public void MarkPaid(string reference) { if (Status != CheckoutStatus.Pending || ExpiresAt <= DateTime.UtcNow) throw new InvalidOperationException("Checkout is no longer payable."); if (string.IsNullOrWhiteSpace(reference)) throw new ArgumentException("Payment reference is required."); Status = CheckoutStatus.Paid; ProviderReference = reference.Trim(); PaidAt = DateTime.UtcNow; }
}
