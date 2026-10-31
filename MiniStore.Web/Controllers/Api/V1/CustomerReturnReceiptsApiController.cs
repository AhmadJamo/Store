using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/customer-return-receipts")]
public sealed class CustomerReturnReceiptsApiController(
    CustomerReturnReceiptService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("CustomerReturnReceipts.View")]
    public async Task<ActionResult<List<CustomerReturnReceiptSummaryDto>>> GetAll(string? search = null) =>
        Ok(await service.GetAllAsync(search));

    [HttpGet("{id:int}"), PermissionAuthorize("CustomerReturnReceipts.View")]
    public async Task<ActionResult<CustomerReturnReceiptDetailsDto>> Get(int id)
    {
        var value = await service.GetAsync(id);
        return value is null ? NotFound() : Ok(value);
    }

    [HttpGet("create-form"), PermissionAuthorize("CustomerReturnReceipts.Create")]
    public async Task<ActionResult<CustomerReturnReceiptCreatePageDto>> GetCreateForm(int customerReturnRequestId)
    {
        var value = await service.GetCreatePageAsync(customerReturnRequestId);
        return value is null ? NotFound() : Ok(value);
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerReturnReceipts.Create")]
    public Task<IActionResult> Create(CreateCustomerReturnReceiptDto input) =>
        Run(async () => Ok(new { id = await service.CreateAsync(input) }));

    [HttpPost("{id:int}/inspect"), ValidateAntiForgeryToken, PermissionAuthorize("CustomerReturnReceipts.Inspect")]
    public Task<IActionResult> Inspect(int id, CustomerReturnReceiptCommandDto input) =>
        Change(id, input, service.InspectAsync);

    [HttpPost("{id:int}/post"), ValidateAntiForgeryToken, PermissionAuthorize("CustomerReturnReceipts.PostDisposition")]
    public Task<IActionResult> Post(int id, CustomerReturnReceiptCommandDto input) =>
        Change(id, input, service.PostAsync);

    [HttpPost("{id:int}/cancel"), ValidateAntiForgeryToken, PermissionAuthorize("CustomerReturnReceipts.Cancel")]
    public Task<IActionResult> Cancel(int id, CustomerReturnReceiptCommandDto input) =>
        Change(id, input, service.CancelAsync);

    private Task<IActionResult> Change(
        int id,
        CustomerReturnReceiptCommandDto input,
        Func<CustomerReturnReceiptCommandDto, Task> action) =>
        id != input.Id
            ? Task.FromResult<IActionResult>(BadRequest())
            : Run(async () => { await action(input); return NoContent(); });

    private async Task<IActionResult> Run(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
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
