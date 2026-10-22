using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class AnalyticsWorkspaceRepository(AppDbContext context) : IAnalyticsWorkspaceRepository
{
    public Task<List<AnalyticsWorkspace>> GetAvailableAsync(string userId) => context.AnalyticsWorkspaces
        .AsNoTracking().Include(x => x.Widgets)
        .Where(x => x.Visibility == AnalyticsWorkspaceVisibility.Shared || x.OwnerUserId == userId)
        .OrderByDescending(x => x.IsDefault).ThenBy(x => x.Name).ToListAsync();
    public Task<List<AnalyticsWorkspace>> GetAvailableForUpdateAsync(string userId) => context.AnalyticsWorkspaces
        .Include(x => x.Widgets).Where(x => x.Visibility == AnalyticsWorkspaceVisibility.Shared || x.OwnerUserId == userId)
        .OrderByDescending(x => x.IsDefault).ThenBy(x => x.Name).ToListAsync();
    public Task<AnalyticsWorkspace?> GetByIdAsync(int id) => context.AnalyticsWorkspaces.Include(x => x.Widgets).SingleOrDefaultAsync(x => x.Id == id);
    public Task AddAsync(AnalyticsWorkspace workspace) => context.AnalyticsWorkspaces.AddAsync(workspace).AsTask();
}
