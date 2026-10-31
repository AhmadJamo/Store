using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/customer-payments")]
public sealed class CustomerPaymentsApiController(CustomerPaymentService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("CustomerPayments.View")]
    public async Task<ActionResult<List<CustomerPaymentDto>>> GetAll(string? search = null) =>
        Ok(await service.GetAllAsync(search));

    [HttpGet("{id:int}"), PermissionAuthorize("CustomerPayments.View")]
    public async Task<ActionResult<CustomerPaymentDto>> Get(int id)
    { var value = await service.GetAsync(id); return value is null ? NotFound() : Ok(value); }

    [HttpGet("create-form"), PermissionAuthorize("CustomerPayments.Create")]
    public async Task<ActionResult<CustomerPaymentCreatePageDto>> GetCreateForm(int customerInvoiceId)
    {
        var page = await service.GetCreatePageAsync(customerInvoiceId);
        return string.IsNullOrWhiteSpace(page.CustomerName) ? NotFound() : Ok(page);
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerPayments.Create")]
    public Task<IActionResult> Create(CreateCustomerPaymentDto input) =>
        Run(async () => Ok(new { id = await service.CreateAndPostAsync(input) }));

    [HttpPost("{id:int}/reverse"), ValidateAntiForgeryToken, PermissionAuthorize("CustomerPayments.Reverse")]
    public Task<IActionResult> Reverse(int id, ReverseCustomerPaymentDto input) =>
        id != input.Id ? Task.FromResult<IActionResult>(BadRequest()) :
        Run(async () => { await service.ReverseAsync(input); return NoContent(); });

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
