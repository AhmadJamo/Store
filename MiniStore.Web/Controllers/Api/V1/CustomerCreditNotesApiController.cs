using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/customer-credit-notes")]
public sealed class CustomerCreditNotesApiController(
    CustomerCreditNoteService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("CustomerCreditNotes.View")]
    public async Task<ActionResult<List<CustomerCreditNoteSummaryDto>>> GetAll() =>
        Ok(await service.GetAllAsync());

    [HttpGet("{id:int}"), PermissionAuthorize("CustomerCreditNotes.View")]
    public async Task<ActionResult<CustomerCreditNoteDetailsDto>> Get(int id)
    {
        var value = await service.GetAsync(id);
        return value is null ? NotFound() : Ok(value);
    }

    [HttpGet("create-form"), PermissionAuthorize("CustomerCreditNotes.Create")]
    public async Task<ActionResult<CustomerCreditNoteCreatePageDto>> GetCreateForm(int customerInvoiceId)
    {
        var value = await service.GetCreatePageAsync(customerInvoiceId);
        return value.InvoiceId == 0 ? NotFound() : Ok(value);
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerCreditNotes.Create")]
    public async Task<IActionResult> Create(CreateCustomerCreditNoteDto input)
    {
        try { return Ok(new { id = await service.CreateAndPostAsync(input) }); }
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
