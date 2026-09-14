using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Infrastructure.Persistence;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ITenantContext _tenantContext;

    public AuditSaveChangesInterceptor(
        ICurrentUserService currentUserService,
        ITenantContext tenantContext)
    {
        _currentUserService = currentUserService;
        _tenantContext = tenantContext;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AddAuditLogs(eventData.Context);

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    private void AddAuditLogs(DbContext? context)
    {
        if (context == null)
            return;

        var userId = _currentUserService.UserId;

        var entries = context.ChangeTracker
            .Entries()
            .Where(entry =>
                entry.Entity is not AuditLog &&
                entry.Entity is not Tenant &&
                entry.State is EntityState.Added or
                    EntityState.Modified or
                    EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            var auditTenantId = _tenantContext.TenantId ??
                (entry.Entity as TenantMembership)?.TenantId;
            if (auditTenantId is not > 0)
                continue;

            var primaryKey = entry.Metadata.FindPrimaryKey();
            var entityId = primaryKey == null
                ? "Unknown"
                : string.Join(
                    "|",
                    primaryKey.Properties.Select(property =>
                        entry.Property(property.Name).CurrentValue?.ToString()
                        ?? entry.Property(property.Name).OriginalValue?.ToString()
                        ?? "Unknown"));

            var auditEntry = context.Set<AuditLog>().Add(new AuditLog(
                entry.Metadata.ClrType.Name,
                entityId,
                entry.State.ToString(),
                userId));
            auditEntry.Property("TenantId").CurrentValue = auditTenantId.Value;
        }
    }
}
