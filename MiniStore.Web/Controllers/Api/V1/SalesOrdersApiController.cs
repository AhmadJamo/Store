using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/sales-orders")]
public sealed class SalesOrdersApiController(SalesOrderService service,
    CustomerInvoiceService invoices, SalesOrderApprovalService approvals,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("SalesOrders.View")]
    public async Task<ActionResult<List<SalesOrderSummaryDto>>> GetAll(
        SalesOrderStatus? status = null, string? search = null) =>
        Ok(await service.GetAllAsync(status, search));

    [HttpGet("{id:int}"), PermissionAuthorize("SalesOrders.View")]
    public async Task<ActionResult<SalesOrderDetailsDto>> Get(int id)
    {
        var result = await service.GetAsync(id);
        if (result is null) return NotFound();
        result.Invoice = await invoices.GetProgressAsync(id);
        return Ok(result);
    }

    [HttpPost("from-quotation"), ValidateAntiForgeryToken, PermissionAuthorize("SalesOrders.Create")]
    public Task<IActionResult> CreateFromQuotation(ConvertSalesQuotationDto input) =>
        Run(async () => Ok(new { id = await service.CreateFromQuotationAsync(input) }));

    [HttpPost("{id:int}/confirm"), ValidateAntiForgeryToken, PermissionAuthorize("SalesOrders.Confirm")]
    public Task<IActionResult> Confirm(int id, SalesOrderStatusCommandDto input) =>
        Change(id, input, async () => Ok(new { outcome = await service.ConfirmAsync(input) }));

    [HttpPost("{id:int}/cancel"), ValidateAntiForgeryToken, PermissionAuthorize("SalesOrders.Cancel")]
    public Task<IActionResult> Cancel(int id, SalesOrderStatusCommandDto input) =>
        Change(id, input, async () => { await service.CancelAsync(input); return NoContent(); });

    [HttpPost("{id:int}/approval/approve"), ValidateAntiForgeryToken, PermissionAuthorize("SalesOrders.ApproveException")]
    public Task<IActionResult> Approve(int id, SalesOrderApprovalDecisionDto input) =>
        Decide(id, input, true);

    [HttpPost("{id:int}/approval/reject"), ValidateAntiForgeryToken, PermissionAuthorize("SalesOrders.ApproveException")]
    public Task<IActionResult> Reject(int id, SalesOrderApprovalDecisionDto input) =>
        Decide(id, input, false);

    private Task<IActionResult> Decide(int id, SalesOrderApprovalDecisionDto input, bool approve)
    {
        if (id != input.SalesOrderId) return Task.FromResult<IActionResult>(BadRequest());
        return Run(async () => Ok(new { status = await approvals.DecideAsync(input, approve) }));
    }

    private Task<IActionResult> Change(int id, SalesOrderStatusCommandDto input,
        Func<Task<IActionResult>> action) => id != input.Id
        ? Task.FromResult<IActionResult>(BadRequest()) : Run(action);

    private async Task<IActionResult> Run(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or UnauthorizedAccessException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
        }
    }
}
