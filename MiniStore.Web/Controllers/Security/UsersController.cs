using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Security;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

[Authorize]
public class UsersController(ITenantUserAdministrationService users) : Controller
{
    [HttpGet]
    [PermissionAuthorize("Users.View")]
    public async Task<IActionResult> Index() => View((await users.GetAllAsync()).ToList());

    [HttpGet]
    [PermissionAuthorize("Users.Create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = (await users.GetCreateAsync()).Roles.ToList();
        return View(new CreateTenantUserDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Users.Create")]
    public async Task<IActionResult> Create(CreateTenantUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Error = "All fields are required.";
            return await Rebuild(dto);
        }

        try
        {
            await users.CreateAsync(dto);
            TempData["Success"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ViewBag.Error = exception.Message;
            return await Rebuild(dto);
        }
    }

    private async Task<IActionResult> Rebuild(CreateTenantUserDto dto)
    {
        dto.Password = string.Empty;
        ViewBag.Roles = (await users.GetCreateAsync()).Roles.ToList();
        return View(dto);
    }
}
