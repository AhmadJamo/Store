using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class BranchAccessRepository(AppDbContext context) : IBranchAccessRepository
{
    public async Task<IReadOnlyList<int>> GetGrantedBranchIdsAsync(int tenantId, string userId) =>
        await context.TenantUserBranchGrants
            .Where(x => x.TenantId == tenantId && x.UserId == userId)
            .Select(x => x.BranchId).OrderBy(x => x).ToListAsync();

    public async Task<IReadOnlyList<TenantUserBranchGrant>> GetAllGrantsAsync(int tenantId) =>
        await context.TenantUserBranchGrants.Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.UserId).ThenBy(x => x.BranchId).ToListAsync();

    public async Task<IReadOnlyList<BranchAccessUser>> GetActiveUsersAsync(int tenantId) =>
        await context.TenantMemberships.Where(x => x.TenantId == tenantId && x.IsActive)
            .Join(context.Users, membership => membership.UserId, user => user.Id,
                (_, user) => new BranchAccessUser(
                    user.Id, user.UserName ?? user.Email ?? user.Id, user.Email))
            .OrderBy(x => x.DisplayName).ToListAsync();

    public async Task ReplaceUserGrantsAsync(
        int tenantId, string userId, IReadOnlyCollection<int> branchIds, string grantedByUserId)
    {
        var isMember = await context.TenantMemberships.AnyAsync(x =>
            x.TenantId == tenantId && x.UserId == userId && x.IsActive);
        if (!isMember) throw new InvalidOperationException("The selected user is unavailable.");

        var requested = branchIds.Distinct().ToHashSet();
        var validCount = await context.Branches.CountAsync(x => requested.Contains(x.Id));
        if (validCount != requested.Count) throw new InvalidOperationException("The selected branch is unavailable.");

        var current = await context.TenantUserBranchGrants
            .Where(x => x.TenantId == tenantId && x.UserId == userId).ToListAsync();
        context.TenantUserBranchGrants.RemoveRange(current.Where(x => !requested.Contains(x.BranchId)));
        var currentIds = current.Select(x => x.BranchId).ToHashSet();
        await context.TenantUserBranchGrants.AddRangeAsync(requested
            .Where(branchId => !currentIds.Contains(branchId))
            .Select(branchId => new TenantUserBranchGrant(tenantId, userId, branchId, grantedByUserId)));
    }
}
