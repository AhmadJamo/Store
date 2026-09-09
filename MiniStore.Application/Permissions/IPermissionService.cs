using System.Security.Claims;

namespace MiniStore.Application.Permissions;

public interface IPermissionService
{
    Task<bool> CanAsync(
        ClaimsPrincipal user,
        string permission);
}