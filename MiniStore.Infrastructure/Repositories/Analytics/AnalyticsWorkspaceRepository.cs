using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class AnalyticsWorkspaceRepository(AppDbContext context) : IAnalyticsWorkspaceRepository
{
    public Task<List<AnalyticsWorkspace>> GetAvailableAsync(string userId, IReadOnlyCollection<int> roleIds, DateTime nowUtc) => context.AnalyticsWorkspaces
        .AsNoTracking().Include(x => x.Widgets).Include(x => x.Delegations)
        .Where(x => x.Visibility == AnalyticsWorkspaceVisibility.Shared || x.OwnerUserId == userId ||
            x.Delegations.Any(d => d.RevokedAtUtc == null && d.StartsAtUtc <= nowUtc &&
                (d.ExpiresAtUtc == null || d.ExpiresAtUtc > nowUtc) &&
                (d.TargetUserId == userId || d.TargetRoleId.HasValue && roleIds.Contains(d.TargetRoleId.Value))))
        .OrderByDescending(x => x.IsDefault).ThenBy(x => x.Name).ToListAsync();
    public Task<List<AnalyticsWorkspace>> GetAvailableForUpdateAsync(string userId) => context.AnalyticsWorkspaces
        .Include(x => x.Widgets).Where(x => x.Visibility == AnalyticsWorkspaceVisibility.Shared || x.OwnerUserId == userId)
        .OrderByDescending(x => x.IsDefault).ThenBy(x => x.Name).ToListAsync();
    public Task<AnalyticsWorkspace?> GetByIdAsync(int id) => context.AnalyticsWorkspaces.AsNoTracking().Include(x => x.Widgets).Include(x => x.Delegations).SingleOrDefaultAsync(x => x.Id == id);
    public Task<AnalyticsWorkspace?> GetForUpdateAsync(int id) => context.AnalyticsWorkspaces.Include(x => x.Widgets).Include(x => x.Delegations).SingleOrDefaultAsync(x => x.Id == id);
    public Task AddAsync(AnalyticsWorkspace workspace) => context.AnalyticsWorkspaces.AddAsync(workspace).AsTask();
}
