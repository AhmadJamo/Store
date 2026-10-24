using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/security")]
public sealed class SecurityController(IAntiforgery antiforgery) : ControllerBase
{
    [HttpGet("csrf")]
    [ProducesResponseType<AntiforgeryTokenDto>(StatusCodes.Status200OK)]
    public ActionResult<AntiforgeryTokenDto> GetCsrfToken()
    {
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);
        return Ok(new AntiforgeryTokenDto(
            tokens.RequestToken ?? throw new InvalidOperationException("An antiforgery token could not be created."),
            tokens.HeaderName ?? "X-CSRF-TOKEN"));
    }

    [HttpPost("mutation-probe")]
    [ProducesResponseType<MutationProbeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<MutationProbeDto> MutationProbe() =>
        Ok(new MutationProbeDto(true, HttpContext.TraceIdentifier));
}
