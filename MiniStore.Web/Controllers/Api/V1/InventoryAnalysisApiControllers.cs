using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Inventory.Insights;
using MiniStore.Application.DTOs.Inventory.Reconciliation;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/inventory-insights")]
public sealed class InventoryInsightsApiController(
    InventoryInsightsService insights,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("InventoryInsights.View")]
    public async Task<IActionResult> Get(
        int? warehouseId,
        string? search,
        InventoryActivityState? state,
        bool attentionOnly = false,
        int slowDays = 30,
        int deadDays = 90)
    {
        try
        {
            return Ok(await insights.GetPageAsync(new InventoryInsightsQueryDto
            {
                WarehouseId = warehouseId,
                Search = search ?? string.Empty,
                State = state,
                AttentionOnly = attentionOnly,
                SlowDays = slowDays,
                DeadDays = deadDays
            }));
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

[ApiController]
[Authorize]
[Route("api/v1/inventory-reconciliation")]
public sealed class InventoryReconciliationApiController(
    InventoryReconciliationService reconciliation) : ControllerBase
{
    [HttpGet, PermissionAuthorize("InventoryReconciliation.View")]
    public async Task<ActionResult<InventoryReconciliationPageDto>> Get(
        int? warehouseId,
        string? search,
        bool exceptionsOnly = true,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default) =>
        Ok(await reconciliation.SearchAsync(new InventoryReconciliationQueryDto
        {
            WarehouseId = warehouseId,
            Search = search,
            ExceptionsOnly = exceptionsOnly,
            Page = page,
            PageSize = pageSize
        }, cancellationToken));
}
