using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/sales-orders/{salesOrderId:int}/reservations")]
public sealed class SalesOrderReservationsApiController(SalesOrderReservationService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("SalesOrders.View")]
    public async Task<ActionResult<SalesOrderReservationPageDto>> Get(int salesOrderId)
    { var result = await service.GetPageAsync(salesOrderId); return result is null ? NotFound() : Ok(result); }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesOrders.Reserve")]
    public Task<IActionResult> Reserve(int salesOrderId, CreateSalesOrderReservationDto input) =>
        Change(salesOrderId, input.SalesOrderId, async () => { await service.ReserveAsync(input); return NoContent(); });

    [HttpPost("release"), ValidateAntiForgeryToken, PermissionAuthorize("SalesOrders.ReleaseReservation")]
    public Task<IActionResult> Release(int salesOrderId, ReleaseSalesOrderReservationDto input) =>
        Change(salesOrderId, input.SalesOrderId, async () => { await service.ReleaseAsync(input); return NoContent(); });

    private Task<IActionResult> Change(int routeId, int inputId, Func<Task<IActionResult>> action) =>
        routeId != inputId ? Task.FromResult<IActionResult>(BadRequest()) : Run(action);

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
