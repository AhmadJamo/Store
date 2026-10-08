using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class PurchaseApprovalRulesController(
    PurchaseApprovalService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet]
    [PermissionAuthorize("PurchaseApprovalRules.View")]
    public async Task<IActionResult> Index() => View(await service.GetRulesPageAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("PurchaseApprovalRules.Manage")]
    public async Task<IActionResult> Create(CreatePurchaseApprovalRuleDto dto)
    {
        try
        {
            await service.CreateRuleAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Purchase approval rule created."].Value;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return View(nameof(Index), await service.GetRulesPageAsync(dto));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("PurchaseApprovalRules.Manage")]
    public async Task<IActionResult> SetActive(int id, bool active, byte[] rowVersion)
    {
        await service.SetActiveAsync(id, active, rowVersion);
        return RedirectToAction(nameof(Index));
    }
}
