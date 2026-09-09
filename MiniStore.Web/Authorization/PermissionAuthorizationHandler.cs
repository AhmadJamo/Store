using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Web.Authorization;

public class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppDbContext _context;

    public PermissionAuthorizationHandler(
        UserManager<IdentityUser> userManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
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

        // Admin has all permissions
        if (await _userManager.IsInRoleAsync(
                user,
                "Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        var roleIds =
            await _userManager.GetRolesAsync(user);

        if (!roleIds.Any())
            return;

        var roleEntities =
            await _context.Roles
                .Where(x => roleIds.Contains(x.Name!))
                .Select(x => x.Id)
                .ToListAsync();

        var hasPermission =
            await _context.RolePermissions
                .Include(x => x.Permission)
                .AnyAsync(x =>
                    roleEntities.Contains(x.RoleId) &&
                    x.Permission.Name ==
                        requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}