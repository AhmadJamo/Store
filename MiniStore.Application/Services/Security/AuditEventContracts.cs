using MiniStore.Domain.Entities;

namespace MiniStore.Application.Services;

public sealed record AuditEventWriteRequest
{
    public required AuditEventScope Scope { get; init; }
    public required AuditEventCategory Category { get; init; }
    public required AuditEventSeverity Severity { get; init; }
    public required AuditEventOutcome Outcome { get; init; }
    public required string EventType { get; init; }
    public required string Action { get; init; }
    public required AuditActorType ActorType { get; init; }
    public string? ActorId { get; init; }
    public required AuditSourceChannel SourceChannel { get; init; }
    public required string CorrelationId { get; init; }
    public string? CausationId { get; init; }
    public string? OperationId { get; init; }
    public required string ResourceType { get; init; }
    public required string ResourceId { get; init; }
    public string? ReasonCode { get; init; }
    public string? ReasonRedacted { get; init; }
    public IReadOnlyDictionary<string, object?>? BeforeValues { get; init; }
    public IReadOnlyDictionary<string, object?>? AfterValues { get; init; }
    public IReadOnlyDictionary<string, object?>? Metadata { get; init; }
    public string? RequestPathTemplate { get; init; }
    public string? SourceIpMasked { get; init; }
    public string? UserAgentHash { get; init; }
    public DateTime? OccurredAtUtc { get; init; }
}

public sealed record RedactedAuditPayload(string? ReasonRedacted, string? ChangesJson, string? MetadataJson);

public interface IAuditEventWriter
{
    ValueTask<Guid?> RecordAsync(AuditEventWriteRequest request, CancellationToken cancellationToken = default);
}

public interface IAuditRequestContext
{
    string CorrelationId { get; }
    string? RequestPathTemplate { get; }
    AuditSourceChannel SourceChannel { get; }
}

public interface IAuditEventRedactor
{
    RedactedAuditPayload Redact(AuditEventWriteRequest request);
}

public interface IAuditEventIntegrityService
{
    string CreateHash(AuditEvent auditEvent);
    bool Verify(AuditEvent auditEvent);
}
