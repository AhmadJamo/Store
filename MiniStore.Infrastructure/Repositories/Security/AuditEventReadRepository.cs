using System.Data;
using Microsoft.Data.SqlClient;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Services.Security;

namespace MiniStore.Infrastructure.Repositories;

public sealed class AuditEventReadRepository(AuditDatabaseAccessOptions options, ITenantContext tenantContext)
    : IAuditEventReadRepository
{
    public Task<AuditEventReadResult> ReadTenantAsync(AuditEventQuerySpec query, CancellationToken cancellationToken = default)
    {
        var tenantId = tenantContext.TenantId
                       ?? throw new InvalidOperationException("An active company is required to read company audit events.");
        return ReadAsync(query, "[Scope] = 1 AND [TenantId] = @TenantId", tenantId, cancellationToken);
    }

    public Task<AuditEventReadResult> ReadPlatformAsync(AuditEventQuerySpec query, CancellationToken cancellationToken = default)
    {
        if (tenantContext.TenantId.HasValue)
            throw new InvalidOperationException("Platform audit events require a non-company execution scope.");
        return ReadAsync(query, "[Scope] IN (2, 3)", null, cancellationToken);
    }

    private async Task<AuditEventReadResult> ReadAsync(AuditEventQuerySpec query, string scopePredicate,
        int? tenantId, CancellationToken cancellationToken)
    {
        var where = new List<string> { scopePredicate, "[OccurredAtUtc] >= @FromUtc", "[OccurredAtUtc] < @ToUtc" };
        var parameters = new List<SqlParameter>();
        if (tenantId.HasValue) parameters.Add(new("@TenantId", SqlDbType.Int) { Value = tenantId.Value });
        parameters.Add(new("@FromUtc", SqlDbType.DateTime2) { Value = query.FromUtc });
        parameters.Add(new("@ToUtc", SqlDbType.DateTime2) { Value = query.ToExclusiveUtc });
        Add(where, parameters, query.Category, "[Category]", "@Category", SqlDbType.Int);
        Add(where, parameters, query.Outcome, "[Outcome]", "@Outcome", SqlDbType.Int);
        Add(where, parameters, query.EventType, "[EventType]", "@EventType", SqlDbType.NVarChar, 150);
        Add(where, parameters, query.ActorId, "[ActorId]", "@ActorId", SqlDbType.NVarChar, 450);
        Add(where, parameters, query.ResourceType, "[ResourceType]", "@ResourceType", SqlDbType.NVarChar, 150);
        Add(where, parameters, query.ResourceId, "[ResourceId]", "@ResourceId", SqlDbType.NVarChar, 150);
        Add(where, parameters, query.CorrelationId, "[CorrelationId]", "@CorrelationId", SqlDbType.NVarChar, 100);
        var predicate = string.Join(" AND ", where);

        await using var connection = new SqlConnection(options.ReadConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var countCommand = new SqlCommand($"SELECT COUNT_BIG(*) FROM [dbo].[AuditEvents] WHERE {predicate}", connection);
        AddParameters(countCommand, parameters);
        var totalLong = (long)(await countCommand.ExecuteScalarAsync(cancellationToken) ?? 0L);
        var total = totalLong > int.MaxValue ? int.MaxValue : (int)totalLong;

        var sensitiveColumns = query.IncludeSensitive
            ? "[SourceIpMasked], [UserAgentHash]"
            : "CAST(NULL AS nvarchar(100)) AS [SourceIpMasked], CAST(NULL AS nvarchar(64)) AS [UserAgentHash]";
        var sql = $"""
            SELECT [Id], [EventId], [Scope], [OccurredAtUtc], [Category], [Severity], [Outcome],
                   [EventType], [Action], [ActorType], [ActorId], [SourceChannel], [CorrelationId],
                   [ResourceType], [ResourceId], [ReasonCode], [ChangesRedactedJson], [MetadataRedactedJson],
                   [RequestPathTemplate], {sensitiveColumns}
            FROM [dbo].[AuditEvents]
            WHERE {predicate}
            ORDER BY [OccurredAtUtc] DESC, [Id] DESC
            OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
            """;
        await using var command = new SqlCommand(sql, connection);
        AddParameters(command, parameters);
        command.Parameters.Add(new SqlParameter("@Skip", SqlDbType.Int) { Value = query.Skip });
        command.Parameters.Add(new SqlParameter("@Take", SqlDbType.Int) { Value = query.Take });
        var items = new List<AuditEventItemDto>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
            items.Add(new AuditEventItemDto(reader.GetInt64(0), reader.GetGuid(1), (AuditEventScope)reader.GetInt32(2),
                reader.GetDateTime(3), (AuditEventCategory)reader.GetInt32(4), (AuditEventSeverity)reader.GetInt32(5),
                (AuditEventOutcome)reader.GetInt32(6), reader.GetString(7), reader.GetString(8),
                (AuditActorType)reader.GetInt32(9), reader.GetString(10), (AuditSourceChannel)reader.GetInt32(11),
                reader.GetString(12), reader.GetString(13), reader.GetString(14), Nullable(reader, 15),
                Nullable(reader, 16), Nullable(reader, 17), Nullable(reader, 18), Nullable(reader, 19), Nullable(reader, 20)));
        return new(items, total);
    }

    private static string? Nullable(SqlDataReader reader, int ordinal) => reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);

    private static void Add<T>(ICollection<string> where, ICollection<SqlParameter> parameters, T? value,
        string column, string name, SqlDbType type, int size = 0)
    {
        if (value is null) return;
        where.Add($"{column} = {name}");
        var parameter = new SqlParameter(name, type) { Value = value is Enum ? Convert.ToInt32(value) : value };
        if (size > 0) parameter.Size = size;
        parameters.Add(parameter);
    }

    private static void AddParameters(SqlCommand command, IEnumerable<SqlParameter> parameters)
    {
        foreach (var source in parameters)
            command.Parameters.Add(new SqlParameter(source.ParameterName, source.SqlDbType, source.Size) { Value = source.Value });
    }
}
