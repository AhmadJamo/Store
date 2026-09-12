using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: /Users
    [HttpGet]
    [PermissionAuthorize("Users.View")]
    public async Task<IActionResult> Index()
    {
        var users =
            await _userManager.Users
                .OrderBy(x => x.UserName)
                .ToListAsync();

        var userRoles =
            new Dictionary<string, IList<string>>();

        foreach (var user in users)
        {
            userRoles[user.Id] =
                await _userManager.GetRolesAsync(user);
        }

        ViewBag.UserRoles =
            userRoles;

        return View(users);
    }

    // GET: /Users/Create
    [HttpGet]
    [PermissionAuthorize("Users.Create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles =
            await _roleManager.Roles
                .OrderBy(x => x.Name)
                .ToListAsync();

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
        string role)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(role))
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

        var selectedRole =
            await _roleManager.FindByNameAsync(
                role);

        if (selectedRole == null)
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

        var roleResult =
            await _userManager.AddToRoleAsync(
                user,
                role);

        if (!roleResult.Succeeded)
        {
            ViewBag.Error =
                string.Join(
                    " ",
                    roleResult.Errors.Select(
                        x => x.Description));

            await LoadRoles();

            return View();
        }

        TempData["Success"] =
            "User created successfully.";

        return RedirectToAction(
            nameof(Index));
    }

    private async Task LoadRoles()
    {
        ViewBag.Roles =
            await _roleManager.Roles
                .OrderBy(x => x.Name)
                .ToListAsync();
    }
}