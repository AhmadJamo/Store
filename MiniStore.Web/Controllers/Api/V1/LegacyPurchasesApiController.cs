using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/legacy-purchases")]
public sealed class LegacyPurchasesApiController(
    PurchaseService purchases,
    PurchasePostingService posting,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("Purchases.View")]
    public async Task<ActionResult<List<PurchaseDto>>> GetAll() => Ok(await purchases.GetAllAsync());

    [HttpGet("{id:int}"), PermissionAuthorize("Purchases.View")]
    public async Task<ActionResult<LegacyPurchaseDetailsPageDto>> Get(int id)
    {
        var purchase = await purchases.GetByIdAsync(id);
        if (purchase is null) return NotFound();
        return Ok(new LegacyPurchaseDetailsPageDto
        {
            Purchase = purchase,
            IsPosted = await posting.IsPostedAsync(purchase.InvoiceNumber)
        });
    }

    [HttpPost("{id:int}/post"), ValidateAntiForgeryToken, PermissionAuthorize("Purchases.Create")]
    public async Task<IActionResult> Post(int id)
    {
        try
        {
            await posting.PostAsync(id);
            return NoContent();
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
