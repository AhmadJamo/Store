using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class SupplierPurchasingController(
    SupplierPurchasingInfoService service,
    IStringLocalizer<SharedResource> localizer,
    ILogger<SupplierPurchasingController> logger) : Controller
{
    [HttpGet]
    [PermissionAuthorize("Purchases.SupplierTerms.View")]
    public async Task<IActionResult> Index(int? editId = null)
    {
        try { return View(await service.GetPageAsync(editId)); }
        catch (InvalidOperationException exception)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Purchases.SupplierTerms.Manage")]
    public async Task<IActionResult> Save(SaveSupplierPurchasingInfoDto dto)
    {
        try
        {
            if (!ModelState.IsValid) throw new ArgumentException("Please check the entered supplier purchasing data.");
            await service.SaveAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Supplier purchasing information saved."].Value;
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to save supplier purchasing information");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer["The operation could not be completed. Please try again or contact the administrator."].Value;
        }
        return RedirectToAction(nameof(Index), dto.Id > 0 ? new { editId = dto.Id } : null);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Purchases.SupplierTerms.Manage")]
    public async Task<IActionResult> SetActive(int id, bool isActive, byte[] rowVersion)
    {
        try
        {
            await service.SetActiveAsync(id, isActive, rowVersion);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Supplier purchasing information updated."].Value;
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
        }
        return RedirectToAction(nameof(Index));
    }
}
