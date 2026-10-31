using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Saas;

public record TenantSubscriptionSummaryDto(
    int PlanId,
    string PlanNameEnglish,
    string PlanNameArabic,
    SubscriptionStatus Status,
    BillingCycle BillingCycle,
    DateTime CurrentPeriodStart,
    DateTime CurrentPeriodEnd,
    DateTime? GracePeriodEnd,
    bool CancelAtPeriodEnd,
    bool AllowsUse);

public record TenantSubscriptionPageDto(
    TenantSubscriptionSummaryDto? CurrentSubscription,
    IReadOnlyList<PlanCardDto> Plans);

public class TenantSubscriptionPageService(
    ISaasRepository repository,
    ITenantContext tenantContext,
    PlatformSaasService plans)
{
    public async Task<TenantSubscriptionPageDto> GetAsync()
    {
        var tenantId = tenantContext.TenantId
            ?? throw new InvalidOperationException("An active company is required.");
        var subscription = await repository.GetSubscriptionAsync(tenantId);
        TenantSubscriptionSummaryDto? current = null;

        if (subscription is not null)
        {
            var plan = await repository.GetPlanAsync(subscription.PlanId)
                ?? throw new InvalidOperationException("Subscription plan was not found.");
            current = new TenantSubscriptionSummaryDto(
                subscription.PlanId,
                plan.NameEnglish,
                plan.NameArabic,
                subscription.Status,
                subscription.BillingCycle,
                subscription.CurrentPeriodStart,
                subscription.CurrentPeriodEnd,
                subscription.GracePeriodEnd,
                subscription.CancelAtPeriodEnd,
                subscription.AllowsUse(DateTime.UtcNow));
        }

        return new TenantSubscriptionPageDto(current, await plans.GetPublicPlansAsync());
    }
}
