using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Areas.Platform.Controllers;

[Area("Platform")]
[Route("platform/audit-retention")]
[Authorize(AuthenticationSchemes = PlatformAuthentication.Scheme,
    Policy = PlatformAuthentication.AuditRetentionPolicy)]
public sealed class PlatformAuditRetentionController(
    AuditRetentionService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index() => View(await service.GetStatusAsync(HttpContext.RequestAborted));

    [HttpPost("holds"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateHold(CreateAuditLegalHoldDto input)
    {
        try
        {
            await service.CreateLegalHoldAsync(input, CancellationToken.None);
            Success("Legal hold created. Retention will skip every matching audit event.");
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { Error(exception.Message); }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("holds/{id:long}/release"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ReleaseHold(long id, string reason)
    {
        try
        {
            await service.ReleaseLegalHoldAsync(id, reason, CancellationToken.None);
            Success("Legal hold released. Normal retention rules apply again.");
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { Error(exception.Message); }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("checkpoint"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkpoint()
    {
        try
        {
            var result = await service.CreateCheckpointAsync(CancellationToken.None);
            Success("External audit checkpoint created for {0} events.", result.RecordCount);
        }
        catch (InvalidOperationException exception) { Error(exception.Message); }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("run"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Run()
    {
        try
        {
            var result = await service.RunRetentionAsync(CancellationToken.None);
            Success("Audit retention completed after a verified external checkpoint; {0} events were removed.", result.DeletedCount);
        }
        catch (InvalidOperationException exception) { Error(exception.Message); }
        return RedirectToAction(nameof(Index));
    }

    private void Success(string key, int? count = null)
    {
        TempData["AuditRetentionType"] = "success";
        TempData["AuditRetentionMessage"] = count.HasValue
            ? string.Format(localizer[key].Value, count.Value)
            : localizer[key].Value;
    }

    private void Error(string key)
    {
        TempData["AuditRetentionType"] = "danger";
        TempData["AuditRetentionMessage"] = localizer[key].Value;
    }
}
