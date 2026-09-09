using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniStore.Application.Permissions;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Authorization;

public class PermissionService : IPermissionService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppDbContext _context;

    public PermissionService(
        UserManager<IdentityUser> userManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<bool> CanAsync(
        ClaimsPrincipal user,
        string permission)
    {
        if (user.Identity == null ||
            !user.Identity.IsAuthenticated)
        {
            return false;
        }

        var currentUser =
            await _userManager.GetUserAsync(user);

        if (currentUser == null)
            return false;

        // Admin has every permission
        if (await _userManager.IsInRoleAsync(
                currentUser,
                "Admin"))
        {
            return true;
        }

        var roleNames =
            await _userManager.GetRolesAsync(
                currentUser);

        if (!roleNames.Any())
            return false;

        var roleIds =
            await _context.Roles
                .Where(x =>
                    roleNames.Contains(x.Name!))
                .Select(x => x.Id)
                .ToListAsync();

        return await _context.RolePermissions
            .Include(x => x.Permission)
            .AnyAsync(x =>
                roleIds.Contains(x.RoleId) &&
                x.Permission.Name == permission);
    }
}