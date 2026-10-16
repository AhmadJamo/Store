namespace MiniStore.Domain.Entities;

public sealed class AuditLegalHold
{
    private AuditLegalHold()
    {
        ReasonRedacted = CreatedBy = string.Empty;
    }

    public long Id { get; private set; }
    public Guid HoldId { get; private set; }
    public AuditEventScope Scope { get; private set; }
    public int? TenantId { get; private set; }
    public DateTime FromUtc { get; private set; }
    public DateTime ToUtc { get; private set; }
    public string ReasonRedacted { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public bool IsActive { get; private set; }
    public string? ReleasedBy { get; private set; }
    public string? ReleaseReasonRedacted { get; private set; }
    public DateTime? ReleasedAtUtc { get; private set; }

    public AuditLegalHold(Guid holdId, AuditEventScope scope, int? tenantId, DateTime fromUtc, DateTime toUtc,
        string reasonRedacted, string createdBy)
    {
        if (holdId == Guid.Empty) throw new ArgumentException("Legal hold identity is required.");
        if (!Enum.IsDefined(scope)) throw new ArgumentException("Legal hold scope is invalid.");
        if ((scope == AuditEventScope.Tenant) != tenantId.HasValue || tenantId <= 0)
            throw new ArgumentException("Tenant legal holds require one tenant and platform/system holds cannot carry a tenant.");
        if (fromUtc.Kind != DateTimeKind.Utc || toUtc.Kind != DateTimeKind.Utc || toUtc < fromUtc)
            throw new ArgumentException("Legal hold dates must be a valid UTC range.");

        HoldId = holdId;
        Scope = scope;
        TenantId = tenantId;
        FromUtc = fromUtc;
        ToUtc = toUtc;
        ReasonRedacted = Required(reasonRedacted, 500, nameof(reasonRedacted));
        CreatedBy = Required(createdBy, 450, nameof(createdBy));
        CreatedAtUtc = DateTime.UtcNow;
        IsActive = true;
    }

    public void Release(string releasedBy, string reasonRedacted)
    {
        if (!IsActive) throw new InvalidOperationException("Legal hold is already released.");
        ReleasedBy = Required(releasedBy, 450, nameof(releasedBy));
        ReleaseReasonRedacted = Required(reasonRedacted, 500, nameof(reasonRedacted));
        ReleasedAtUtc = DateTime.UtcNow;
        IsActive = false;
    }

    private static string Required(string? value, int maxLength, string name)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized) || normalized.Length > maxLength)
            throw new ArgumentException($"{name} is required and cannot exceed {maxLength} characters.");
        return normalized;
    }
}
