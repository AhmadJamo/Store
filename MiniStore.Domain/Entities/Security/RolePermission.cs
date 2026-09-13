namespace MiniStore.Domain.Entities;

public class RolePermission
{
    public int Id { get; private set; }

    public string RoleId { get; private set; }

    public int PermissionId { get; private set; }

    public Permission Permission { get; private set; }

    private RolePermission()
    {
        RoleId = string.Empty;
        Permission = null!;
    }

    public RolePermission(
        string roleId,
        int permissionId)
    {
        if (string.IsNullOrWhiteSpace(roleId))
            throw new ArgumentException(
                "Role ID is required.");

        if (permissionId <= 0)
            throw new ArgumentException(
                "Permission ID must be greater than zero.");

        RoleId = roleId;
        PermissionId = permissionId;
        Permission = null!;
    }
}