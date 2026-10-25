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
}
