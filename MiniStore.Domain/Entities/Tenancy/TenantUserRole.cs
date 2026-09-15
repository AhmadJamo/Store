namespace MiniStore.Domain.Entities;

public class TenantUserRole
{
    public int TenantId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public int TenantRoleId { get; private set; }
    private TenantUserRole() { }
    public TenantUserRole(int tenantId, string userId, int tenantRoleId)
    {
        if (tenantId <= 0 || string.IsNullOrWhiteSpace(userId) || tenantRoleId <= 0)
            throw new ArgumentException("Tenant, user and role are required.");
        TenantId = tenantId;
        UserId = userId;
        TenantRoleId = tenantRoleId;
    }
}
