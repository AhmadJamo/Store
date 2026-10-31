using Microsoft.AspNetCore.Identity;
using MiniStore.Application.Permissions;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Infrastructure.Authorization;

public class PermissionService : IPermissionService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITenantContext _tenantContext;
    private readonly ITenantAuthorizationRepository _authorization;

    public PermissionService(
        UserManager<IdentityUser> userManager,
        ITenantContext tenantContext,
        ITenantAuthorizationRepository authorization)
    {
        _userManager = userManager;
        _tenantContext = tenantContext;
        _authorization = authorization;
    }

    public async Task<bool> CanAsync(
        string userId,
        string permission)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return false;

        var currentUser =
            await _userManager.FindByIdAsync(userId);

        if (currentUser == null)
            return false;

        if (_tenantContext.TenantId is not int tenantId)
            return false;

        // Admin has every permission inside the active company only.
        if (await _authorization.HasRoleAsync(tenantId, currentUser.Id, "Admin"))
        {
            return true;
        }

        if (AdministrationPermissions.RequiresAdmin(permission))
            return false;

        return await _authorization.HasPermissionAsync(tenantId, currentUser.Id, permission);
    }

    public async Task<IReadOnlyList<string>> GetGrantedAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId) || _tenantContext.TenantId is not int tenantId)
            return [];

        var currentUser = await _userManager.FindByIdAsync(userId);
        if (currentUser is null)
            return [];

        // Keep the Angular/session permission projection aligned with CanAsync and
        // PermissionAuthorizationHandler: the protected company Admin role grants
        // every code-owned permission inside the active company.
        if (await _authorization.HasRoleAsync(tenantId, currentUser.Id, "Admin"))
        {
            return PermissionDefinitions.All
                .Select(x => x.Name)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToList();
        }

        return await _authorization.GetGrantedPermissionsAsync(tenantId, currentUser.Id);
    }

}
