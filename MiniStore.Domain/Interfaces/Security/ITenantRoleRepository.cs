using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ITenantRoleRepository
{
    Task<IReadOnlyList<TenantRole>> GetAllAsync(int tenantId);
    Task<TenantRole?> GetAsync(int tenantId, int roleId);
    Task<TenantRole?> GetByNormalizedNameAsync(int tenantId, string normalizedName);
    Task<IReadOnlyList<Permission>> GetPermissionCatalogAsync();
    Task<IReadOnlyList<int>> GetPermissionIdsAsync(int roleId);
    Task<int> GetUserCountAsync(int tenantId, int roleId);
    Task<int> GetPermissionCountAsync(int roleId);
    Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetUserRoleNamesAsync(int tenantId, IReadOnlyCollection<string> userIds);
    Task AddAsync(TenantRole role);
    Task ReplacePermissionsAsync(int roleId, IReadOnlyCollection<int> permissionIds);
    void Remove(TenantRole role);
}
