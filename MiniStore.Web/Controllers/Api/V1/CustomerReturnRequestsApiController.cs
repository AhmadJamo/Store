using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/customer-return-requests")]
public sealed class CustomerReturnRequestsApiController(CustomerReturnRequestService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("CustomerReturnRequests.View")]
    public async Task<ActionResult<List<CustomerReturnRequestSummaryDto>>> GetAll(string? search = null) =>
        Ok(await service.GetAllAsync(search));
    [HttpGet("{id:int}"), PermissionAuthorize("CustomerReturnRequests.View")]
    public async Task<ActionResult<CustomerReturnRequestDetailsDto>> Get(int id)
    { var value = await service.GetAsync(id); return value is null ? NotFound() : Ok(value); }
    [HttpGet("create-form"), PermissionAuthorize("CustomerReturnRequests.Create")]
    public async Task<ActionResult<CustomerReturnRequestCreatePageDto>> GetCreateForm(int salesDeliveryId)
    { var value = await service.GetCreatePageAsync(salesDeliveryId); return value is null ? NotFound() : Ok(value); }
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerReturnRequests.Create")]
    public Task<IActionResult> Create(CreateCustomerReturnRequestDto input) =>
        Run(async () => Ok(new { id = await service.CreateAsync(input) }));
    [HttpPost("{id:int}/submit"), ValidateAntiForgeryToken, PermissionAuthorize("CustomerReturnRequests.Submit")]
    public Task<IActionResult> Submit(int id, CustomerReturnRequestCommandDto input) => Change(id, input, service.SubmitAsync);
    [HttpPost("{id:int}/approve"), ValidateAntiForgeryToken, PermissionAuthorize("CustomerReturnRequests.Approve")]
    public Task<IActionResult> Approve(int id, CustomerReturnRequestCommandDto input) => Change(id, input, service.ApproveAsync);
    [HttpPost("{id:int}/reject"), ValidateAntiForgeryToken, PermissionAuthorize("CustomerReturnRequests.Reject")]
    public Task<IActionResult> Reject(int id, CustomerReturnRequestCommandDto input) => Change(id, input, service.RejectAsync);
    [HttpPost("{id:int}/cancel"), ValidateAntiForgeryToken, PermissionAuthorize("CustomerReturnRequests.Cancel")]
    public Task<IActionResult> Cancel(int id, CustomerReturnRequestCommandDto input) => Change(id, input, service.CancelAsync);
    private Task<IActionResult> Change(int id, CustomerReturnRequestCommandDto input,
        Func<CustomerReturnRequestCommandDto, Task> action) =>
        id != input.Id ? Task.FromResult<IActionResult>(BadRequest()) :
        Run(async () => { await action(input); return NoContent(); });
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
