using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/purchase-sourcing")]
public sealed class PurchaseSourcingApiController(PurchaseSourcingService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("PurchaseSourcing.View")]
    public async Task<ActionResult<List<PurchaseSourcingDto>>> GetAll(PurchaseSourcingStatus? status, string? search) =>
        Ok(await service.GetAllAsync(status, search));

    [HttpGet("{id:int}"), PermissionAuthorize("PurchaseSourcing.View")]
    public async Task<ActionResult<PurchaseSourcingDto>> Get(int id)
    { var item = await service.GetAsync(id); return item is null ? NotFound() : Ok(item); }

    [HttpGet("create-form"), PermissionAuthorize("PurchaseSourcing.Create")]
    public async Task<ActionResult<PurchaseSourcingCreatePageDto>> GetCreateForm(int purchaseRequestId)
    { var page = await service.GetCreatePageAsync(purchaseRequestId); return page.Request is null ? NotFound() : Ok(page); }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("PurchaseSourcing.Create")]
    public async Task<IActionResult> Create(CreatePurchaseSourcingDto input)
    {
        try { input.Attachments = []; return Ok(new { id = await service.CreateAndSendAsync(input) }); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { return Validation(exception); }
    }

    private IActionResult Validation(Exception exception)
    { ModelState.AddModelError(string.Empty, localizer[exception.Message]); return InternalApiProblemFactory.Validation(HttpContext, ModelState, localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation); }
}
