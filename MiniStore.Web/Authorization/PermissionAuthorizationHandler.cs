using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Web.Authorization;

public class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITenantContext _tenantContext;
    private readonly ITenantAuthorizationRepository _authorization;

    public PermissionAuthorizationHandler(
        UserManager<IdentityUser> userManager,
        ITenantContext tenantContext,
        ITenantAuthorizationRepository authorization)
    {
        _userManager = userManager;
        _tenantContext = tenantContext;
        _authorization = authorization;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity == null ||
            !context.User.Identity.IsAuthenticated)
        {
            return;
        }

        var user =
            await _userManager.GetUserAsync(
                context.User);

        if (user == null)
            return;

        if (_tenantContext.TenantId is not int tenantId)
            return;

        if (await _authorization.HasRoleAsync(tenantId, user.Id, "Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        if (MiniStore.Application.Permissions.AdministrationPermissions.RequiresAdmin(requirement.Permission))
            return;

        var hasPermission = await _authorization.HasPermissionAsync(tenantId, user.Id, requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }

}
