using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IAnalyticsWorkspaceRepository
{
    Task<List<AnalyticsWorkspace>> GetAvailableAsync(string userId, IReadOnlyCollection<int> roleIds, DateTime nowUtc);
    Task<List<AnalyticsWorkspace>> GetAvailableForUpdateAsync(string userId);
    Task<AnalyticsWorkspace?> GetByIdAsync(int id);
    Task<AnalyticsWorkspace?> GetForUpdateAsync(int id);
    Task AddAsync(AnalyticsWorkspace workspace);
}
