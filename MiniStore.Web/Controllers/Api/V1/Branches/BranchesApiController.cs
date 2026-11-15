using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Dtos.Settings;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers.Api.V1.Branches;

[ApiController]
[Authorize]
[Route("api/v1/branches")]
public sealed class BranchesApiController(InventoryAccessService service) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("Administration.Access")]
    public async Task<ActionResult<BranchWorkspaceDto>> Get() =>
        Ok(await service.GetBranchWorkspaceAsync());
}
