namespace MiniStore.Domain.Entities;

public class TenantUserRole
{
    public int TenantId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string RoleId { get; private set; } = string.Empty;
    private TenantUserRole() { }
    public TenantUserRole(int tenantId, string userId, string roleId) { if (tenantId <= 0 || string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId)) throw new ArgumentException("Tenant, user and role are required."); TenantId=tenantId; UserId=userId; RoleId=roleId; }
}
