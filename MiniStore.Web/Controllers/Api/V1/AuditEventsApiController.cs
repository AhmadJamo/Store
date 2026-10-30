using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

public sealed record AuditEventExportRequest(AuditEventQueryDto Query, string Purpose);

[ApiController]
[Authorize]
[Route("api/v1/audit-events")]
public sealed class AuditEventsApiController(
    AuditEventReadService service,
    IAuthorizationService authorization,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("AuditEvents.View")]
    public async Task<IActionResult> Get([FromQuery] AuditEventQueryDto query)
    {
        try
        {
            return Ok(await service.GetTenantPageAsync(
                query,
                await CanViewSensitive(),
                HttpContext.RequestAborted));
        }
        catch (ArgumentException exception)
        {
            return Validation(exception.Message);
        }
    }

    [HttpPost("export")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("AuditEvents.View")]
    [PermissionAuthorize("AuditEvents.Export")]
    public async Task<IActionResult> Export(AuditEventExportRequest request)
    {
        try
        {
            var result = await service.ExportTenantAsync(
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
        (await authorization.AuthorizeAsync(User, "Permission:AuditEvents.ViewSensitive")).Succeeded;

    private IActionResult Validation(string message)
    {
        ModelState.AddModelError(string.Empty, localizer[message]);
        return InternalApiProblemFactory.Validation(
            HttpContext,
            ModelState,
            localizer["One or more validation errors occurred."]);
    }
}
