namespace MiniStore.Domain.Entities;

public class AuditLog
{
    public long Id { get; private set; }
    public string EntityName { get; private set; }
    public string EntityId { get; private set; }
    public string Action { get; private set; }
    public string UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AuditLog()
    {
        EntityName = string.Empty;
        EntityId = string.Empty;
        Action = string.Empty;
        UserId = string.Empty;
    }

    public AuditLog(
        string entityName,
        string entityId,
        string action,
        string userId)
    {
        EntityName = entityName;
        EntityId = entityId;
        Action = action;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }
}
