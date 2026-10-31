namespace MiniStore.Application.Permissions;

public static class AdministrationPermissions
{
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        "Administration.Access",
        "Users.Create",
        "Users.Edit",
        "Users.Delete",
        "Roles.Create",
        "Roles.Edit",
        "Roles.Delete"
    };

    // Delegating identity/permission mutations would allow users to grant themselves access.
    public static bool RequiresAdmin(string permission) => All.Contains(permission);
}
