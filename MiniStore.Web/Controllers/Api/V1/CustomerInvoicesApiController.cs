using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/customer-invoices")]
public sealed class CustomerInvoicesApiController(CustomerInvoiceService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("CustomerInvoices.View")]
    public async Task<ActionResult<List<CustomerInvoiceSummaryDto>>> GetAll(
        CustomerInvoiceStatus? status = null, string? search = null) => Ok(await service.GetAllAsync(status, search));

    [HttpGet("{id:int}"), PermissionAuthorize("CustomerInvoices.View")]
    public async Task<ActionResult<CustomerInvoiceDetailsDto>> Get(int id)
    { var result = await service.GetAsync(id); return result is null ? NotFound() : Ok(result); }

    [HttpGet("create-form"), PermissionAuthorize("CustomerInvoices.Create")]
    public async Task<ActionResult<CustomerInvoiceCreatePageDto>> GetCreateForm(int salesOrderId)
    { var result = await service.GetCreatePageAsync(salesOrderId); return result is null ? NotFound() : Ok(result); }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerInvoices.Create")]
    public Task<IActionResult> Create(CreateCustomerInvoiceDto input) =>
        Run(async () => Ok(new { id = await service.CreateDraftAsync(input) }));

    [HttpPost("{id:int}/approve"), ValidateAntiForgeryToken, PermissionAuthorize("CustomerInvoices.Approve")]
    public Task<IActionResult> Approve(int id, CustomerInvoiceStatusCommandDto input) =>
        Change(id, input.Id, async () => { await service.ApproveAsync(input); return NoContent(); });

    [HttpPost("{id:int}/cancel"), ValidateAntiForgeryToken, PermissionAuthorize("CustomerInvoices.Cancel")]
    public Task<IActionResult> Cancel(int id, CustomerInvoiceStatusCommandDto input) =>
        Change(id, input.Id, async () => { await service.CancelAsync(input); return NoContent(); });

    [HttpPost("{id:int}/post"), ValidateAntiForgeryToken, PermissionAuthorize("CustomerInvoices.Post")]
    public Task<IActionResult> Post(int id, CustomerInvoiceStatusCommandDto input) =>
        Change(id, input.Id, async () => { await service.PostAsync(input); return NoContent(); });

    [HttpPost("{id:int}/reverse"), ValidateAntiForgeryToken, PermissionAuthorize("CustomerInvoices.Reverse")]
    public Task<IActionResult> Reverse(int id, ReverseCustomerInvoiceDto input) =>
        Change(id, input.Id, async () => { await service.ReverseAsync(input); return NoContent(); });

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
