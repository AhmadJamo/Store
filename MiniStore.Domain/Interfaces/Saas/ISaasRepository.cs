using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISaasRepository
{
    Task<IReadOnlyList<Plan>> GetPublicPlansAsync();
    Task<IReadOnlyList<Plan>> GetPlansAsync();
    Task<Plan?> GetPlanAsync(int id);
    Task<TenantSubscription?> GetSubscriptionAsync(int tenantId);
    Task<Tenant?> GetTenantAsync(int tenantId);
    Task<IReadOnlyList<Tenant>> GetTenantsAsync();
    Task<PlatformOperator?> GetOperatorAsync(string userId);
    Task<IReadOnlyList<PromotionCode>> GetPromotionCodesAsync();
    Task<int> CountWarehousesAsync();
    Task<int> CountUsersAsync(int tenantId);
    Task AddPlanAsync(Plan plan);
    Task AddPromotionCodeAsync(PromotionCode code);
    Task SaveChangesAsync();
}
