using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Services.Security;

public sealed class SecurityAuditEventWriter(
    IServiceScopeFactory scopeFactory,
    IOptions<AuditEventOptions> options,
    ILogger<SecurityAuditEventWriter> logger) : ISecurityAuditEventWriter
{
    public async Task<Guid?> RecordAsync(
        IndependentAuditEventWriteRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!options.Value.Enabled) return null;
        if ((request.Scope == AuditEventScope.Tenant) != request.TenantId.HasValue)
            throw new InvalidOperationException("Independent tenant audit events require one tenant; platform/system events cannot carry a tenant.");

        try
        {
            using var scope = scopeFactory.CreateScope();
            var services = scope.ServiceProvider;
            IDisposable? tenant = request.TenantId.HasValue
                ? services.GetRequiredService<ITenantScope>().BeginScope(request.TenantId.Value)
                : null;
            using (tenant)
            {
                var writer = services.GetRequiredService<IAuditEventWriter>();
                var eventId = await writer.RecordAsync(new AuditEventWriteRequest
                {
                    Scope = request.Scope,
                    Category = request.Category,
                    Severity = request.Severity,
                    Outcome = request.Outcome,
                    EventType = request.EventType,
                    Action = request.Action,
                    ActorType = request.ActorType,
                    ActorId = request.ActorId,
                    SourceChannel = request.SourceChannel,
                    CorrelationId = request.CorrelationId,
                    ResourceType = request.ResourceType,
                    ResourceId = request.ResourceId,
                    ReasonCode = request.ReasonCode,
                    RequestPathTemplate = request.RequestPathTemplate,
                    SourceIpMasked = request.SourceIpMasked,
                    UserAgentHash = request.UserAgentHash
                }, cancellationToken);
                await services.GetRequiredService<AppDbContext>().SaveChangesAsync(cancellationToken);
                return eventId;
            }
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Independent security audit event {EventType} could not be persisted.", request.EventType);
            throw;
        }
    }
}
