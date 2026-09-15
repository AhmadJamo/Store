using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ITenantMembershipRepository
{
    Task<TenantMembership?> GetDefaultActiveAsync(string userId);
    Task<TenantMembership?> GetActiveByCompanyCodeAsync(string userId, string companyCode);
    Task<IReadOnlyList<string>> GetActiveUserIdsAsync(int tenantId);
    Task<bool> ExistsAsync(int tenantId, string userId);
    Task AddAsync(TenantMembership membership);
    Task SaveChangesAsync();
}
