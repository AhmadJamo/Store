using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Products;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class ProductTemplatesController(
    ProductTemplateService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet]
    [PermissionAuthorize("Products.View")]
    public async Task<IActionResult> Index() => View(await service.GetPageAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<IActionResult> Create(CreateProductTemplateDto dto)
    {
        try
        {
            await service.CreateAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Product template created successfully."].Value;
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
    public async Task<IActionResult> Assign(AssignProductTemplateDto dto)
    {
        try
        {
            await service.AssignAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Product template assignment updated successfully."].Value;
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
    public async Task<IActionResult> Preview(VariantGenerationInputDto dto)
    {
        try { return View("Index", await service.PreviewGenerationAsync(dto)); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<IActionResult> Generate(VariantGenerationInputDto dto)
    {
        try
        {
            var created = await service.GenerateAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = string.Format(localizer["{0} variant(s) created successfully."], created);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
        }
        return RedirectToAction(nameof(Index));
    }
}
