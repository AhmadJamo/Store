using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/legacy-purchase-returns")]
public sealed class LegacyPurchaseReturnsApiController(
    PurchaseReturnService returns,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("PurchaseReturns.View")]
    public async Task<ActionResult<List<PurchaseReturnDto>>> GetAll() => Ok(await returns.GetAllAsync());

    [HttpGet("{id:int}"), PermissionAuthorize("PurchaseReturns.View")]
    public async Task<ActionResult<PurchaseReturnDto>> Get(int id)
    {
        var value = await returns.GetByIdAsync(id);
        return value is null ? NotFound() : Ok(value);
    }

    [HttpGet("create-form"), PermissionAuthorize("PurchaseReturns.Create")]
    public async Task<IActionResult> GetCreateForm(int purchaseId)
    {
        try
        {
            var page = await returns.GetCreatePageAsync(purchaseId);
            return page is null ? NotFound() : Ok(page);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(
                HttpContext,
                ModelState,
                localizer["One or more validation errors occurred."],
                InternalApiProblemCodes.Validation);
        }
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("PurchaseReturns.Create")]
    public async Task<IActionResult> Create(CreatePurchaseReturnDto input)
    {
        try
        {
            return Ok(new { id = await returns.CreateAsync(input) });
        }
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
