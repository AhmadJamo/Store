using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Infrastructure.Services.Security;

namespace MiniStore.Infrastructure.Repositories;

public sealed class AuditRetentionRepository(AppDbContext context, AuditDatabaseAccessOptions access) : IAuditRetentionRepository
{
    public async Task<IReadOnlyList<AuditLegalHoldDto>> GetLegalHoldsAsync(CancellationToken cancellationToken = default) =>
        await context.AuditLegalHolds.AsNoTracking().OrderByDescending(value => value.IsActive)
            .ThenByDescending(value => value.CreatedAtUtc)
            .Select(value => new AuditLegalHoldDto(value.Id, value.HoldId, value.Scope, value.TenantId,
                value.FromUtc, value.ToUtc, value.ReasonRedacted, value.CreatedBy, value.CreatedAtUtc,
                value.IsActive, value.ReleasedBy, value.ReleaseReasonRedacted, value.ReleasedAtUtc))
            .ToListAsync(cancellationToken);

    public Task AddLegalHoldAsync(AuditLegalHold hold, CancellationToken cancellationToken = default) =>
        context.AuditLegalHolds.AddAsync(hold, cancellationToken).AsTask();

    public Task<AuditLegalHold?> GetLegalHoldAsync(long id, CancellationToken cancellationToken = default) =>
        context.AuditLegalHolds.SingleOrDefaultAsync(value => value.Id == id, cancellationToken);

    public async Task<IReadOnlyList<AuditCheckpointRow>> GetCheckpointRowsAsync(CancellationToken cancellationToken = default) =>
        await context.AuditEvents.IgnoreQueryFilters().AsNoTracking().OrderBy(value => value.Id)
            .Select(value => new AuditCheckpointRow(value.Id, value.EventId, value.IntegrityHash))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<AuditRetentionCandidate>> GetExpiredCandidatesAsync(DateTime nowUtc,
        AuditRetentionPolicy policy, CancellationToken cancellationToken = default)
    {
        var protectedCutoff = nowUtc.AddYears(-policy.ProtectedRetentionYears);
        var operationalCutoff = nowUtc.AddYears(-policy.OperationalRetentionYears);
        var requestCutoff = nowUtc.AddDays(-policy.RequestEvidenceDays);
        var detailedAuthenticationTypes = new[]
        {
            SemanticAuditEventTypes.TenantLoginFailed,
            SemanticAuditEventTypes.PlatformLoginFailed
        };

        return await context.AuditEvents.IgnoreQueryFilters().AsNoTracking()
            .Where(value =>
                (detailedAuthenticationTypes.Contains(value.EventType) && value.OccurredAtUtc < requestCutoff) ||
                ((value.Category == AuditEventCategory.Security || value.Category == AuditEventCategory.Administration ||
                  value.Action == "Post" || value.Action == "Reverse" || value.Action == "Approve" || value.Action == "Override")
                    ? value.OccurredAtUtc < protectedCutoff
                    : value.OccurredAtUtc < operationalCutoff))
            .Where(value => !context.AuditLegalHolds.Any(hold => hold.IsActive && hold.Scope == value.Scope &&
                hold.TenantId == value.TenantId && hold.FromUtc <= value.OccurredAtUtc && hold.ToUtc >= value.OccurredAtUtc))
            .OrderBy(value => value.OccurredAtUtc).ThenBy(value => value.Id)
            .Take(policy.BatchSize)
            .Select(value => new AuditRetentionCandidate(value.Id, value.EventId, value.Scope, value.TenantId,
                value.OccurredAtUtc, value.Category, value.EventType, value.IntegrityHash,
                detailedAuthenticationTypes.Contains(value.EventType) ? "RequestEvidence90Days" :
                value.Category == AuditEventCategory.Security || value.Category == AuditEventCategory.Administration ||
                value.Action == "Post" || value.Action == "Reverse" || value.Action == "Approve" || value.Action == "Override"
                    ? "Protected7Years" : "Operational3Years"))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> DeleteArchivedCandidatesAsync(IReadOnlyList<AuditRetentionCandidate> candidates,
        CancellationToken cancellationToken = default)
    {
        if (candidates.Count == 0) return 0;
        await using var connection = new SqlConnection(access.MaintenanceConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken);
        var values = string.Join(",", candidates.Select((_, index) => $"(@Id{index},@Hash{index})"));
        var protectedRows = $"""
            FROM [dbo].[AuditEvents] AS eventRow
            INNER JOIN (VALUES {values}) AS expected([Id], [IntegrityHash])
                ON expected.[Id] = eventRow.[Id] AND expected.[IntegrityHash] = eventRow.[IntegrityHash]
            WHERE NOT EXISTS (
                SELECT 1 FROM [dbo].[AuditLegalHolds] AS hold
                WHERE hold.[IsActive] = 1 AND hold.[Scope] = eventRow.[Scope]
                  AND ((hold.[TenantId] = eventRow.[TenantId]) OR (hold.[TenantId] IS NULL AND eventRow.[TenantId] IS NULL))
                  AND hold.[FromUtc] <= eventRow.[OccurredAtUtc] AND hold.[ToUtc] >= eventRow.[OccurredAtUtc])
            """;
        await using var count = new SqlCommand($"SELECT COUNT_BIG(*) {protectedRows}", connection, transaction);
        AddCandidateParameters(count, candidates);
        if ((long)(await count.ExecuteScalarAsync(cancellationToken) ?? 0L) != candidates.Count)
        {
            await transaction.RollbackAsync(cancellationToken);
            return 0;
        }

        await using var delete = new SqlCommand($"DELETE eventRow {protectedRows}", connection, transaction);
        AddCandidateParameters(delete, candidates);
        var deleted = await delete.ExecuteNonQueryAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deleted;
    }

    private static void AddCandidateParameters(SqlCommand command, IReadOnlyList<AuditRetentionCandidate> candidates)
    {
        for (var index = 0; index < candidates.Count; index++)
        {
            command.Parameters.Add(new SqlParameter($"@Id{index}", SqlDbType.BigInt) { Value = candidates[index].Id });
            command.Parameters.Add(new SqlParameter($"@Hash{index}", SqlDbType.Char, 64) { Value = candidates[index].IntegrityHash });
        }
    }
}
