using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ITenantAuthorizationRepository
{
    Task<bool> HasRoleAsync(int tenantId, string userId, string roleName);
    Task<bool> HasPermissionAsync(int tenantId, string userId, string permission);
    Task<IReadOnlyList<string>> GetGrantedPermissionsAsync(int tenantId, string userId);
    Task<IReadOnlyList<int>> GetRoleIdsAsync(int tenantId, string userId);
    Task AddRoleAsync(TenantUserRole assignment);
    Task SaveChangesAsync();
}
