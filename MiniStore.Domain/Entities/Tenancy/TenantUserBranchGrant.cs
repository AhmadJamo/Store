namespace MiniStore.Domain.Entities;

public sealed class TenantUserBranchGrant
{
    public int TenantId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public int BranchId { get; private set; }
    public string GrantedByUserId { get; private set; } = string.Empty;
    public DateTime GrantedAtUtc { get; private set; } = DateTime.UtcNow;

    private TenantUserBranchGrant() { }

    public TenantUserBranchGrant(int tenantId, string userId, int branchId, string grantedByUserId)
    {
        if (tenantId <= 0 || branchId <= 0 || string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(grantedByUserId))
            throw new ArgumentException("Tenant, user, branch and granting user are required.");
        TenantId = tenantId;
        UserId = userId;
        BranchId = branchId;
        GrantedByUserId = grantedByUserId;
    }
}
