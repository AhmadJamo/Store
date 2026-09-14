namespace MiniStore.Application.Permissions;

public static class AdministrationPermissions
{
    // Delegating identity/permission mutations would allow users to grant themselves access.
    public static bool RequiresAdmin(string permission) => permission is
        "Administration.Access" or
        "Users.Create" or "Users.Edit" or "Users.Delete" or
        "Roles.Create" or "Roles.Edit" or "Roles.Delete";
}
