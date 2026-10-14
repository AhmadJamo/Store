using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class PurchaseReplenishmentController(
    ReplenishmentAutomationService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("PurchaseReplenishment.View")]
    public async Task<IActionResult> Index() => View(await service.GetPageAsync());

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("PurchaseReplenishment.Manage")]
    public async Task<IActionResult> SaveSettings(ReplenishmentAutomationPageDto input)
    {
        try
        {
            await service.SaveSettingsAsync(input.Settings);
            Notify("success", "Automated replenishment settings saved.");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            Notify("danger", ex.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("PurchaseReplenishment.Run")]
    public async Task<IActionResult> RunNow()
    {
        try
        {
            await service.RunNowAsync();
            Notify("success", "Replenishment planning run completed.");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            Notify("danger", ex.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    private void Notify(string type, string message)
    {
        TempData["NotificationType"] = type;
        TempData["NotificationMessage"] = localizer[message].Value;
    }
}
