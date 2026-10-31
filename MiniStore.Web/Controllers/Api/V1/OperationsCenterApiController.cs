using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Dtos.Shared;
using MiniStore.Application.Services;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/operations-center")]
public sealed class OperationsCenterApiController(OperationsCenterService operations) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<OperationsCenterDto>> Get() => Ok(await operations.GetAsync());
}
