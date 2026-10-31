using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Security;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/roles")]
public sealed class RolesApiController(
    TenantRoleService roles,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("Roles.View")]
    public async Task<ActionResult<IReadOnlyList<TenantRoleListItemDto>>> GetAll() =>
        Ok(await roles.GetAllAsync());

    [HttpGet("form")]
    [PermissionAuthorize("Roles.Create")]
    public async Task<ActionResult<TenantRoleFormDto>> GetCreateForm() =>
        Ok(Map(await roles.GetCreateAsync()));

    [HttpGet("{id:int}")]
    [PermissionAuthorize("Roles.Edit")]
    public async Task<ActionResult<TenantRoleFormDto>> GetEditForm(int id)
    {
        var role = await roles.GetEditAsync(id);
        if (role is null) return NotFoundProblem();
        if (role.IsSystem) return Validation("System roles cannot be edited.");
        return Ok(Map(role));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Roles.Create")]
    public async Task<ActionResult<TenantRoleFormDto>> Create(SaveTenantRoleDto dto)
    {
        try
        {
            await roles.CreateAsync(ToEdit(dto));
            var created = (await roles.GetAllAsync()).Single(role =>
                string.Equals(role.Name, dto.Name.Trim(), StringComparison.OrdinalIgnoreCase));
            return StatusCode(StatusCodes.Status201Created, Map((await roles.GetEditAsync(created.Id))!));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        { return Validation(exception.Message); }
    }

    [HttpPut("{id:int}")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Roles.Edit")]
    public async Task<ActionResult<TenantRoleFormDto>> Update(int id, SaveTenantRoleDto dto)
    {
        if (id != dto.Id) return IdentifierMismatch();
        try
        {
            await roles.UpdateAsync(ToEdit(dto));
            return Ok(Map((await roles.GetEditAsync(id))!));
        }
        catch (RoleConcurrencyException exception)
        {
            return InternalApiProblemFactory.Create(HttpContext, StatusCodes.Status409Conflict,
                localizer["Role settings changed since you opened this page."],
                InternalApiProblemCodes.Concurrency, localizer[exception.Message]);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        { return Validation(exception.Message); }
    }

    [HttpDelete("{id:int}")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Roles.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await roles.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        { return Validation(exception.Message); }
    }

    private ActionResult IdentifierMismatch()
    {
        ModelState.AddModelError(nameof(SaveTenantRoleDto.Id), localizer["The route and payload identifiers do not match."]);
        return InternalApiProblemFactory.Validation(HttpContext, ModelState,
            localizer["One or more validation errors occurred."]);
    }

    private ActionResult Validation(string message)
    {
        ModelState.AddModelError(string.Empty, localizer[message]);
        return InternalApiProblemFactory.Validation(HttpContext, ModelState,
            localizer["One or more validation errors occurred."]);
    }

    private ObjectResult NotFoundProblem() => InternalApiProblemFactory.Create(
        HttpContext, StatusCodes.Status404NotFound, localizer["The requested role was not found."],
        InternalApiProblemCodes.NotFound);

    private static TenantRoleEditDto ToEdit(SaveTenantRoleDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        SelectedPermissionIds = dto.SelectedPermissionIds,
        RowVersion = dto.RowVersion
    };

    private static TenantRoleFormDto Map(TenantRoleEditDto dto) => new(
        dto.Id,
        dto.Name,
        dto.Description,
        dto.IsSystem,
        dto.SelectedPermissionIds,
        dto.Permissions.Select(permission => new TenantPermissionOptionDto(
            permission.Id, permission.Name, permission.Group)).ToList(),
        dto.RowVersion);
}
