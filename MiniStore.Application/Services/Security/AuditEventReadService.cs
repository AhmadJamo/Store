using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class AuditEventQueryDto
{
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
    public AuditEventCategory? Category { get; set; }
    public AuditEventOutcome? Outcome { get; set; }
    public string? EventType { get; set; }
    public string? ActorId { get; set; }
    public string? ResourceType { get; set; }
    public string? ResourceId { get; set; }
    public string? CorrelationId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}

public sealed record AuditEventQuerySpec(
    DateTime FromUtc,
    DateTime ToExclusiveUtc,
    AuditEventCategory? Category,
    AuditEventOutcome? Outcome,
    string? EventType,
    string? ActorId,
    string? ResourceType,
    string? ResourceId,
    string? CorrelationId,
    int Skip,
    int Take,
    bool IncludeSensitive);

public sealed record AuditEventItemDto(
    long Id,
    Guid EventId,
    AuditEventScope Scope,
    DateTime OccurredAtUtc,
    AuditEventCategory Category,
    AuditEventSeverity Severity,
    AuditEventOutcome Outcome,
    string EventType,
    string Action,
    AuditActorType ActorType,
    string ActorId,
    AuditSourceChannel SourceChannel,
    string CorrelationId,
    string ResourceType,
    string ResourceId,
    string? ReasonCode,
    string? ChangesRedactedJson,
    string? MetadataRedactedJson,
    string? RequestPathTemplate,
    string? SourceIpMasked,
    string? UserAgentHash);

public sealed record AuditEventReadResult(IReadOnlyList<AuditEventItemDto> Items, int TotalCount);

public sealed class AuditEventPageDto
{
    public required AuditEventQueryDto Query { get; init; }
    public required IReadOnlyList<AuditEventItemDto> Items { get; init; }
    public required int TotalCount { get; init; }
    public required bool CanViewSensitive { get; init; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)Query.PageSize));
}

public sealed record AuditEventExportDto(byte[] Content, string FileName, int RecordCount);

public interface IAuditEventReadRepository
{
    Task<AuditEventReadResult> ReadTenantAsync(AuditEventQuerySpec query, CancellationToken cancellationToken = default);
    Task<AuditEventReadResult> ReadPlatformAsync(AuditEventQuerySpec query, CancellationToken cancellationToken = default);
}

public sealed class AuditEventReadService(
    IAuditEventReadRepository repository,
    IAuditEventWriter writer,
    IAuditRequestContext requestContext,
    IUnitOfWork unitOfWork)
{
    public static readonly string[] ExportPurposes = ["InternalReview", "SecurityInvestigation", "ComplianceReview"];
    private const int MaximumExportRows = 5000;

    public async Task<AuditEventPageDto> GetTenantPageAsync(
        AuditEventQueryDto query,
        bool includeSensitive,
        CancellationToken cancellationToken = default) =>
        await GetPageAsync(query, includeSensitive, false, cancellationToken);

    public async Task<AuditEventPageDto> GetPlatformPageAsync(
        AuditEventQueryDto query,
        bool includeSensitive,
        CancellationToken cancellationToken = default) =>
        await GetPageAsync(query, includeSensitive, true, cancellationToken);

    public Task<AuditEventExportDto> ExportTenantAsync(
        AuditEventQueryDto query,
        string purpose,
        bool includeSensitive,
        CancellationToken cancellationToken = default) =>
        ExportAsync(query, purpose, includeSensitive, false, cancellationToken);

    public Task<AuditEventExportDto> ExportPlatformAsync(
        AuditEventQueryDto query,
        string purpose,
        bool includeSensitive,
        CancellationToken cancellationToken = default) =>
        ExportAsync(query, purpose, includeSensitive, true, cancellationToken);

    private async Task<AuditEventPageDto> GetPageAsync(
        AuditEventQueryDto query,
        bool includeSensitive,
        bool platform,
        CancellationToken cancellationToken)
    {
        var normalized = Normalize(query, includeSensitive, false);
        var result = platform
            ? await repository.ReadPlatformAsync(normalized.Spec, cancellationToken)
            : await repository.ReadTenantAsync(normalized.Spec, cancellationToken);
        return new AuditEventPageDto
        {
            Query = normalized.Query,
            Items = result.Items,
            TotalCount = result.TotalCount,
            CanViewSensitive = includeSensitive
        };
    }

    private async Task<AuditEventExportDto> ExportAsync(
        AuditEventQueryDto query,
        string purpose,
        bool includeSensitive,
        bool platform,
        CancellationToken cancellationToken)
    {
        if (!ExportPurposes.Contains(purpose, StringComparer.Ordinal))
            throw new ArgumentException("Select a valid audit export purpose.");
        var normalized = Normalize(query, includeSensitive, true);
        var result = platform
            ? await repository.ReadPlatformAsync(normalized.Spec, cancellationToken)
            : await repository.ReadTenantAsync(normalized.Spec, cancellationToken);
        if (result.TotalCount > MaximumExportRows)
            throw new InvalidOperationException("Narrow the audit filters before exporting more than 5,000 events.");

        var content = CreateCsv(result.Items, includeSensitive);
        var fingerprint = Convert.ToHexString(SHA256.HashData(content));
        Guid? eventId = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            eventId = await writer.RecordAsync(new AuditEventWriteRequest
            {
                Scope = platform ? AuditEventScope.Platform : AuditEventScope.Tenant,
                Category = AuditEventCategory.DataAccess,
                Severity = AuditEventSeverity.Information,
                Outcome = AuditEventOutcome.Succeeded,
                EventType = SemanticAuditEventTypes.AuditEventsExported,
                Action = "Export",
                ActorType = AuditActorType.User,
                SourceChannel = platform ? AuditSourceChannel.Platform : requestContext.SourceChannel,
                CorrelationId = requestContext.CorrelationId,
                ResourceType = "AuditEventExport",
                ResourceId = requestContext.CorrelationId,
                ReasonCode = purpose,
                Metadata = new Dictionary<string, object?>
                {
                    ["RecordCount"] = result.Items.Count,
                    ["FileFingerprint"] = fingerprint,
                    ["ExportScope"] = platform ? "Platform" : "Tenant"
                },
                RequestPathTemplate = requestContext.RequestPathTemplate,
                SourceIpMasked = requestContext.SourceIpMasked,
                UserAgentHash = requestContext.UserAgentHash
            }, cancellationToken);
            if (!eventId.HasValue)
                throw new InvalidOperationException("Audit event recording must be enabled before exporting audit evidence.");
        });

        return new AuditEventExportDto(
            content,
            $"audit-events-{(platform ? "platform" : "tenant")}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv",
            result.Items.Count);
    }

    private static (AuditEventQueryDto Query, AuditEventQuerySpec Spec) Normalize(
        AuditEventQueryDto source,
        bool includeSensitive,
        bool export)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var from = source.From ?? today.AddDays(-30);
        var to = source.To ?? today;
        if (to < from) throw new ArgumentException("Audit end date cannot be earlier than the start date.");
        if (to.DayNumber - from.DayNumber > 366) throw new ArgumentException("Audit searches cannot exceed 366 days.");
        var pageSize = source.PageSize is 25 or 50 or 100 ? source.PageSize : 25;
        var page = Math.Clamp(source.Page, 1, 1_000_000);
        var normalized = new AuditEventQueryDto
        {
            From = from,
            To = to,
            Category = Enum.IsDefined(source.Category ?? 0) ? source.Category : null,
            Outcome = Enum.IsDefined(source.Outcome ?? 0) ? source.Outcome : null,
            EventType = SafeFilter(source.EventType, 150),
            ActorId = SafeFilter(source.ActorId, 450),
            ResourceType = SafeFilter(source.ResourceType, 150),
            ResourceId = SafeFilter(source.ResourceId, 150),
            CorrelationId = SafeFilter(source.CorrelationId, 100),
            Page = export ? 1 : page,
            PageSize = export ? MaximumExportRows + 1 : pageSize
        };
        return (normalized, new AuditEventQuerySpec(
            from.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            to.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            normalized.Category, normalized.Outcome, normalized.EventType, normalized.ActorId,
            normalized.ResourceType, normalized.ResourceId, normalized.CorrelationId,
            export ? 0 : (page - 1) * pageSize,
            normalized.PageSize,
            includeSensitive));
    }

    private static string? SafeFilter(string? value, int maxLength)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrEmpty(normalized)) return null;
        if (normalized.Length > maxLength) throw new ArgumentException("One or more audit filters are too long.");
        return normalized;
    }

    private static byte[] CreateCsv(IReadOnlyList<AuditEventItemDto> rows, bool includeSensitive)
    {
        var output = new StringBuilder();
        output.Append("OccurredAtUtc,Scope,Category,Severity,Outcome,EventType,Action,ActorType,ActorId,SourceChannel,ResourceType,ResourceId,ReasonCode,CorrelationId,Route,Changes,Metadata");
        if (includeSensitive) output.Append(",SourceIpMasked,UserAgentHash");
        output.AppendLine();
        foreach (var row in rows)
        {
            var values = new List<string?>
            {
                row.OccurredAtUtc.ToString("O", CultureInfo.InvariantCulture), row.Scope.ToString(), row.Category.ToString(),
                row.Severity.ToString(), row.Outcome.ToString(), row.EventType, row.Action, row.ActorType.ToString(),
                row.ActorId, row.SourceChannel.ToString(), row.ResourceType, row.ResourceId, row.ReasonCode,
                row.CorrelationId, row.RequestPathTemplate, row.ChangesRedactedJson, row.MetadataRedactedJson
            };
            if (includeSensitive) { values.Add(row.SourceIpMasked); values.Add(row.UserAgentHash); }
            output.AppendLine(string.Join(',', values.Select(Csv)));
        }
        return new UTF8Encoding(true).GetBytes(output.ToString());
    }

    private static string Csv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var safe = value[0] is '=' or '+' or '-' or '@' ? $"'{value}" : value;
        return $"\"{safe.Replace("\"", "\"\"")}\"";
    }
}
