using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Accounting;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/financial-reports"), PermissionAuthorize("JournalEntries.View")]
public sealed class FinancialReportsApiController(FinancialReportService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<FinancialReportPageDto>> Get([FromQuery] FinancialReportQueryDto query)
    {
        try { return Ok(await service.GetAsync(query)); }
        catch (ArgumentException error)
        {
            ModelState.AddModelError(string.Empty, localizer[error.Message]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
        }
    }
}
