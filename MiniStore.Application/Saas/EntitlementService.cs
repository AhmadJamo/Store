using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Saas;

public class EntitlementService(ISaasRepository repository, ITenantContext tenantContext)
{
    public async Task EnsureSubscriptionAsync()
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("An active company is required.");
        var subscription = await repository.GetSubscriptionAsync(tenantId);
        if (subscription is null || !subscription.AllowsUse(DateTime.UtcNow)) throw new InvalidOperationException("The company subscription is not active.");
    }
    public async Task EnsureLimitAsync(string key)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("An active company is required.");
        var subscription = await repository.GetSubscriptionAsync(tenantId);
        if (subscription is null || !subscription.AllowsUse(DateTime.UtcNow)) throw new InvalidOperationException("The company subscription is not active.");
        var plan = await repository.GetPlanAsync(subscription.PlanId) ?? throw new InvalidOperationException("The subscription plan was not found.");
        var limit = plan.Limits.FirstOrDefault(x => x.Key == key)?.Value;
        if (limit is null) return;
        var usage = key switch { SaasLimitKeys.Users => await repository.CountUsersAsync(tenantId), SaasLimitKeys.Warehouses => await repository.CountWarehousesAsync(), _ => 0 };
        if (usage >= limit.Value) throw new InvalidOperationException($"The {key} limit for the current plan has been reached.");
    }
}
