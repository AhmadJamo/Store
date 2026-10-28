using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/supplier-payments")]
public sealed class SupplierPaymentsApiController(
    SupplierPaymentService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("SupplierPayments.View")]
    public async Task<ActionResult<List<SupplierPaymentDto>>> GetAll(string? search) => Ok(await service.GetAllAsync(search));

    [HttpGet("{id:int}"), PermissionAuthorize("SupplierPayments.View")]
    public async Task<ActionResult<SupplierPaymentDto>> Get(int id)
    {
        var payment = await service.GetAsync(id);
        return payment is null ? NotFound() : Ok(payment);
    }

    [HttpGet("create-form"), PermissionAuthorize("SupplierPayments.Create")]
    public async Task<IActionResult> GetCreateForm(int vendorBillId)
    {
        var page = await service.GetCreatePageAsync(vendorBillId);
        return string.IsNullOrWhiteSpace(page.SupplierName) ? NotFound() : Ok(page);
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SupplierPayments.Create")]
    public async Task<IActionResult> Create(CreateSupplierPaymentDto input)
    {
        try { return Ok(new { id = await service.CreateAndPostAsync(input) }); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
        }
    }
}
