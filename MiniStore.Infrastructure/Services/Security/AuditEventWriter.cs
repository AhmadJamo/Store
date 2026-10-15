using Microsoft.Extensions.Options;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Services.Security;

public sealed class AuditEventWriter : IAuditEventWriter
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditEventRedactor _redactor;
    private readonly IAuditEventIntegrityService _integrity;
    private readonly AuditEventOptions _options;

    public AuditEventWriter(
        AppDbContext context,
        ITenantContext tenantContext,
        ICurrentUserService currentUser,
        IAuditEventRedactor redactor,
        IAuditEventIntegrityService integrity,
        IOptions<AuditEventOptions> options)
    {
        _context = context;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
        _redactor = redactor;
        _integrity = integrity;
        _options = options.Value;
    }

    public ValueTask<Guid?> RecordAsync(AuditEventWriteRequest request, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled) return ValueTask.FromResult<Guid?>(null);
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(_options.ActiveIntegrityKeyVersion))
            throw new InvalidOperationException("AuditEvents integrity key version is required when auditing is enabled.");

        var activeTenantId = _tenantContext.TenantId;
        int? tenantId = request.Scope == AuditEventScope.Tenant
            ? activeTenantId ?? throw new InvalidOperationException("An active tenant is required for a tenant audit event.")
            : null;
        if (request.Scope != AuditEventScope.Tenant && activeTenantId.HasValue)
            throw new InvalidOperationException("Platform and system audit events require a non-tenant execution scope.");

        var payload = _redactor.Redact(request);
        var auditEvent = new AuditEvent(
            Guid.NewGuid(), request.Scope, tenantId, request.OccurredAtUtc ?? DateTime.UtcNow,
            request.Category, request.Severity, request.Outcome, request.EventType, request.Action,
            request.ActorType, request.ActorId ?? _currentUser.UserId, request.SourceChannel,
            request.CorrelationId, request.ResourceType, request.ResourceId, request.CausationId,
            request.OperationId, request.ReasonCode, payload.ReasonRedacted, payload.ChangesJson,
            payload.MetadataJson, request.RequestPathTemplate, request.SourceIpMasked, request.UserAgentHash);
        auditEvent.SealIntegrity(_options.ActiveIntegrityKeyVersion, _integrity.CreateHash(auditEvent));
        _context.AuditEvents.Add(auditEvent);
        return ValueTask.FromResult<Guid?>(auditEvent.EventId);
    }
}
