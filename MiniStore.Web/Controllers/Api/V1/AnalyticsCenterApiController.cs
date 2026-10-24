using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Dtos.Analytics;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/analytics-center")]
public sealed class AnalyticsCenterApiController(AnalyticsCenterService analytics) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("AnalyticsCenter.View")]
    public async Task<ActionResult<AnalyticsCenterPageDto>> Get([FromQuery] AnalyticsCenterQueryDto query) =>
        Ok(await analytics.GetAsync(query));
}
