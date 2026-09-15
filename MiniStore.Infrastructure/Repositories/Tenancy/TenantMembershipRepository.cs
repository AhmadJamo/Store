using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class TenantMembershipRepository(AppDbContext context)
    : ITenantMembershipRepository
{
    public Task<TenantMembership?> GetDefaultActiveAsync(string userId) =>
        context.TenantMemberships
            .Where(x => x.UserId == userId && x.IsActive)
            .Join(
                context.Tenants.Where(x => x.IsActive),
                membership => membership.TenantId,
                tenant => tenant.Id,
                (membership, _) => membership)
            .OrderByDescending(x => x.IsOwner)
            .ThenBy(x => x.TenantId)
            .FirstOrDefaultAsync();

    public Task<TenantMembership?> GetActiveByCompanyCodeAsync(string userId, string companyCode)
    {
        var normalizedCode = companyCode.Trim().ToLowerInvariant();
        return context.TenantMemberships
            .Where(x => x.UserId == userId && x.IsActive)
            .Join(
                context.Tenants.Where(x => x.IsActive && x.Slug == normalizedCode),
                membership => membership.TenantId,
                tenant => tenant.Id,
                (membership, _) => membership)
            .SingleOrDefaultAsync();
    }

    public async Task<IReadOnlyList<string>> GetActiveUserIdsAsync(int tenantId) =>
        await context.TenantMemberships
            .Where(x => x.TenantId == tenantId && x.IsActive)
            .Select(x => x.UserId)
            .ToListAsync();

    public Task<bool> ExistsAsync(int tenantId, string userId) =>
        context.TenantMemberships
            .Where(x => x.TenantId == tenantId && x.UserId == userId && x.IsActive)
            .Join(
                context.Tenants.Where(x => x.IsActive),
                membership => membership.TenantId,
                tenant => tenant.Id,
                (membership, _) => membership)
            .AnyAsync();

    public Task AddAsync(TenantMembership membership) =>
        context.TenantMemberships.AddAsync(membership).AsTask();

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
