using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Api;
using MiniStore.Application.Saas;
using MiniStore.Web.Middleware.Api;
using MiniStore.Web.Services;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Route("api/v1/registration")]
public sealed class RegistrationApiController(
    PlatformSaasService saas,
    TenantCompanyRegistrationService registration,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType<CompanyRegistrationPageDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CompanyRegistrationPageDto>> Get() =>
        Ok(new CompanyRegistrationPageDto(await saas.GetPublicPlansAsync()));

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("registration")]
    [ProducesResponseType<CompanyRegistrationResultDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<CompanyRegistrationResultDto>> Register(RegisterCompanyRequestDto request)
    {
        try
        {
            await registration.RegisterAndSignInAsync(new RegisterCompanyCommand(
                request.OwnerUsername,
                request.OwnerEmail,
                request.Password,
                request.CompanyName,
                request.CompanySlug,
                request.PlanId));
            return StatusCode(
                StatusCodes.Status201Created,
                new CompanyRegistrationResultDto(true));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            var (field, message) = MapRegistrationError(exception.Message);
            ModelState.AddModelError(field, localizer[message]);
            return InternalApiProblemFactory.Validation(
                HttpContext,
                ModelState,
                localizer["One or more validation errors occurred."]);
        }
    }

    private static (string Field, string Message) MapRegistrationError(string message)
    {
        if (message.Contains("username", StringComparison.OrdinalIgnoreCase))
            return (nameof(RegisterCompanyRequestDto.OwnerUsername), message);
        if (message.Contains("email", StringComparison.OrdinalIgnoreCase))
            return (nameof(RegisterCompanyRequestDto.OwnerEmail), message);
        if (message.Contains("address", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("slug", StringComparison.OrdinalIgnoreCase))
            return (nameof(RegisterCompanyRequestDto.CompanySlug), message);
        if (message.Contains("Tenant name", StringComparison.OrdinalIgnoreCase))
            return (nameof(RegisterCompanyRequestDto.CompanyName), message);
        if (message.Contains("plan", StringComparison.OrdinalIgnoreCase))
            return (nameof(RegisterCompanyRequestDto.PlanId), message);

        return (
            nameof(RegisterCompanyRequestDto.Password),
            "The password does not meet the account security requirements.");
    }
}
