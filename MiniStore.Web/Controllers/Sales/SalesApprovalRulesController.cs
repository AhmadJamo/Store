using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class SalesApprovalRulesController(
    SalesOrderApprovalService service, IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("SalesApprovalRules.View")]
    public async Task<IActionResult> Index() => View(await service.GetRulesPageAsync());

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesApprovalRules.Manage")]
    public async Task<IActionResult> Create(CreateSalesApprovalRuleDto dto)
    {
        try
        {
            await service.CreateRuleAsync(dto);
            Notify("success", "Sales approval rule created.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return View(nameof(Index), await service.GetRulesPageAsync(dto));
        }
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesApprovalRules.Manage")]
    public async Task<IActionResult> SetActive(int id, bool active, byte[] rowVersion)
    {
        try { await service.SetActiveAsync(id, active, rowVersion); Notify("success", "Sales approval rule status updated."); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { Notify("danger", exception.Message); }
        return RedirectToAction(nameof(Index));
    }

    private void Notify(string type, string key)
    { TempData["NotificationType"] = type; TempData["NotificationMessage"] = localizer[key].Value; }
}
