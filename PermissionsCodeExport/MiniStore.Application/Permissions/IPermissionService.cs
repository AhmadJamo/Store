namespace MiniStore.Application.Permissions;

public interface IPermissionService
{
    Task<bool> CanAsync(
        string userId,
        string permission);
}