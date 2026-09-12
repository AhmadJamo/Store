using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Web.Controllers;

[Authorize(Roles = "Admin")]
public class RolesController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppDbContext _context;
    public RolesController(
     RoleManager<IdentityRole> roleManager,
     UserManager<IdentityUser> userManager, AppDbContext context)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
    }

    // GET: /Roles
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

        ViewBag.RoleUsers = roleUsers;

        return View(roles);
    }



    // GET: /Roles/Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var permissions =
            await _context.Permissions
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .ToListAsync();

        return View(permissions);
    }





    // POST: /Roles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string name,
        int[] selectedPermissions)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ViewBag.Error = "Role name is required.";

            var permissions =
                await _context.Permissions
                    .OrderBy(x => x.Group)
                    .ThenBy(x => x.Name)
                    .ToListAsync();

            return View(permissions);
        }

        if (await _roleManager.RoleExistsAsync(name))
        {
            ViewBag.Error = "Role already exists.";

            var permissions =
                await _context.Permissions
                    .OrderBy(x => x.Group)
                    .ThenBy(x => x.Name)
                    .ToListAsync();

            return View(permissions);
        }

        var role = new IdentityRole(name);

        var result =
            await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            ViewBag.Error =
                string.Join(
                    ", ",
                    result.Errors.Select(x => x.Description));

            var permissions =
                await _context.Permissions
                    .OrderBy(x => x.Group)
                    .ThenBy(x => x.Name)
                    .ToListAsync();

            return View(permissions);
        }

        foreach (var permissionId in selectedPermissions.Distinct())
        {
            var permissionExists =
                await _context.Permissions
                    .AnyAsync(x => x.Id == permissionId);

            if (!permissionExists)
                continue;

            _context.RolePermissions.Add(
                new RolePermission(
                    role.Id,
                    permissionId));
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }





    // GET: /Roles/Edit/{id}
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var role =
            await _roleManager.FindByIdAsync(id);

        if (role == null)
            return NotFound();

        var permissions =
            await _context.Permissions
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .ToListAsync();

        var selectedPermissions =
            await _context.RolePermissions
                .Where(x => x.RoleId == id)
                .Select(x => x.PermissionId)
                .ToListAsync();

        ViewBag.RoleId = id;
        ViewBag.RoleName = role.Name;
        ViewBag.SelectedPermissions =
            selectedPermissions;

        return View(permissions);
    }




    // POST: /Roles/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    string id,
    string name,
    int[] selectedPermissions)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var role =
            await _roleManager.FindByIdAsync(id);

        if (role == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(name))
        {
            ViewBag.Error = "Role name is required.";

            var permissions =
                await _context.Permissions
                    .OrderBy(x => x.Group)
                    .ThenBy(x => x.Name)
                    .ToListAsync();

            ViewBag.RoleId = id;
            ViewBag.RoleName = name;
            ViewBag.SelectedPermissions =
                selectedPermissions.ToList();

            return View(permissions);
        }

        var existingRole =
            await _roleManager.FindByNameAsync(name);

        if (existingRole != null &&
            existingRole.Id != role.Id)
        {
            ViewBag.Error = "Role name already exists.";

            var permissions =
                await _context.Permissions
                    .OrderBy(x => x.Group)
                    .ThenBy(x => x.Name)
                    .ToListAsync();

            ViewBag.RoleId = id;
            ViewBag.RoleName = name;
            ViewBag.SelectedPermissions =
                selectedPermissions.ToList();

            return View(permissions);
        }

        role.Name = name;

        var updateResult =
            await _roleManager.UpdateAsync(role);

        if (!updateResult.Succeeded)
        {
            ViewBag.Error =
                string.Join(
                    ", ",
                    updateResult.Errors.Select(x =>
                        x.Description));

            var permissions =
                await _context.Permissions
                    .OrderBy(x => x.Group)
                    .ThenBy(x => x.Name)
                    .ToListAsync();

            ViewBag.RoleId = id;
            ViewBag.RoleName = name;
            ViewBag.SelectedPermissions =
                selectedPermissions.ToList();

            return View(permissions);
        }

        var oldPermissions =
            await _context.RolePermissions
                .Where(x => x.RoleId == id)
                .ToListAsync();

        _context.RolePermissions.RemoveRange(
            oldPermissions);

        foreach (var permissionId in
                 selectedPermissions.Distinct())
        {
            var permissionExists =
                await _context.Permissions
                    .AnyAsync(x => x.Id == permissionId);

            if (!permissionExists)
                continue;

            _context.RolePermissions.Add(
                new RolePermission(
                    id,
                    permissionId));
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }





    // GET: /Roles/Delete/{id}
    [HttpGet]
    public async Task<IActionResult> Delete(
        string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var role =
            await _roleManager.FindByIdAsync(id);

        if (role == null)
            return NotFound();

        var users =
            await _userManager.GetUsersInRoleAsync(
                role.Name!);

        if (users.Count > 0)
        {
            TempData["Error"] =
                "Cannot delete a role that has users.";

            return RedirectToAction(nameof(Index));
        }

        var result =
            await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            TempData["Error"] =
                string.Join(
                    " ",
                    result.Errors.Select(x =>
                        x.Description));

            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] =
            "Role deleted successfully.";

        return RedirectToAction(nameof(Index));
    }
}