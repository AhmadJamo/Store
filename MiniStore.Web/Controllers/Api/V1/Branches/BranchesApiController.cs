using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Settings;
using MiniStore.Application.Exceptions;
using MiniStore.Application.Dtos.Security;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1.Branches;

[ApiController]
[Authorize]
[Route("api/v1/branches")]
public sealed class BranchesApiController(
    InventoryAccessService service,
    BranchService branches,
    BranchAccessService access,
    IAuthorizationService authorization,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("Branches.View")]
    public async Task<ActionResult<BranchWorkspaceDto>> Get(DateTime? fromUtc = null, DateTime? toUtc = null)
    {
        var canViewOperations = (await authorization.AuthorizeAsync(User, PermissionAuthorizeAttribute.PolicyPrefix + "Branches.ViewOperations")).Succeeded;
        return Ok(await service.GetBranchWorkspaceAsync(fromUtc, toUtc, canViewOperations));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Branches.Create")]
    public Task<ActionResult<BranchRowDto>> Create(BranchCommandDto command) =>
        Execute(() => branches.CreateAsync(command.Code, command.Name));

    [HttpPut("{id:int}")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Branches.Edit")]
    public Task<ActionResult<BranchRowDto>> Update(int id, BranchCommandDto command) =>
        Execute(() => branches.UpdateAsync(id, command.Code, command.Name));

    [HttpPut("{id:int}/status")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Branches.SetActive")]
    public Task<ActionResult<BranchRowDto>> SetActive(int id, BranchStatusCommandDto command) =>
        Execute(() => branches.SetActiveAsync(id, command.IsActive));
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

    private async Task<ActionResult<BranchRowDto>> Execute(Func<Task<BranchRowDto>> operation)
    {
        try { return Ok(await operation()); }
        catch (InvalidOperationException exception) when (exception is ResourceNotFoundException or BusinessConflictException or DuplicateResourceException or OptimisticConcurrencyException) { return InternalApiProblemFactory.ApplicationProblem(HttpContext, exception, localizer[exception.Message]); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState, localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
        }
    }
}
