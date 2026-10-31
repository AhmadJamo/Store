using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Api;
using MiniStore.Application.Services.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/session")]
public sealed class SessionController(
    UserManager<IdentityUser> users,
    InternalApiSessionService sessions) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<InternalApiSessionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<InternalApiSessionDto>> Get(CancellationToken cancellationToken)
    {
        var userId = users.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        return Ok(await sessions.GetAsync(
            userId,
            User.Identity?.Name,
            User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
            CultureInfo.CurrentUICulture.Name,
            cancellationToken));
    }
}
