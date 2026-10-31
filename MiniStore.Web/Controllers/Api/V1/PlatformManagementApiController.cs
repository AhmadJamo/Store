using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Saas;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

public record PlatformManagementPageDto(
    IReadOnlyList<PlanCardDto> Plans,
    IReadOnlyList<CompanySubscriptionDto> Companies,
    IReadOnlyList<PlatformPromotionDto> Promotions,
    IReadOnlyList<PendingCheckoutDto> PendingPayments);
public record ConfirmPlatformPaymentRequest(string ProviderReference);

[ApiController]
[Route("api/v1/platform/management")]
[Authorize(AuthenticationSchemes = PlatformAuthentication.Scheme, Policy = PlatformAuthentication.Policy)]
public sealed class PlatformManagementApiController(
    PlatformSaasService service,
    IBillingCheckoutService checkout,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PlatformManagementPageDto>> Get() => Ok(new PlatformManagementPageDto(
        await service.GetPlansAsync(),
        await service.GetCompaniesAsync(),
        await service.GetPromotionDtosAsync(),
        await checkout.GetPendingAsync()));

    [HttpPut("companies/{tenantId:int}")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PlatformAuthentication.ManagementPolicy)]
    public Task<IActionResult> UpdateCompany(int tenantId, UpdateCompanySubscriptionDto request) =>
        Run(async () =>
        {
            if (tenantId != request.TenantId)
                throw new ArgumentException("Route and payload identifiers do not match.");
            await service.UpdateCompanySubscriptionAsync(request);
        });

    [HttpPost("plans")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PlatformAuthentication.ManagementPolicy)]
    public Task<IActionResult> CreatePlan(CreatePlanDto request) =>
        Run(() => service.CreatePlanAsync(request));

    [HttpPost("promotions")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PlatformAuthentication.ManagementPolicy)]
    public Task<IActionResult> CreatePromotion(CreatePromotionCodeDto request) =>
        Run(() => service.CreatePromotionCodeAsync(request));

    [HttpPost("payments/{sessionId:guid}/confirm")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = PlatformAuthentication.BillingPolicy)]
    public Task<IActionResult> ConfirmPayment(Guid sessionId, ConfirmPlatformPaymentRequest request) =>
        Run(() => checkout.CompleteAsync(sessionId, request.ProviderReference));

    private async Task<IActionResult> Run(Func<Task> operation)
    {
        try
        {
            await operation();
            return NoContent();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(
                HttpContext,
                ModelState,
                localizer["One or more validation errors occurred."]);
        }
    }
}
