using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class AuditEventsController(
    AuditEventReadService service,
    IAuthorizationService authorization,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("AuditEvents.View")]
    public async Task<IActionResult> Index([FromQuery] AuditEventQueryDto query)
    {
        try
        {
            var sensitive = (await authorization.AuthorizeAsync(User, null,
                $"{PermissionAuthorizeAttribute.PolicyPrefix}AuditEvents.ViewSensitive")).Succeeded;
            return View(await service.GetTenantPageAsync(query, sensitive, HttpContext.RequestAborted));
        }
        catch (ArgumentException exception)
        {
            TempData["NotificationType"] = "danger";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
            return View(await service.GetTenantPageAsync(new AuditEventQueryDto(), false, HttpContext.RequestAborted));
        }
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("AuditEvents.View"), PermissionAuthorize("AuditEvents.Export")]
    public async Task<IActionResult> Export(AuditEventQueryDto query, string purpose)
    {
        try
        {
            var sensitive = (await authorization.AuthorizeAsync(User, null,
                $"{PermissionAuthorizeAttribute.PolicyPrefix}AuditEvents.ViewSensitive")).Succeeded;
            var result = await service.ExportTenantAsync(query, purpose, sensitive, CancellationToken.None);
            return File(result.Content, "text/csv; charset=utf-8", result.FileName);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "danger";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
            return RedirectToAction(nameof(Index), query);
        }
    }
}
