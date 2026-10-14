using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class ReplenishmentAutomationRepository(AppDbContext context) : IReplenishmentAutomationRepository
{
    public Task<ReplenishmentAutomationSettings?> GetSettingsAsync() =>
        context.ReplenishmentAutomationSettings.SingleOrDefaultAsync(x => x.SingletonKey == 1);
    public Task AddSettingsAsync(ReplenishmentAutomationSettings settings) =>
        context.ReplenishmentAutomationSettings.AddAsync(settings).AsTask();
    public Task<List<ReplenishmentRun>> GetRecentRunsAsync(int count) => context.ReplenishmentRuns
        .Include(x => x.Suggestions).OrderByDescending(x => x.StartedAtUtc).Take(count).ToListAsync();
    public Task<List<int>> GetDueTenantIdsAsync(DateTime nowUtc) => context.ReplenishmentAutomationSettings
        .IgnoreQueryFilters().Where(x => x.IsEnabled && x.NextRunAtUtc <= nowUtc)
        .Select(x => EF.Property<int>(x, "TenantId")).Distinct().ToListAsync();
    public Task AddRunAsync(ReplenishmentRun run) => context.ReplenishmentRuns.AddAsync(run).AsTask();
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
