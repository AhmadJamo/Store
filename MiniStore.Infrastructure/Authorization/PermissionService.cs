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

}
