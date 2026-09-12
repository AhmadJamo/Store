using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

[Authorize]
public class RolesController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppDbContext _context;


public RolesController(
    RoleManager<IdentityRole> roleManager,
    UserManager<IdentityUser> userManager,
    AppDbContext context)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
    }

    // GET: /Roles
    [HttpGet]
    [PermissionAuthorize("Roles.View")]
    public async Task<IActionResult> Index()
    {
        var roles =
            await _roleManager.Roles
                .OrderBy(x => x.Name)
                .ToListAsync();

        var roleUsers =
            new Dictionary<string, int>();

        foreach (var role in roles)
        {
            var users =
                await _userManager.GetUsersInRoleAsync(
                    role.Name!);

            roleUsers[role.Id] =
                users.Count;
        }

        var rolePermissionCounts =
            await _context.RolePermissions
                .GroupBy(x => x.RoleId)
                .Select(x => new
                {
                    RoleId = x.Key,
                    Count = x.Count()
                })
                .ToDictionaryAsync(
                    x => x.RoleId,
                    x => x.Count);

        ViewBag.RoleUsers =
            roleUsers;

        ViewBag.RolePermissionCounts =
            rolePermissionCounts;

        return View(roles);
    }

    // GET: /Roles/Create
    [HttpGet]
    [PermissionAuthorize("Roles.Create")]
    public async Task<IActionResult> Create()
    {
        var permissions =
            await GetPermissionsAsync();

        return View(permissions);
    }

    // POST: /Roles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Roles.Create")]
    public async Task<IActionResult> Create(
        string name,
        int[]? selectedPermissions)
    {
        name =
            name?.Trim()
            ?? string.Empty;

        selectedPermissions ??= [];

        if (string.IsNullOrWhiteSpace(name))
        {
            ViewBag.Error =
                "Role name is required.";

            return View(
                await GetPermissionsAsync());
        }

        if (await _roleManager.RoleExistsAsync(name))
        {
            ViewBag.Error =
                "Role already exists.";

            return View(
                await GetPermissionsAsync());
        }

        var role =
            new IdentityRole(name);

        var result =
            await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            ViewBag.Error =
                string.Join(
                    ", ",
                    result.Errors.Select(
                        x => x.Description));

            return View(
                await GetPermissionsAsync());
        }

        var permissionIds =
            selectedPermissions
                .Distinct()
                .ToList();

        if (permissionIds.Count > 0)
        {
            var validPermissionIds =
                await _context.Permissions
                    .Where(x =>
                        permissionIds.Contains(x.Id))
                    .Select(x => x.Id)
                    .ToListAsync();

            foreach (var permissionId in validPermissionIds)
            {
                _context.RolePermissions.Add(
                    new RolePermission(
                        role.Id,
                        permissionId));
            }

            await _context.SaveChangesAsync();
        }

        TempData["Success"] =
            "Role created successfully.";

        return RedirectToAction(
            nameof(Index));
    }

    // GET: /Roles/Edit/{id}
    [HttpGet]
    [PermissionAuthorize("Roles.Edit")]
    public async Task<IActionResult> Edit(
        string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var role =
            await _roleManager.FindByIdAsync(id);

        if (role == null)
            return NotFound();

        // Admin is a protected system role.
        if (IsProtectedRole(role))
        {
            TempData["Error"] =
                "The Admin role is a protected system role.";

            return RedirectToAction(
                nameof(Index));
        }

        var permissions =
            await GetPermissionsAsync();

        var selectedPermissions =
            await _context.RolePermissions
                .Where(x =>
                    x.RoleId == id)
                .Select(x =>
                    x.PermissionId)
                .ToListAsync();

        ViewBag.RoleId =
            id;

        ViewBag.RoleName =
            role.Name;

        ViewBag.SelectedPermissions =
            selectedPermissions;

        return View(permissions);
    }

    // POST: /Roles/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Roles.Edit")]
    public async Task<IActionResult> Edit(
        string id,
        string name,
        int[]? selectedPermissions)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        selectedPermissions ??= [];

        var role =
            await _roleManager.FindByIdAsync(id);

        if (role == null)
            return NotFound();

        // Admin is a protected system role.
        if (IsProtectedRole(role))
        {
            TempData["Error"] =
                "The Admin role is a protected system role.";

            return RedirectToAction(
                nameof(Index));
        }

        name =
            name?.Trim()
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            return await ReturnEditWithError(
                id,
                name,
                selectedPermissions,
                "Role name is required.");
        }

        var existingRole =
            await _roleManager.FindByNameAsync(name);

        if (existingRole != null &&
            existingRole.Id != role.Id)
        {
            return await ReturnEditWithError(
                id,
                name,
                selectedPermissions,
                "Role name already exists.");
        }

        role.Name =
            name;

        var updateResult =
            await _roleManager.UpdateAsync(role);

        if (!updateResult.Succeeded)
        {
            var error =
                string.Join(
                    ", ",
                    updateResult.Errors.Select(
                        x => x.Description));

            return await ReturnEditWithError(
                id,
                name,
                selectedPermissions,
                error);
        }

        var oldPermissions =
            await _context.RolePermissions
                .Where(x =>
                    x.RoleId == id)
                .ToListAsync();

        _context.RolePermissions.RemoveRange(
            oldPermissions);

        var permissionIds =
            selectedPermissions
                .Distinct()
                .ToList();

        if (permissionIds.Count > 0)
        {
            var validPermissionIds =
                await _context.Permissions
                    .Where(x =>
                        permissionIds.Contains(x.Id))
                    .Select(x => x.Id)
                    .ToListAsync();

            foreach (var permissionId in validPermissionIds)
            {
                _context.RolePermissions.Add(
                    new RolePermission(
                        id,
                        permissionId));
            }
        }

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "Role updated successfully.";

        return RedirectToAction(
            nameof(Index));
    }

    // GET: /Roles/Delete/{id}
    [HttpGet]
    [PermissionAuthorize("Roles.Delete")]
    public async Task<IActionResult> Delete(
        string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var role =
            await _roleManager.FindByIdAsync(id);

        if (role == null)
            return NotFound();

        // Admin is a protected system role.
        if (IsProtectedRole(role))
        {
            TempData["Error"] =
                "The Admin role is a protected system role.";

            return RedirectToAction(
                nameof(Index));
        }

        var users =
            await _userManager.GetUsersInRoleAsync(
                role.Name!);

        if (users.Count > 0)
        {
            TempData["Error"] =
                "Cannot delete a role that has users.";

            return RedirectToAction(
                nameof(Index));
        }

        var result =
            await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            TempData["Error"] =
                string.Join(
                    " ",
                    result.Errors.Select(
                        x => x.Description));

            return RedirectToAction(
                nameof(Index));
        }

        TempData["Success"] =
            "Role deleted successfully.";

        return RedirectToAction(
            nameof(Index));
    }

    private async Task<List<Permission>> GetPermissionsAsync()
    {
        return await _context.Permissions
            .OrderBy(x => x.Group)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    private async Task<IActionResult> ReturnEditWithError(
        string id,
        string name,
        int[] selectedPermissions,
        string error)
    {
        var permissions =
            await GetPermissionsAsync();

        ViewBag.Error =
            error;

        ViewBag.RoleId =
            id;

        ViewBag.RoleName =
            name;

        ViewBag.SelectedPermissions =
            selectedPermissions.ToList();

        return View(
            "Edit",
            permissions);
    }

    private static bool IsProtectedRole(
        IdentityRole role)
    {
        return string.Equals(
            role.Name,
            "Admin",
            StringComparison.OrdinalIgnoreCase);
    }

}
