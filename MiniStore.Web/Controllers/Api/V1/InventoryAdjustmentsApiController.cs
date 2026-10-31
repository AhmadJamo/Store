using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Inventory.Adjustments;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/inventory-adjustments")]
public sealed class InventoryAdjustmentsApiController(
    InventoryAdjustmentService adjustments,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("InventoryAdjustments.View")]
    public async Task<ActionResult<List<InventoryAdjustmentRowDto>>> GetAll() => Ok(await adjustments.GetAllAsync());

    [HttpGet("create-form"), PermissionAuthorize("InventoryAdjustments.Create")]
    public async Task<ActionResult<InventoryAdjustmentCreateApiPageDto>> GetCreateForm()
    {
        var page = await adjustments.GetCreatePageAsync();
        return Ok(new InventoryAdjustmentCreateApiPageDto
        {
            Input = page.Input,
            Warehouses = page.Warehouses.Select(x => new InventoryAdjustmentWarehouseOptionDto(x.Id, x.Name)).ToList(),
            Locations = page.Locations.Select(x => new InventoryAdjustmentLocationOptionDto(x.Id, x.WarehouseId, x.Name)).ToList(),
            Products = page.Products.Select(x => new InventoryAdjustmentProductOptionDto(x.Id, x.Name)).ToList()
        });
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("InventoryAdjustments.Create")]
    public async Task<IActionResult> Create(CreateInventoryAdjustmentDto input)
    {
        try { return Ok(new { id = await adjustments.CreateAsync(input, UserId()) }); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { return Validation(exception); }
    }

    [HttpGet("{id:long}"), PermissionAuthorize("InventoryAdjustments.View")]
    public async Task<ActionResult<InventoryAdjustmentDetailsDto>> GetDetails(long id)
    {
        var result = await adjustments.GetDetailsAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:long}/count"), ValidateAntiForgeryToken, PermissionAuthorize("InventoryAdjustments.Count")]
    public Task<IActionResult> Count(long id, RecordInventoryCountsDto input) => Execute(() => adjustments.RecordCountsAsync(id, input, UserId()));

    [HttpPost("{id:long}/approve"), ValidateAntiForgeryToken, PermissionAuthorize("InventoryAdjustments.Approve")]
    public Task<IActionResult> Approve(long id) => Execute(() => adjustments.ApproveAsync(id, UserId()));

    [HttpPost("{id:long}/post"), ValidateAntiForgeryToken, PermissionAuthorize("InventoryAdjustments.Post")]
    public Task<IActionResult> Post(long id) => Execute(() => adjustments.PostAsync(id, UserId()));

    [HttpPost("{id:long}/cancel"), ValidateAntiForgeryToken, PermissionAuthorize("InventoryAdjustments.Cancel")]
    public Task<IActionResult> Cancel(long id) => Execute(() => adjustments.CancelAsync(id, UserId()));

    private async Task<IActionResult> Execute(Func<Task> action)
    {
        try { await action(); return NoContent(); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { return Validation(exception); }
    }

    private IActionResult Validation(Exception exception)
    {
        ModelState.AddModelError(string.Empty, localizer[exception.Message]);
        return InternalApiProblemFactory.Validation(HttpContext, ModelState,
            localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
    }

    private string UserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("Authenticated user was not found.");
}
