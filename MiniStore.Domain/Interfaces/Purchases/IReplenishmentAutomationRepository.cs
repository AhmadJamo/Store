using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IReplenishmentAutomationRepository
{
    Task<ReplenishmentAutomationSettings?> GetSettingsAsync();
    Task AddSettingsAsync(ReplenishmentAutomationSettings settings);
    Task<List<ReplenishmentRun>> GetRecentRunsAsync(int count);
    Task<List<int>> GetDueTenantIdsAsync(DateTime nowUtc);
    Task AddRunAsync(ReplenishmentRun run);
    Task SaveChangesAsync();
}
