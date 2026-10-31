using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Api;
using MiniStore.Web.Middleware.Api;
using MiniStore.Web.Services;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthenticationApiController(
    TenantAccountAuthenticationService authentication,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("login")]
    [ProducesResponseType<TenantLoginResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<TenantLoginResultDto>> Login(TenantLoginRequestDto request)
    {
        var outcome = await authentication.SignInAsync(
            request.UserName,
            request.Password,
            request.CompanyCode);

        if (outcome.Status == TenantLoginStatus.InvalidCredentials)
            return InternalApiProblemFactory.Create(
                HttpContext,
                StatusCodes.Status401Unauthorized,
                localizer["Invalid username or password."],
                InternalApiProblemCodes.Unauthorized);

        if (outcome.Status == TenantLoginStatus.CompanyAccessDenied)
            return InternalApiProblemFactory.Create(
                HttpContext,
                StatusCodes.Status403Forbidden,
                localizer[string.IsNullOrWhiteSpace(request.CompanyCode)
                    ? "Your account is not assigned to an active company."
                    : "The company code is invalid or your account is not assigned to it."],
                InternalApiProblemCodes.Forbidden);

        return Ok(new TenantLoginResultDto(outcome.RequiresCompanyOnboarding));
    }

    [Authorize]
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        await authentication.SignOutAsync();
        return NoContent();
    }
}
