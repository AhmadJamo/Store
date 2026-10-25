using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Dtos.Analytics;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/analytics-center")]
public sealed class AnalyticsCenterApiController(
    AnalyticsCenterService analytics,
    IAuthorizationService authorization) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("AnalyticsCenter.View")]
    public async Task<ActionResult<AnalyticsCenterPageDto>> Get([FromQuery] AnalyticsCenterQueryDto query) =>
        Ok(await analytics.GetAsync(query));

    [HttpGet("builder")]
    [PermissionAuthorize("AnalyticsCenter.Customize")]
    public async Task<ActionResult<AnalyticsWorkspaceCreatePageDto>> GetBuilder() =>
        Ok(await analytics.GetCreatePageAsync());

    [HttpPost("workspaces")]
    [PermissionAuthorize("AnalyticsCenter.Customize")]
    public async Task<ActionResult<object>> CreateWorkspace(CreateAnalyticsWorkspaceDto dto)
    {
        var canManage = (await authorization.AuthorizeAsync(
            User, null, "Permission:AnalyticsCenter.Manage")).Succeeded;
        var id = await analytics.CreateAsync(dto, canManage);
        return CreatedAtAction(nameof(Get), new { workspaceId = id }, new { id });
    }

    [HttpGet("workspaces/{workspaceId:int}/access")]
    [PermissionAuthorize("AnalyticsCenter.Customize")]
    public async Task<ActionResult<AnalyticsWorkspaceAccessDto>> GetAccess(int workspaceId) =>
        Ok(await analytics.GetAccessAsync(workspaceId, await CanManageAsync()));

    [HttpPost("workspaces/{workspaceId:int}/delegations")]
    [PermissionAuthorize("AnalyticsCenter.Customize")]
    public async Task<IActionResult> CreateDelegation(int workspaceId, CreateAnalyticsDelegationDto dto)
    {
        await analytics.CreateDelegationAsync(workspaceId, dto, await CanManageAsync());
        return NoContent();
    }

    [HttpPost("workspaces/{workspaceId:int}/delegations/revoke")]
    [PermissionAuthorize("AnalyticsCenter.Customize")]
    public async Task<IActionResult> RevokeDelegation(int workspaceId, RevokeAnalyticsDelegationDto dto)
    {
        await analytics.RevokeDelegationAsync(workspaceId, dto, await CanManageAsync());
        return NoContent();
    }

    private async Task<bool> CanManageAsync() => (await authorization.AuthorizeAsync(
        User, null, "Permission:AnalyticsCenter.Manage")).Succeeded;
}
