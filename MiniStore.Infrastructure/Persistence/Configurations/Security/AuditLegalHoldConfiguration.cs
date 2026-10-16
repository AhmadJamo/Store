using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class AuditLegalHoldConfiguration : IEntityTypeConfiguration<AuditLegalHold>
{
    public void Configure(EntityTypeBuilder<AuditLegalHold> builder)
    {
        builder.ToTable("AuditLegalHolds", table =>
        {
            table.HasCheckConstraint("CK_AuditLegalHolds_ScopeTenant",
                "([Scope] = 1 AND [TenantId] IS NOT NULL AND [TenantId] > 0) OR ([Scope] IN (2, 3) AND [TenantId] IS NULL)");
            table.HasCheckConstraint("CK_AuditLegalHolds_Range", "[ToUtc] >= [FromUtc]");
        });
        builder.HasKey(value => value.Id);
        builder.Property(value => value.HoldId).IsRequired();
        builder.Property(value => value.ReasonRedacted).IsRequired().HasMaxLength(500);
        builder.Property(value => value.CreatedBy).IsRequired().HasMaxLength(450);
        builder.Property(value => value.ReleasedBy).HasMaxLength(450);
        builder.Property(value => value.ReleaseReasonRedacted).HasMaxLength(500);
        builder.HasIndex(value => value.HoldId).IsUnique();
        builder.HasIndex(value => new { value.IsActive, value.Scope, value.TenantId, value.FromUtc, value.ToUtc });
        builder.HasOne<Tenant>().WithMany().HasForeignKey(value => value.TenantId).OnDelete(DeleteBehavior.Restrict);
    }
}
