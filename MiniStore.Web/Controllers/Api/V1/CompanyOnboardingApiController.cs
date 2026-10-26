using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Saas;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[PermissionAuthorize("Administration.Access")]
[Route("api/v1/company-onboarding")]
public sealed class CompanyOnboardingApiController(
    ICompanyOnboardingService onboarding,
    ITenantContext tenantContext,
    UserManager<IdentityUser> users,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<CompanyOnboardingDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CompanyOnboardingDto>> Get() =>
        Ok(await onboarding.GetAsync(RequireTenant()));

    [HttpPost("complete")]
    [ValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Complete(CompleteCompanyOnboardingCommand command)
    {
        try
        {
            await onboarding.CompleteAsync(RequireTenant(), RequireUser(), command);
            return NoContent();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return Validation(exception.Message);
        }
    }

    [HttpPost("skip")]
    [ValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Skip()
    {
        try
        {
            await onboarding.SkipAsync(RequireTenant(), RequireUser());
            return NoContent();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return Validation(exception.Message);
        }
    }

    private BadRequestObjectResult Validation(string message)
    {
        ModelState.AddModelError(string.Empty, localizer[message]);
        return InternalApiProblemFactory.Validation(
            HttpContext,
            ModelState,
            localizer["One or more validation errors occurred."]);
    }

    private int RequireTenant() => tenantContext.TenantId
        ?? throw new InvalidOperationException("An active company is required.");

    private string RequireUser() => users.GetUserId(User)
        ?? throw new InvalidOperationException("A signed-in user is required.");
}
