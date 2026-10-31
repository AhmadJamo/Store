using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/sales-deliveries")]
public sealed class SalesDeliveriesApiController(SalesDeliveryService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("SalesDeliveries.View")]
    public async Task<ActionResult<List<SalesDeliverySummaryDto>>> GetAll(string? search = null) =>
        Ok(await service.GetAllAsync(search));

    [HttpGet("{id:int}"), PermissionAuthorize("SalesDeliveries.View")]
    public async Task<ActionResult<SalesDeliveryDetailsDto>> Get(int id)
    { var result = await service.GetAsync(id); return result is null ? NotFound() : Ok(result); }

    [HttpGet("create-form"), PermissionAuthorize("SalesDeliveries.Create")]
    public async Task<ActionResult<SalesDeliveryCreatePageDto>> GetCreateForm(int salesOrderId)
    { var result = await service.GetCreatePageAsync(salesOrderId); return result is null ? NotFound() : Ok(result); }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesDeliveries.Create")]
    public Task<IActionResult> Create(CreateSalesDeliveryDto input) =>
        Run(async () => Ok(new { id = await service.CreateDraftAsync(input) }));

    [HttpPost("{id:int}/post"), ValidateAntiForgeryToken, PermissionAuthorize("SalesDeliveries.Post")]
    public Task<IActionResult> Post(int id, SalesDeliveryStatusCommandDto input) =>
        Change(id, input, async () => { await service.PostAsync(input); return NoContent(); });

    [HttpPost("{id:int}/cancel"), ValidateAntiForgeryToken, PermissionAuthorize("SalesDeliveries.Cancel")]
    public Task<IActionResult> Cancel(int id, SalesDeliveryStatusCommandDto input) =>
        Change(id, input, async () => { await service.CancelAsync(input); return NoContent(); });

    private Task<IActionResult> Change(int id, SalesDeliveryStatusCommandDto input,
        Func<Task<IActionResult>> action) => id != input.Id
        ? Task.FromResult<IActionResult>(BadRequest()) : Run(action);

    private async Task<IActionResult> Run(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
        }
    }
}
