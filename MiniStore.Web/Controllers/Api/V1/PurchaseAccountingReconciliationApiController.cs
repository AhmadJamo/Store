using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/purchase-accounting-reconciliation")]
public sealed class PurchaseAccountingReconciliationApiController(
    PurchaseAccountingReconciliationService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("PurchaseAccountingReconciliation.View")]
    public async Task<IActionResult> Get(
        [FromQuery] PurchaseAccountingReconciliationQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await service.GetAsync(query, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(
                HttpContext,
                ModelState,
                localizer["One or more validation errors occurred."],
                InternalApiProblemCodes.Validation);
        }
    }
}
