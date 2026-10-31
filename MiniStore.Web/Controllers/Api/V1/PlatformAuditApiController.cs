using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

public sealed record PlatformAuditExportRequest(AuditEventQueryDto Query, string Purpose);
public sealed record ReleasePlatformAuditHoldRequest(string Reason);

[ApiController]
[Route("api/v1/platform/audit-events")]
[Authorize(AuthenticationSchemes = PlatformAuthentication.Scheme,
    Policy = PlatformAuthentication.AuditViewPolicy)]
public sealed class PlatformAuditEventsApiController(
    AuditEventReadService service,
    IAuthorizationService authorization,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] AuditEventQueryDto query)
    {
        try
        {
            return Ok(await service.GetPlatformPageAsync(query, await CanViewSensitive(), HttpContext.RequestAborted));
        }
        catch (ArgumentException exception)
        {
            return Validation(exception.Message);
        }
    }

    [HttpPost("export")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PlatformAuthentication.AuditExportPolicy)]
    public async Task<IActionResult> Export(PlatformAuditExportRequest request)
    {
        try
        {
            var result = await service.ExportPlatformAsync(
                request.Query,
                request.Purpose,
                await CanViewSensitive(),
                HttpContext.RequestAborted);
            return File(result.Content, "text/csv; charset=utf-8", result.FileName);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return Validation(exception.Message);
        }
    }

    private async Task<bool> CanViewSensitive() =>
        (await authorization.AuthorizeAsync(User, null, PlatformAuthentication.AuditSensitivePolicy)).Succeeded;

    private IActionResult Validation(string message)
    {
        ModelState.AddModelError(string.Empty, localizer[message]);
        return InternalApiProblemFactory.Validation(
            HttpContext,
            ModelState,
            localizer["One or more validation errors occurred."]);
    }
}

[ApiController]
[Route("api/v1/platform/audit-retention")]
[Authorize(AuthenticationSchemes = PlatformAuthentication.Scheme,
    Policy = PlatformAuthentication.AuditRetentionPolicy)]
public sealed class PlatformAuditRetentionApiController(
    AuditRetentionService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AuditRetentionStatusDto>> Get() =>
        Ok(await service.GetStatusAsync(HttpContext.RequestAborted));

    [HttpPost("holds")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> CreateHold(CreateAuditLegalHoldDto request) =>
        Run(async () => await service.CreateLegalHoldAsync(request, HttpContext.RequestAborted));

    [HttpPost("holds/{id:long}/release")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> ReleaseHold(long id, ReleasePlatformAuditHoldRequest request) =>
        Run(() => service.ReleaseLegalHoldAsync(id, request.Reason, HttpContext.RequestAborted));

    [HttpPost("checkpoint")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Checkpoint() =>
        Run(async () => await service.CreateCheckpointAsync(HttpContext.RequestAborted));

    [HttpPost("run")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> RunRetention() =>
        Run(async () => await service.RunRetentionAsync(HttpContext.RequestAborted));

    private async Task<IActionResult> Run(Func<Task> operation)
    {
        try
        {
            await operation();
            return NoContent();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(
                HttpContext,
                ModelState,
                localizer["One or more validation errors occurred."]);
        }
    }
}
