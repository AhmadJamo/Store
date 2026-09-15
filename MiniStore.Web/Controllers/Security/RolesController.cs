using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Security;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

[Authorize]
public class RolesController(TenantRoleService roles, ILogger<RolesController> logger) : Controller
{
    [HttpGet]
    [PermissionAuthorize("Roles.View")]
    public async Task<IActionResult> Index() => View(await roles.GetAllAsync());

    [HttpGet]
    [PermissionAuthorize("Roles.Create")]
    public async Task<IActionResult> Create() => View(await roles.GetCreateAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Roles.Create")]
    public async Task<IActionResult> Create(TenantRoleEditDto dto)
    {
        try
        {
            await roles.CreateAsync(dto);
            TempData["Success"] = "Role created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            logger.LogWarning(exception, "Tenant role creation was rejected.");
            var model = await roles.GetCreateAsync();
            model.Name = dto.Name;
            model.Description = dto.Description;
            model.SelectedPermissionIds = dto.SelectedPermissionIds;
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }
    }

    [HttpGet]
    [PermissionAuthorize("Roles.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await roles.GetEditAsync(id);
        if (model is null) return NotFound();
        if (model.IsSystem)
        {
            TempData["Error"] = "System roles cannot be edited.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Roles.Edit")]
    public async Task<IActionResult> Edit(TenantRoleEditDto dto)
    {
        try
        {
            await roles.UpdateAsync(dto);
            TempData["Success"] = "Role updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            logger.LogWarning(exception, "Tenant role update was rejected for role {RoleId}.", dto.Id);
            var model = await roles.GetEditAsync(dto.Id);
            if (model is null) return NotFound();
            model.Name = dto.Name;
            model.Description = dto.Description;
            model.SelectedPermissionIds = dto.SelectedPermissionIds;
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Roles.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await roles.DeleteAsync(id);
            TempData["Success"] = "Role deleted successfully.";
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            logger.LogWarning(exception, "Tenant role deletion was rejected for role {RoleId}.", id);
            TempData["Error"] = exception.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
