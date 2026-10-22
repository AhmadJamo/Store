using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Analytics;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class AnalyticsCenterController(AnalyticsCenterService service,
    IAuthorizationService authorization, IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("AnalyticsCenter.View")]
    public async Task<IActionResult> Index([FromQuery] AnalyticsCenterQueryDto query)
    {
        try { return View(await service.GetAsync(query)); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            Notify("danger", ex.Message);
            return View(await service.GetAsync(new AnalyticsCenterQueryDto()));
        }
    }

    [HttpGet, PermissionAuthorize("AnalyticsCenter.Customize")]
    public async Task<IActionResult> Create() => View(await service.GetCreatePageAsync());

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("AnalyticsCenter.Customize")]
    public async Task<IActionResult> Create(CreateAnalyticsWorkspaceDto dto)
    {
        if (!ModelState.IsValid) return View(await service.GetCreatePageAsync(dto));
        try
        {
            var canShare = (await authorization.AuthorizeAsync(User, null, "Permission:AnalyticsCenter.Manage")).Succeeded;
            var id = await service.CreateAsync(dto, canShare);
            Notify("success", "Analytics workspace created.");
            return RedirectToAction(nameof(Index), new { workspaceId = id });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        { ModelState.AddModelError(string.Empty, localizer[ex.Message]); return View(await service.GetCreatePageAsync(dto)); }
    }

    private void Notify(string type, string key) { TempData["NotificationType"] = type; TempData["NotificationMessage"] = localizer[key].Value; }
}
