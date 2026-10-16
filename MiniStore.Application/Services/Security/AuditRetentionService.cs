using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed record AuditRetentionPolicy(
    bool Enabled,
    int ProtectedRetentionYears,
    int OperationalRetentionYears,
    int RequestEvidenceDays,
    int BatchSize);

public sealed record AuditLegalHoldDto(long Id, Guid HoldId, AuditEventScope Scope, int? TenantId,
    DateTime FromUtc, DateTime ToUtc, string ReasonRedacted, string CreatedBy, DateTime CreatedAtUtc,
    bool IsActive, string? ReleasedBy, string? ReleaseReasonRedacted, DateTime? ReleasedAtUtc);

public sealed record CreateAuditLegalHoldDto(AuditEventScope Scope, int? TenantId, DateTime FromUtc,
    DateTime ToUtc, string Reason);

public sealed record AuditCheckpointRow(long Id, Guid EventId, string IntegrityHash);

public sealed record AuditRetentionCandidate(long Id, Guid EventId, AuditEventScope Scope, int? TenantId,
    DateTime OccurredAtUtc, AuditEventCategory Category, string EventType, string IntegrityHash,
    string RetentionClass);

public sealed record AuditArchiveReceipt(Guid BatchId, string FileFingerprint, int RecordCount);
public sealed record AuditCheckpointReceipt(Guid CheckpointId, string RootHash, int RecordCount);
public sealed record AuditRetentionRunResult(int CandidateCount, int DeletedCount, Guid? BatchId, Guid CheckpointId);
public sealed record AuditRetentionStatusDto(AuditRetentionPolicy Policy, IReadOnlyList<AuditLegalHoldDto> Holds);

public interface IAuditRetentionRepository
{
    Task<IReadOnlyList<AuditLegalHoldDto>> GetLegalHoldsAsync(CancellationToken cancellationToken = default);
    Task AddLegalHoldAsync(AuditLegalHold hold, CancellationToken cancellationToken = default);
    Task<AuditLegalHold?> GetLegalHoldAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditCheckpointRow>> GetCheckpointRowsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditRetentionCandidate>> GetExpiredCandidatesAsync(DateTime nowUtc,
        AuditRetentionPolicy policy, CancellationToken cancellationToken = default);
    Task<int> DeleteArchivedCandidatesAsync(IReadOnlyList<AuditRetentionCandidate> candidates,
        CancellationToken cancellationToken = default);
}

public interface IAuditArchiveStore
{
    Task<AuditCheckpointReceipt> WriteCheckpointAsync(IReadOnlyList<AuditCheckpointRow> rows,
        CancellationToken cancellationToken = default);
    Task<AuditArchiveReceipt> WriteDestructionManifestAsync(IReadOnlyList<AuditRetentionCandidate> candidates,
        Guid checkpointId, CancellationToken cancellationToken = default);
    Task MarkDestructionCompletedAsync(AuditArchiveReceipt receipt, int deletedCount,
        CancellationToken cancellationToken = default);
}

public sealed class AuditRetentionService(
    IAuditRetentionRepository repository,
    IAuditArchiveStore archiveStore,
    AuditRetentionPolicy policy,
    IAuditEventWriter auditWriter,
    IAuditRequestContext requestContext,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
{
    public async Task<AuditRetentionStatusDto> GetStatusAsync(CancellationToken cancellationToken = default) =>
        new(policy, await repository.GetLegalHoldsAsync(cancellationToken));

    public async Task<Guid> CreateLegalHoldAsync(CreateAuditLegalHoldDto input,
        CancellationToken cancellationToken = default)
    {
        var actor = Required(currentUser.UserId, 450, "Actor");
        var fromUtc = EnsureUtc(input.FromUtc);
        var toUtc = EnsureUtc(input.ToUtc);
        if (toUtc.TimeOfDay == TimeSpan.Zero) toUtc = toUtc.Date.AddDays(1).AddTicks(-1);
        var hold = new AuditLegalHold(Guid.NewGuid(), input.Scope, input.TenantId,
            fromUtc, toUtc, Required(input.Reason, 500, "Reason"), actor);
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await repository.AddLegalHoldAsync(hold, cancellationToken);
            RequireRecorded(await auditWriter.RecordAsync(PlatformEvent(SemanticAuditEventTypes.AuditLegalHoldCreated,
                "Create", "AuditLegalHold", hold.HoldId.ToString("D"), "OwnerApproved",
                new Dictionary<string, object?> { ["Scope"] = hold.Scope.ToString(), ["TenantId"] = hold.TenantId }), cancellationToken));
        });
        return hold.HoldId;
    }

    public async Task ReleaseLegalHoldAsync(long id, string reason, CancellationToken cancellationToken = default)
    {
        var hold = await repository.GetLegalHoldAsync(id, cancellationToken)
                   ?? throw new InvalidOperationException("Legal hold was not found.");
        hold.Release(Required(currentUser.UserId, 450, "Actor"), Required(reason, 500, "Reason"));
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            RequireRecorded(await auditWriter.RecordAsync(PlatformEvent(SemanticAuditEventTypes.AuditLegalHoldReleased,
                "Release", "AuditLegalHold", hold.HoldId.ToString("D"), "OwnerApproved",
                new Dictionary<string, object?> { ["Scope"] = hold.Scope.ToString(), ["TenantId"] = hold.TenantId }), cancellationToken));
        });
    }

    public async Task<AuditCheckpointReceipt> CreateCheckpointAsync(CancellationToken cancellationToken = default)
    {
        EnsureEnabled();
        var receipt = await archiveStore.WriteCheckpointAsync(
            await repository.GetCheckpointRowsAsync(cancellationToken), cancellationToken);
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            RequireRecorded(await auditWriter.RecordAsync(SystemEvent(SemanticAuditEventTypes.AuditCheckpointCreated,
                "Checkpoint", receipt.CheckpointId.ToString("D"), new Dictionary<string, object?>
                {
                    ["RecordCount"] = receipt.RecordCount,
                    ["RootFingerprint"] = receipt.RootHash
                }), cancellationToken));
        });
        return receipt;
    }

    public async Task<AuditRetentionRunResult> RunRetentionAsync(CancellationToken cancellationToken = default)
    {
        EnsureEnabled();
        var checkpoint = await CreateCheckpointAsync(cancellationToken);
        var candidates = await repository.GetExpiredCandidatesAsync(DateTime.UtcNow, policy, cancellationToken);
        if (candidates.Count == 0) return new(0, 0, null, checkpoint.CheckpointId);

        var archive = await archiveStore.WriteDestructionManifestAsync(candidates, checkpoint.CheckpointId, cancellationToken);
        var deleted = await repository.DeleteArchivedCandidatesAsync(candidates, cancellationToken);
        if (deleted != candidates.Count)
            throw new InvalidOperationException("Retention candidates changed or became legally held after the archive manifest was written.");
        await archiveStore.MarkDestructionCompletedAsync(archive, deleted, cancellationToken);
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            RequireRecorded(await auditWriter.RecordAsync(SystemEvent(SemanticAuditEventTypes.AuditRetentionExecuted,
                "Retain", archive.BatchId.ToString("D"), new Dictionary<string, object?>
                {
                    ["RecordCount"] = deleted,
                    ["FileFingerprint"] = archive.FileFingerprint
                }), cancellationToken));
        });
        return new(candidates.Count, deleted, archive.BatchId, checkpoint.CheckpointId);
    }

    private AuditEventWriteRequest PlatformEvent(string eventType, string action, string resourceType,
        string resourceId, string reasonCode, IReadOnlyDictionary<string, object?> metadata) => new()
    {
        Scope = AuditEventScope.Platform, Category = AuditEventCategory.Administration,
        Severity = AuditEventSeverity.Information, Outcome = AuditEventOutcome.Succeeded,
        EventType = eventType, Action = action, ActorType = AuditActorType.User,
        ActorId = currentUser.UserId, SourceChannel = AuditSourceChannel.Platform,
        CorrelationId = requestContext.CorrelationId, ResourceType = resourceType, ResourceId = resourceId,
        ReasonCode = reasonCode, Metadata = metadata, RequestPathTemplate = requestContext.RequestPathTemplate,
        SourceIpMasked = requestContext.SourceIpMasked, UserAgentHash = requestContext.UserAgentHash
    };

    private static AuditEventWriteRequest SystemEvent(string eventType, string action, string resourceId,
        IReadOnlyDictionary<string, object?> metadata) => new()
    {
        Scope = AuditEventScope.System, Category = AuditEventCategory.Security,
        Severity = AuditEventSeverity.Information, Outcome = AuditEventOutcome.Succeeded,
        EventType = eventType, Action = action, ActorType = AuditActorType.System, ActorId = "audit-maintenance",
        SourceChannel = AuditSourceChannel.Worker, CorrelationId = Guid.NewGuid().ToString("N"),
        ResourceType = "AuditMaintenance", ResourceId = resourceId, ReasonCode = "ApprovedRetentionPolicy",
        Metadata = metadata
    };

    private void EnsureEnabled()
    {
        if (!policy.Enabled) throw new InvalidOperationException("Audit retention maintenance is disabled.");
    }

    private static void RequireRecorded(Guid? eventId)
    {
        if (!eventId.HasValue)
            throw new InvalidOperationException("Audit event recording must be enabled before audit retention maintenance.");
    }

    private static DateTime EnsureUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };

    private static string Required(string? value, int maxLength, string name)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized) || normalized.Length > maxLength)
            throw new ArgumentException($"{name} is required and cannot exceed {maxLength} characters.");
        return normalized;
    }
}
