namespace MiniStore.Domain.Entities;

public class TenantMembership
{
    public int TenantId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public bool IsOwner { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private TenantMembership()
    {
    }

    public TenantMembership(int tenantId, string userId, bool isOwner = false)
    {
        if (tenantId <= 0) throw new ArgumentException("Tenant is required.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        TenantId = tenantId;
        UserId = userId;
        IsOwner = isOwner;
    }

    public void SetActive(bool isActive) => IsActive = isActive;
    public void SetOwner(bool isOwner) => IsOwner = isOwner;
}
