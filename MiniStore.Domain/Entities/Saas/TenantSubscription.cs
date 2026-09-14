namespace MiniStore.Domain.Entities;

public class TenantSubscription
{
    public int Id { get; private set; }
    public int TenantId { get; private set; }
    public int PlanId { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public BillingCycle BillingCycle { get; private set; }
    public DateTime CurrentPeriodStart { get; private set; }
    public DateTime CurrentPeriodEnd { get; private set; }
    public DateTime? GracePeriodEnd { get; private set; }
    public bool CancelAtPeriodEnd { get; private set; }
    public string? ExternalCustomerId { get; private set; }
    public string? ExternalSubscriptionId { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    private TenantSubscription() { }
    public TenantSubscription(int tenantId, int planId, DateTime trialEnd)
    { if (tenantId <= 0 || planId <= 0 || trialEnd <= DateTime.UtcNow) throw new ArgumentException("Invalid trial subscription."); TenantId = tenantId; PlanId = planId; Status = SubscriptionStatus.Trial; BillingCycle = BillingCycle.Monthly; CurrentPeriodStart = DateTime.UtcNow; CurrentPeriodEnd = trialEnd; }
    public void ChangePlan(int planId, BillingCycle cycle, DateTime start, DateTime end) { if (planId <= 0 || end <= start) throw new ArgumentException("Invalid subscription period."); PlanId = planId; BillingCycle = cycle; CurrentPeriodStart = start; CurrentPeriodEnd = end; Status = SubscriptionStatus.Active; CancelAtPeriodEnd = false; }
    public void SetStatus(SubscriptionStatus status, DateTime? gracePeriodEnd = null) { if (!Enum.IsDefined(status)) throw new ArgumentException("Invalid subscription status."); Status = status; GracePeriodEnd = gracePeriodEnd; }
    public bool AllowsUse(DateTime utcNow) => Status is SubscriptionStatus.Active or SubscriptionStatus.Trial || (Status is SubscriptionStatus.PastDue or SubscriptionStatus.GracePeriod && GracePeriodEnd >= utcNow);
}

public class PlatformOperator
{
    public string UserId { get; private set; } = string.Empty;
    public PlatformOperatorRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    private PlatformOperator() { }
    public PlatformOperator(string userId, PlatformOperatorRole role) { if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required."); UserId = userId; Role = role; }
}
