using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class SaasRepository(AppDbContext context) : ISaasRepository
{
    public async Task<IReadOnlyList<Plan>> GetPublicPlansAsync() => await context.Plans.AsSplitQuery().Include(x => x.Features).Include(x => x.Limits).Where(x => x.IsActive && x.IsPublic).OrderBy(x => x.SortOrder).ToListAsync();
    public async Task<IReadOnlyList<Plan>> GetPlansAsync() => await context.Plans.AsSplitQuery().Include(x => x.Features).Include(x => x.Limits).OrderBy(x => x.SortOrder).ToListAsync();
    public Task<Plan?> GetPlanAsync(int id) => context.Plans.AsSplitQuery().Include(x => x.Features).Include(x => x.Limits).FirstOrDefaultAsync(x => x.Id == id);
    public Task<TenantSubscription?> GetSubscriptionAsync(int tenantId) => context.TenantSubscriptions.FirstOrDefaultAsync(x => x.TenantId == tenantId);
    public Task<Tenant?> GetTenantAsync(int tenantId) => context.Tenants.FirstOrDefaultAsync(x => x.Id == tenantId);
    public async Task<IReadOnlyList<Tenant>> GetTenantsAsync() => await context.Tenants.OrderBy(x => x.Name).ToListAsync();
    public Task<PlatformOperator?> GetOperatorAsync(string userId) => context.PlatformOperators.FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);
    public async Task<IReadOnlyList<PromotionCode>> GetPromotionCodesAsync() => await context.PromotionCodes.OrderByDescending(x => x.EndsAt).ToListAsync();
    public Task<int> CountWarehousesAsync() => context.Warehouses.CountAsync();
    public Task<int> CountUsersAsync(int tenantId) => context.TenantMemberships.CountAsync(x => x.TenantId == tenantId && x.IsActive);
    public Task AddPlanAsync(Plan plan) => context.Plans.AddAsync(plan).AsTask();
    public Task AddPromotionCodeAsync(PromotionCode code) => context.PromotionCodes.AddAsync(code).AsTask();
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
