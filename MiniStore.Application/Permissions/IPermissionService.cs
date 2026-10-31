namespace MiniStore.Application.Permissions;

public interface IPermissionService
{
    Task<bool> CanAsync(
        string userId,
        string permission);

    Task<IReadOnlyList<string>> GetGrantedAsync(string userId);
}
