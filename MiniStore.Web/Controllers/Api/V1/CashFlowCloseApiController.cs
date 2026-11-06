using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Accounting;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/cash-flow-close"), PermissionAuthorize("JournalEntries.View")]
public sealed class CashFlowCloseApiController(CashFlowCloseService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CashFlowClosePageDto>> Get([FromQuery] CashFlowCloseQueryDto query,
        CancellationToken cancellationToken)
    {
        try { return Ok(await service.GetAsync(query, cancellationToken)); }
        catch (ArgumentException error)
        {
            ModelState.AddModelError(string.Empty, localizer[error.Message]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
        }
    }
}
