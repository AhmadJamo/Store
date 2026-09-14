using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ITenantAuthorizationRepository
{
    Task<bool> HasRoleAsync(int tenantId, string userId, string roleName);
    Task<bool> HasPermissionAsync(int tenantId, string userId, string permission);
    Task AddRoleAsync(TenantUserRole assignment);
    Task SaveChangesAsync();
}
