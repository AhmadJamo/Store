using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Saas;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;
using MiniStore.Web.Services;

namespace MiniStore.Web.Controllers.Api.V1;

public record PlatformLoginRequest(string Username, string Password);
public record PlatformSessionDto(
    string Username,
    string Role,
    bool CanManage,
    bool CanBill,
    bool CanViewAudit,
    bool CanExportAudit,
    bool CanViewSensitiveAudit,
    bool CanManageRetention);
public record PlatformOverviewDto(PlatformSessionDto Session, IReadOnlyList<PlanCardDto> Plans);

[ApiController]
[Route("api/v1/platform")]
public sealed class PlatformSessionApiController(
    PlatformAccountAuthenticationService authentication,
    PlatformSaasService saas,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("login")]
    public async Task<ActionResult<PlatformSessionDto>> Login(PlatformLoginRequest request)
    {
        var result = await authentication.AuthenticateAsync(
            request.Username,
            request.Password,
            HttpContext.RequestAborted);
        if (!result.Succeeded)
            return InternalApiProblemFactory.Create(
                HttpContext,
                StatusCodes.Status401Unauthorized,
                localizer["Invalid platform credentials."],
                InternalApiProblemCodes.Unauthorized);

        await HttpContext.SignInAsync(PlatformAuthentication.Scheme, result.Principal!);
        return Ok(Map(result.Principal!));
    }

    [Authorize(AuthenticationSchemes = PlatformAuthentication.Scheme, Policy = PlatformAuthentication.Policy)]
    [HttpGet("session")]
    public ActionResult<PlatformSessionDto> Session() => Ok(Map(User));

    [Authorize(AuthenticationSchemes = PlatformAuthentication.Scheme, Policy = PlatformAuthentication.Policy)]
    [HttpGet("overview")]
    public async Task<ActionResult<PlatformOverviewDto>> Overview() =>
        Ok(new PlatformOverviewDto(Map(User), await saas.GetPlansAsync()));

    [Authorize(AuthenticationSchemes = PlatformAuthentication.Scheme, Policy = PlatformAuthentication.Policy)]
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(PlatformAuthentication.Scheme);
        return NoContent();
    }

    private static PlatformSessionDto Map(ClaimsPrincipal principal)
    {
        var role = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        var owner = role == "Owner";
        var administrator = role == "Administrator";
        var billing = role == "Billing";
        return new PlatformSessionDto(
            principal.Identity?.Name ?? string.Empty,
            role,
            owner || administrator,
            owner || administrator || billing,
            owner || administrator || billing,
            owner || administrator,
            owner,
            owner);
    }
}
