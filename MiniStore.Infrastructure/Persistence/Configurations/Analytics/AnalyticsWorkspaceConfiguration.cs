using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class AnalyticsWorkspaceConfiguration : IEntityTypeConfiguration<AnalyticsWorkspace>, IEntityTypeConfiguration<AnalyticsWorkspaceWidget>, IEntityTypeConfiguration<AnalyticsWorkspaceDelegation>
{
    public void Configure(EntityTypeBuilder<AnalyticsWorkspace> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.Name).HasMaxLength(150).IsRequired(); b.Property(x => x.NameArabic).HasMaxLength(150);
        b.Property(x => x.Description).HasMaxLength(1000); b.Property(x => x.ModuleKey).HasMaxLength(50).IsRequired();
        b.Property(x => x.OwnerUserId).HasMaxLength(450); b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired();
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => new { x.ModuleKey, x.Visibility }); b.HasIndex(x => new { x.OwnerUserId, x.ModuleKey });
        b.HasMany(x => x.Widgets).WithOne().HasForeignKey(x => x.AnalyticsWorkspaceId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.Delegations).WithOne().HasForeignKey(x => x.AnalyticsWorkspaceId).OnDelete(DeleteBehavior.Cascade);
    }
    public void Configure(EntityTypeBuilder<AnalyticsWorkspaceWidget> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.WidgetKey).HasMaxLength(100).IsRequired();
        b.Property(x => x.TitleOverride).HasMaxLength(150); b.Property(x => x.TitleArabicOverride).HasMaxLength(150);
        b.Property(x => x.TargetValue).HasPrecision(24, 8); b.Property(x => x.AccentColor).HasMaxLength(7);
        b.HasIndex(x => new { x.AnalyticsWorkspaceId, x.WidgetKey }).IsUnique();
        b.HasIndex(x => new { x.AnalyticsWorkspaceId, x.Sequence }).IsUnique();
    }
    public void Configure(EntityTypeBuilder<AnalyticsWorkspaceDelegation> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.TargetUserId).HasMaxLength(450);
        b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.RevokedByUserId).HasMaxLength(450);
        b.HasIndex(x => new { x.AnalyticsWorkspaceId, x.TargetType, x.TargetUserId });
        b.HasIndex(x => new { x.AnalyticsWorkspaceId, x.TargetType, x.TargetRoleId });
        b.HasIndex(x => new { x.TargetUserId, x.RevokedAtUtc, x.ExpiresAtUtc });
        b.HasIndex(x => new { x.TargetRoleId, x.RevokedAtUtc, x.ExpiresAtUtc });
        b.HasOne<TenantRole>().WithMany()
            .HasForeignKey(nameof(AnalyticsWorkspaceDelegation.TargetRoleId), "TenantId")
            .HasPrincipalKey(nameof(TenantRole.Id), nameof(TenantRole.TenantId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}
