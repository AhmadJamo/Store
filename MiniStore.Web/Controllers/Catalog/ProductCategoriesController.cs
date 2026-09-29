using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Products;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class ProductCategoriesController(
    ProductCategoryService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet]
    [PermissionAuthorize("Products.View")]
    public async Task<IActionResult> Index() => View(await service.GetAllAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<IActionResult> Create(CreateProductCategoryDto dto)
    {
        try
        {
            await service.CreateAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = "Product category created successfully.";
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
        }
        return RedirectToAction(nameof(Index));
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
            TempData["NotificationMessage"] = "Product category updated successfully.";
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
        }
        return RedirectToAction(nameof(Index));
    }
}
