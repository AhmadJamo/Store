using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/supplier-quotations")]
public sealed class SupplierQuotationsApiController(SupplierQuotationService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet("create-form"), PermissionAuthorize("SupplierQuotations.Create")]
    public async Task<ActionResult<SupplierQuotationCreatePageDto>> GetCreateForm(int sourcingEventId)
    { var page = await service.GetCreatePageAsync(sourcingEventId); return page.SourcingEvent is null ? NotFound() : Ok(page); }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SupplierQuotations.Create")]
    public async Task<IActionResult> Create(CreateSupplierQuotationDto input)
    { try { return Ok(new { id = await service.CreateAndSubmitAsync(input) }); } catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { return Validation(exception); } }

    [HttpGet("comparison/{sourcingEventId:int}"), PermissionAuthorize("SupplierQuotations.View")]
    public async Task<ActionResult<SupplierQuotationComparisonPageDto>> Comparison(int sourcingEventId)
    { try { return Ok(await service.GetComparisonAsync(sourcingEventId)); } catch (InvalidOperationException) { return NotFound(); } }

    [HttpPost("award"), ValidateAntiForgeryToken, PermissionAuthorize("SupplierQuotations.Award")]
    public async Task<IActionResult> Award(CreatePurchaseQuotationAwardDto input)
    { try { await service.AwardAsync(input); return NoContent(); } catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { return Validation(exception); } }

    private IActionResult Validation(Exception exception)
    { ModelState.AddModelError(string.Empty, localizer[exception.Message]); return InternalApiProblemFactory.Validation(HttpContext, ModelState, localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation); }
}
