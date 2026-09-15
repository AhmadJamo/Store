namespace MiniStore.Domain.Entities;

public class TenantRolePermission
{
    public int TenantRoleId { get; private set; }
    public int PermissionId { get; private set; }
    public Permission Permission { get; private set; } = null!;

    private TenantRolePermission() { }

    public TenantRolePermission(int tenantRoleId, int permissionId)
    {
        if (tenantRoleId <= 0 || permissionId <= 0)
            throw new ArgumentException("A valid role and permission are required.");
        TenantRoleId = tenantRoleId;
        PermissionId = permissionId;
    }
}
