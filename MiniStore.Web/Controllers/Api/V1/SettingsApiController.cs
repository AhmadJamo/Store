using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Settings;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/settings")]
public sealed class SettingsApiController(GeneralSettingsService settings) : ControllerBase
{
    [HttpGet("organization-identity")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<OrganizationIdentityDto>> GetOrganizationIdentity() =>
        Ok(await settings.GetOrganizationIdentityAsync());

    [HttpGet("preferences")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<GeneralSettingsPreferencesDto>> GetPreferences() =>
        Ok(await settings.GetPreferencesAsync());
}
