using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Security;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/users")]
public sealed class UsersApiController(
    ITenantUserAdministrationService users,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("Users.View")]
    public async Task<ActionResult<IReadOnlyList<TenantUserListItemDto>>> GetAll() =>
        Ok(await users.GetAllAsync());

    [HttpGet("form")]
    [PermissionAuthorize("Users.Create")]
    public async Task<ActionResult<TenantUserCreatePageDto>> GetCreateForm() =>
        Ok(await users.GetCreateAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Users.Create")]
    public async Task<ActionResult<TenantUserListItemDto>> Create(CreateTenantUserDto dto)
    {
        try
        {
            return StatusCode(StatusCodes.Status201Created, await users.CreateAsync(dto));
        }
        catch (TenantUserCreationException exception)
        {
            ModelState.AddModelError(
                exception.IdentityErrorCodes.Count > 0 ? nameof(dto.Password) : nameof(dto.UserName),
                exception.IdentityErrorCodes.Count > 0
                    ? localizer["The password does not meet the account security requirements."]
                    : localizer[exception.Message]);
            return Validation();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            var message = exception.Message.Contains("Users.Max", StringComparison.Ordinal)
                ? localizer["The user limit for the current plan has been reached."]
                : localizer[exception.Message];
            ModelState.AddModelError(string.Empty, message);
            return Validation();
        }
    }

    private ActionResult Validation() => InternalApiProblemFactory.Validation(
        HttpContext, ModelState, localizer["One or more validation errors occurred."]);
}
