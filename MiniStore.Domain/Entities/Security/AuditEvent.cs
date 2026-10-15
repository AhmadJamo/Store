namespace MiniStore.Domain.Entities;

public enum AuditEventScope { Tenant = 1, Platform = 2, System = 3 }
public enum AuditEventCategory { Security = 1, Administration = 2, Configuration = 3, Operations = 4, Integration = 5, DataAccess = 6 }
public enum AuditEventSeverity { Information = 1, Warning = 2, Critical = 3 }
public enum AuditEventOutcome { Succeeded = 1, Denied = 2, Failed = 3, Unknown = 4 }
public enum AuditActorType { User = 1, System = 2, External = 3, Support = 4 }
public enum AuditSourceChannel { Web = 1, Worker = 2, Api = 3, Portal = 4, Import = 5, Platform = 6 }

public sealed class AuditEvent
{
    private AuditEvent()
    {
        EventType = Action = ActorId = CorrelationId = ResourceType = ResourceId = IntegrityKeyVersion = IntegrityHash = string.Empty;
    }

    public long Id { get; private set; }
    public Guid EventId { get; private set; }
    public int SchemaVersion { get; private set; }
    public AuditEventScope Scope { get; private set; }
    public int? TenantId { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }
    public AuditEventCategory Category { get; private set; }
    public AuditEventSeverity Severity { get; private set; }
    public AuditEventOutcome Outcome { get; private set; }
    public string EventType { get; private set; }
    public string Action { get; private set; }
    public AuditActorType ActorType { get; private set; }
    public string ActorId { get; private set; }
    public AuditSourceChannel SourceChannel { get; private set; }
    public string CorrelationId { get; private set; }
    public string? CausationId { get; private set; }
    public string? OperationId { get; private set; }
    public string ResourceType { get; private set; }
    public string ResourceId { get; private set; }
    public string? ReasonCode { get; private set; }
    public string? ReasonRedacted { get; private set; }
    public string? ChangesRedactedJson { get; private set; }
    public string? MetadataRedactedJson { get; private set; }
    public string? RequestPathTemplate { get; private set; }
    public string? SourceIpMasked { get; private set; }
    public string? UserAgentHash { get; private set; }
    public string IntegrityKeyVersion { get; private set; }
    public string IntegrityHash { get; private set; }
    public DateTime RecordedAtUtc { get; private set; }

    public AuditEvent(
        Guid eventId,
        AuditEventScope scope,
        int? tenantId,
        DateTime occurredAtUtc,
        AuditEventCategory category,
        AuditEventSeverity severity,
        AuditEventOutcome outcome,
        string eventType,
        string action,
        AuditActorType actorType,
        string actorId,
        AuditSourceChannel sourceChannel,
        string correlationId,
        string resourceType,
        string resourceId,
        string? causationId = null,
        string? operationId = null,
        string? reasonCode = null,
        string? reasonRedacted = null,
        string? changesRedactedJson = null,
        string? metadataRedactedJson = null,
        string? requestPathTemplate = null,
        string? sourceIpMasked = null,
        string? userAgentHash = null)
    {
        if (eventId == Guid.Empty) throw new ArgumentException("Audit event identity is required.");
        if (!Enum.IsDefined(scope) || !Enum.IsDefined(category) || !Enum.IsDefined(severity) ||
            !Enum.IsDefined(outcome) || !Enum.IsDefined(actorType) || !Enum.IsDefined(sourceChannel))
            throw new ArgumentException("Audit event classification is invalid.");
        if ((scope == AuditEventScope.Tenant) != tenantId.HasValue || tenantId <= 0)
            throw new ArgumentException("Tenant audit scope requires one valid tenant and platform/system scope cannot carry a tenant.");
        if (occurredAtUtc.Kind != DateTimeKind.Utc) throw new ArgumentException("Audit event time must be UTC.");
        if (outcome is AuditEventOutcome.Denied or AuditEventOutcome.Failed && string.IsNullOrWhiteSpace(reasonCode))
            throw new ArgumentException("Denied and failed audit events require a reason code.");

        EventId = eventId;
        SchemaVersion = 1;
        Scope = scope;
        TenantId = tenantId;
        OccurredAtUtc = occurredAtUtc;
        Category = category;
        Severity = severity;
        Outcome = outcome;
        EventType = TechnicalCode(eventType, 150, nameof(eventType));
        Action = TechnicalCode(action, 50, nameof(action));
        ActorType = actorType;
        ActorId = Required(actorId, 450, nameof(actorId));
        SourceChannel = sourceChannel;
        CorrelationId = Required(correlationId, 100, nameof(correlationId));
        CausationId = Optional(causationId, 100, nameof(causationId));
        OperationId = Optional(operationId, 100, nameof(operationId));
        ResourceType = TechnicalCode(resourceType, 150, nameof(resourceType));
        ResourceId = Required(resourceId, 150, nameof(resourceId));
        ReasonCode = string.IsNullOrWhiteSpace(reasonCode) ? null : TechnicalCode(reasonCode, 100, nameof(reasonCode));
        ReasonRedacted = Optional(reasonRedacted, 1000, nameof(reasonRedacted));
        ChangesRedactedJson = Optional(changesRedactedJson, 16000, nameof(changesRedactedJson));
        MetadataRedactedJson = Optional(metadataRedactedJson, 8000, nameof(metadataRedactedJson));
        RequestPathTemplate = Optional(requestPathTemplate, 250, nameof(requestPathTemplate));
        SourceIpMasked = Optional(sourceIpMasked, 100, nameof(sourceIpMasked));
        UserAgentHash = Optional(userAgentHash, 64, nameof(userAgentHash));
        if (UserAgentHash is not null && (UserAgentHash.Length != 64 || UserAgentHash.Any(character => !Uri.IsHexDigit(character))))
            throw new ArgumentException("User agent hash must be a SHA-256 hexadecimal value.");
        UserAgentHash = UserAgentHash?.ToUpperInvariant();
        IntegrityKeyVersion = string.Empty;
        IntegrityHash = string.Empty;
        RecordedAtUtc = DateTime.UtcNow;
    }

    public void SealIntegrity(string keyVersion, string hash)
    {
        if (!string.IsNullOrEmpty(IntegrityHash)) throw new InvalidOperationException("Audit event integrity is already sealed.");
        IntegrityKeyVersion = TechnicalCode(keyVersion, 50, nameof(keyVersion));
        IntegrityHash = Required(hash, 64, nameof(hash));
        if (IntegrityHash.Length != 64 || IntegrityHash.Any(character => !Uri.IsHexDigit(character)))
            throw new ArgumentException("Audit event integrity hash must be a SHA-256 hexadecimal value.");
        IntegrityHash = IntegrityHash.ToUpperInvariant();
    }

    private static string Required(string? value, int maxLength, string name)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized) || normalized.Length > maxLength)
            throw new ArgumentException($"{name} is required and cannot exceed {maxLength} characters.");
        return normalized;
    }

    private static string? Optional(string? value, int maxLength, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Trim();
        if (normalized.Length > maxLength) throw new ArgumentException($"{name} cannot exceed {maxLength} characters.");
        return normalized;
    }

    private static string TechnicalCode(string? value, int maxLength, string name)
    {
        var normalized = Required(value, maxLength, name);
        if (normalized.Any(character => !(
                character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9' or '.' or '_' or '-' or ':')))
            throw new ArgumentException($"{name} must be a language-neutral technical code.");
        return normalized;
    }
}
