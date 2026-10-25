using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Settings;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using Microsoft.Extensions.Caching.Memory;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Localization;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/settings")]
public sealed class SettingsApiController(
    GeneralSettingsService settings,
    IMemoryCache memoryCache,
    ITenantContext tenantContext) : ControllerBase
{
    [HttpGet("organization-identity")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<OrganizationIdentityDto>> GetOrganizationIdentity() =>
        Ok(await settings.GetOrganizationIdentityAsync());

    [HttpGet("preferences")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<GeneralSettingsPreferencesDto>> GetPreferences() =>
        Ok(await settings.GetPreferencesAsync());

    [HttpPut("preferences")]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<GeneralSettingsPreferencesDto>> UpdatePreferences(
        GeneralSettingsPreferencesDto dto)
    {
        await settings.UpdatePreferencesAsync(dto);
        memoryCache.Remove(DatabaseRequestCultureProvider.GetCacheKey(tenantContext.TenantId));
        return Ok(await settings.GetPreferencesAsync());
    }
}
