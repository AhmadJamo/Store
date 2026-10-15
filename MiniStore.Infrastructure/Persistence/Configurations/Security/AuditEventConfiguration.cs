using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
{
    public void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        builder.ToTable("AuditEvents", table =>
        {
            table.HasCheckConstraint(
                "CK_AuditEvents_ScopeTenant",
                "([Scope] = 1 AND [TenantId] IS NOT NULL AND [TenantId] > 0) OR ([Scope] IN (2, 3) AND [TenantId] IS NULL)");
            table.HasCheckConstraint(
                "CK_AuditEvents_Classification",
                "[SchemaVersion] = 1 AND [Category] BETWEEN 1 AND 6 AND [Severity] BETWEEN 1 AND 3 AND [Outcome] BETWEEN 1 AND 4 AND [ActorType] BETWEEN 1 AND 4 AND [SourceChannel] BETWEEN 1 AND 6");
            table.HasCheckConstraint(
                "CK_AuditEvents_Json",
                "([ChangesRedactedJson] IS NULL OR ISJSON([ChangesRedactedJson]) = 1) AND ([MetadataRedactedJson] IS NULL OR ISJSON([MetadataRedactedJson]) = 1)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventId).IsRequired();
        builder.Property(x => x.SchemaVersion).IsRequired();
        builder.Property(x => x.EventType).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Action).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ActorId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.CorrelationId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.CausationId).HasMaxLength(100);
        builder.Property(x => x.OperationId).HasMaxLength(100);
        builder.Property(x => x.ResourceType).IsRequired().HasMaxLength(150);
        builder.Property(x => x.ResourceId).IsRequired().HasMaxLength(150);
        builder.Property(x => x.ReasonCode).HasMaxLength(100);
        builder.Property(x => x.ReasonRedacted).HasMaxLength(1000);
        builder.Property(x => x.ChangesRedactedJson).HasMaxLength(16000);
        builder.Property(x => x.MetadataRedactedJson).HasMaxLength(8000);
        builder.Property(x => x.RequestPathTemplate).HasMaxLength(250);
        builder.Property(x => x.SourceIpMasked).HasMaxLength(100);
        builder.Property(x => x.UserAgentHash).HasMaxLength(64);
        builder.Property(x => x.IntegrityKeyVersion).IsRequired().HasMaxLength(50);
        builder.Property(x => x.IntegrityHash).IsRequired().HasMaxLength(64).IsFixedLength();
        builder.HasIndex(x => x.EventId).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.OccurredAtUtc, x.Id });
        builder.HasIndex(x => x.CorrelationId);
        builder.HasIndex(x => new { x.ResourceType, x.ResourceId, x.OccurredAtUtc });
        builder.HasIndex(x => new { x.Scope, x.TenantId, x.OperationId }).IsUnique().HasFilter("[OperationId] IS NOT NULL");
        builder.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
    }
}
