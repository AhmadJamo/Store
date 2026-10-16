using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Areas.Platform.Controllers;

[Area("Platform")]
[Route("platform/audit-events")]
[Authorize(AuthenticationSchemes = PlatformAuthentication.Scheme, Policy = PlatformAuthentication.AuditViewPolicy)]
public sealed class PlatformAuditEventsController(
    AuditEventReadService service,
    IAuthorizationService authorization,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] AuditEventQueryDto query)
    {
        try
        {
            var sensitive = (await authorization.AuthorizeAsync(User, null, PlatformAuthentication.AuditSensitivePolicy)).Succeeded;
            return View(await service.GetPlatformPageAsync(query, sensitive, HttpContext.RequestAborted));
        }
        catch (ArgumentException exception)
        {
            TempData["PlatformAuditError"] = localizer[exception.Message].Value;
            return View(await service.GetPlatformPageAsync(new AuditEventQueryDto(), false, HttpContext.RequestAborted));
        }
    }

    [HttpPost("export"), ValidateAntiForgeryToken]
    [Authorize(Policy = PlatformAuthentication.AuditExportPolicy)]
    public async Task<IActionResult> Export(AuditEventQueryDto query, string purpose)
    {
        try
        {
            var sensitive = (await authorization.AuthorizeAsync(User, null, PlatformAuthentication.AuditSensitivePolicy)).Succeeded;
            var result = await service.ExportPlatformAsync(query, purpose, sensitive, CancellationToken.None);
            return File(result.Content, "text/csv; charset=utf-8", result.FileName);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["PlatformAuditError"] = localizer[exception.Message].Value;
            return RedirectToAction(nameof(Index), query);
        }
    }
}
