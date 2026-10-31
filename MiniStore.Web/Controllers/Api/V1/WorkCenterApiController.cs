using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Dtos.Shared;
using MiniStore.Application.Services;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/work-center")]
public sealed class WorkCenterApiController(WorkCenterService workCenter) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<WorkCenterDto>> Get() => Ok(await workCenter.GetAsync());
}
