using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IAnalyticsWorkspaceRepository
{
    Task<List<AnalyticsWorkspace>> GetAvailableAsync(string userId);
    Task<List<AnalyticsWorkspace>> GetAvailableForUpdateAsync(string userId);
    Task<AnalyticsWorkspace?> GetByIdAsync(int id);
    Task AddAsync(AnalyticsWorkspace workspace);
}
