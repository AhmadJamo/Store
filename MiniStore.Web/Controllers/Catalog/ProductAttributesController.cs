using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Products;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class ProductAttributesController(
    ProductAttributeService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet]
    [PermissionAuthorize("Products.View")]
    public async Task<IActionResult> Index() => View(await service.GetPageAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<IActionResult> Create(CreateProductAttributeDto dto)
    {
        try
        {
            await service.CreateAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Product attribute created successfully."].Value;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return View(nameof(Index), await service.GetPageAsync(dto));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<IActionResult> SetActive(int id, bool isActive)
    {
        try
        {
            await service.SetActiveAsync(id, isActive);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Product attribute updated successfully."].Value;
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
        }
        return RedirectToAction(nameof(Index));
    }
}
