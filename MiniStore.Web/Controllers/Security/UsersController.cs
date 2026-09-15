using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStore.Web.Authorization;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Application.Saas;
using MiniStore.Application.Services;

namespace MiniStore.Web.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly TenantRoleService _roles;
    private readonly ITenantContext _tenantContext;
    private readonly ITenantMembershipRepository _memberships;
    private readonly EntitlementService _entitlements;

    public UsersController(
        UserManager<IdentityUser> userManager,
        TenantRoleService roles,
        ITenantContext tenantContext,
        ITenantMembershipRepository memberships,
        EntitlementService entitlements)
    {
        _userManager = userManager;
        _roles = roles;
        _tenantContext = tenantContext;
        _memberships = memberships;
        _entitlements = entitlements;
    }

    // GET: /Users
    [HttpGet]
    [PermissionAuthorize("Users.View")]
    public async Task<IActionResult> Index()
    {
        var tenantId = _tenantContext.TenantId
            ?? throw new InvalidOperationException("An active company is required.");
        var tenantUserIds = await _memberships.GetActiveUserIdsAsync(tenantId);
        var users =
            await _userManager.Users
                .Where(x => tenantUserIds.Contains(x.Id))
                .OrderBy(x => x.UserName)
                .ToListAsync();

        ViewBag.UserRoles = await _roles.GetUserRoleNamesAsync(users.Select(x => x.Id).ToArray());

        return View(users);
    }

    // GET: /Users/Create
    [HttpGet]
    [PermissionAuthorize("Users.Create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = await _roles.GetOptionsAsync();

        return View();
    }

    // POST: /Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Users.Create")]
    public async Task<IActionResult> Create(
        string username,
        string email,
        string password,
        int roleId)
    {
        try
        {
            await _entitlements.EnsureLimitAsync(SaasLimitKeys.Users);
        }
        catch (InvalidOperationException exception)
        {
            ViewBag.Error = exception.Message;
            await LoadRoles();
            return View();
        }
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            roleId <= 0)
        {
            ViewBag.Error =
                "All fields are required.";

            await LoadRoles();

            return View();
        }

        var existingUser =
            await _userManager.FindByNameAsync(
                username);

        if (existingUser != null)
        {
            ViewBag.Error =
                "Username already exists.";

            await LoadRoles();

            return View();
        }

        var selectedRole = (await _roles.GetOptionsAsync()).SingleOrDefault(x => x.Id == roleId);
        if (selectedRole is null)
        {
            ViewBag.Error =
                "Selected role does not exist.";

            await LoadRoles();

            return View();
        }

        var user =
            new IdentityUser
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true
            };

        var result =
            await _userManager.CreateAsync(
                user,
                password);

        if (!result.Succeeded)
        {
            ViewBag.Error =
                string.Join(
                    " ",
                    result.Errors.Select(
                        x => x.Description));

            await LoadRoles();

            return View();
        }

        var tenantId = _tenantContext.TenantId
            ?? throw new InvalidOperationException("An active company is required.");
        await _memberships.AddAsync(new TenantMembership(tenantId, user.Id));
        await _roles.AssignUserAsync(user.Id, selectedRole.Id);
        await _memberships.SaveChangesAsync();

        TempData["Success"] =
            "User created successfully.";

        return RedirectToAction(
            nameof(Index));
    }

    private async Task LoadRoles()
    {
        ViewBag.Roles = await _roles.GetOptionsAsync();
    }
}
