using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Saas;
using MiniStore.Domain.Entities;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

public record CreateSubscriptionCheckoutRequest(int PlanId, BillingCycle Cycle, string? PromotionCode);

[ApiController]
[Authorize]
[Route("api/v1/subscription")]
public sealed class SubscriptionApiController(
    TenantSubscriptionPageService page,
    IBillingCheckoutService checkout,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TenantSubscriptionPageDto>> Get() => Ok(await page.GetAsync());

    [HttpPost("checkout")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<CheckoutQuoteDto>> CreateCheckout(CreateSubscriptionCheckoutRequest request)
    {
        try
        {
            return Ok(await checkout.CreateAsync(request.PlanId, request.Cycle, request.PromotionCode));
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
