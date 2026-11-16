using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Dtos.Settings;
using MiniStore.Application.Dtos.Security;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers.Api.V1.Branches;

[ApiController]
[Authorize]
[Route("api/v1/branches")]
public sealed class BranchesApiController(
    InventoryAccessService service,
    BranchAccessService access) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("Branches.View")]
    public async Task<ActionResult<BranchWorkspaceDto>> Get() =>
        Ok(await service.GetBranchWorkspaceAsync());

    [HttpGet("access-management")]
    [PermissionAuthorize("Branches.ManageAccess")]
    public async Task<ActionResult<BranchAccessManagementDto>> GetAccessManagement() =>
        Ok(await access.GetManagementAsync());

    [HttpPut("access-management")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Branches.ManageAccess")]
    public async Task<ActionResult<BranchAccessManagementDto>> ReplaceUserGrants(
        ReplaceUserBranchGrantsDto dto) =>
        Ok(await access.ReplaceUserGrantsAsync(dto));
}
