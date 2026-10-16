using Microsoft.EntityFrameworkCore;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class AuditEventReadRepository(AppDbContext context, ITenantContext tenantContext)
    : IAuditEventReadRepository
{
    public Task<AuditEventReadResult> ReadTenantAsync(
        AuditEventQuerySpec query,
        CancellationToken cancellationToken = default)
    {
        if (tenantContext.TenantId is null)
            throw new InvalidOperationException("An active company is required to read company audit events.");
        return ReadAsync(context.AuditEvents.AsNoTracking().Where(value => value.Scope == AuditEventScope.Tenant), query, cancellationToken);
    }

    public Task<AuditEventReadResult> ReadPlatformAsync(
        AuditEventQuerySpec query,
        CancellationToken cancellationToken = default)
    {
        if (tenantContext.TenantId.HasValue)
            throw new InvalidOperationException("Platform audit events require a non-company execution scope.");
        return ReadAsync(context.AuditEvents.IgnoreQueryFilters().AsNoTracking()
            .Where(value => value.Scope == AuditEventScope.Platform || value.Scope == AuditEventScope.System), query, cancellationToken);
    }

    private static async Task<AuditEventReadResult> ReadAsync(
        IQueryable<AuditEvent> source,
        AuditEventQuerySpec query,
        CancellationToken cancellationToken)
    {
        source = source.Where(value => value.OccurredAtUtc >= query.FromUtc && value.OccurredAtUtc < query.ToExclusiveUtc);
        if (query.Category.HasValue) source = source.Where(value => value.Category == query.Category.Value);
        if (query.Outcome.HasValue) source = source.Where(value => value.Outcome == query.Outcome.Value);
        if (query.EventType is not null) source = source.Where(value => value.EventType == query.EventType);
        if (query.ActorId is not null) source = source.Where(value => value.ActorId == query.ActorId);
        if (query.ResourceType is not null) source = source.Where(value => value.ResourceType == query.ResourceType);
        if (query.ResourceId is not null) source = source.Where(value => value.ResourceId == query.ResourceId);
        if (query.CorrelationId is not null) source = source.Where(value => value.CorrelationId == query.CorrelationId);

        var total = await source.CountAsync(cancellationToken);
        var items = await source.OrderByDescending(value => value.OccurredAtUtc).ThenByDescending(value => value.Id)
            .Skip(query.Skip).Take(query.Take)
            .Select(value => new AuditEventItemDto(
                value.Id, value.EventId, value.Scope, value.OccurredAtUtc, value.Category, value.Severity,
                value.Outcome, value.EventType, value.Action, value.ActorType, value.ActorId, value.SourceChannel,
                value.CorrelationId, value.ResourceType, value.ResourceId, value.ReasonCode,
                value.ChangesRedactedJson, value.MetadataRedactedJson, value.RequestPathTemplate,
                query.IncludeSensitive ? value.SourceIpMasked : null,
                query.IncludeSensitive ? value.UserAgentHash : null))
            .ToListAsync(cancellationToken);
        return new AuditEventReadResult(items, total);
    }
}
