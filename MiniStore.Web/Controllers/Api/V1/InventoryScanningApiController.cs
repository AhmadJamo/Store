using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Inventory.Scanning;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/inventory-scanning")]
public sealed class InventoryScanningApiController(
    InventoryScanningService scanning,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("InventoryScanning.View")]
    public async Task<ActionResult<InventoryScanPageDto>> Resolve(
        string? adjustmentScan,
        string? productScan,
        string? sourceLocationScan,
        string? locationScan) =>
        Ok(await scanning.ResolveAsync(new InventoryScanQueryDto
        {
            AdjustmentScan = adjustmentScan ?? string.Empty,
            ProductScan = productScan ?? string.Empty,
            SourceLocationScan = sourceLocationScan ?? string.Empty,
            LocationScan = locationScan ?? string.Empty
        }));

    [HttpPost("putaway"), ValidateAntiForgeryToken]
    [PermissionAuthorize("InventoryScanning.View")]
    [PermissionAuthorize("ProductStock.Edit")]
    public Task<IActionResult> Putaway(ScannedPutawayDto input) =>
        Execute(() => scanning.ExecutePutawayAsync(input));

    [HttpPost("relocate"), ValidateAntiForgeryToken]
    [PermissionAuthorize("InventoryScanning.View")]
    [PermissionAuthorize("LocationMovements.Create")]
    public Task<IActionResult> Relocate(ScannedRelocationDto input) =>
        Execute(() => scanning.ExecuteRelocationAsync(input));

    [HttpPost("count"), ValidateAntiForgeryToken]
    [PermissionAuthorize("InventoryScanning.View")]
    [PermissionAuthorize("InventoryAdjustments.Count")]
    public Task<IActionResult> Count(ScannedInventoryCountDto input) =>
        Execute(() => scanning.RecordCountAsync(input));

    private async Task<IActionResult> Execute(Func<Task> action)
    {
        try
        {
            await action();
            return NoContent();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
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
