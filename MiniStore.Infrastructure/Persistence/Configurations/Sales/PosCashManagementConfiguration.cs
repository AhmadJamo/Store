using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class PosCashPolicyConfiguration : IEntityTypeConfiguration<PosCashPolicy>
{
    public void Configure(EntityTypeBuilder<PosCashPolicy> b)
    {
        b.HasKey(x=>x.Id); b.Property(x=>x.CurrencyCode).HasMaxLength(3).IsRequired(); b.Property(x=>x.VarianceTolerance).HasPrecision(18,2); b.Property(x=>x.MandatoryApprovalThreshold).HasPrecision(18,2); b.Property(x=>x.RowVersion).IsRowVersion(); b.HasIndex(x=>x.TenantId).IsUnique(); b.HasOne<Tenant>().WithMany().HasForeignKey(x=>x.TenantId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class PosCashDrawerConfiguration : IEntityTypeConfiguration<PosCashDrawer>
{
    public void Configure(EntityTypeBuilder<PosCashDrawer> b)
    {
        b.HasKey(x=>x.Id); b.HasAlternateKey(x=>new{x.Id,x.TenantId}); b.Property(x=>x.Code).HasMaxLength(40).IsRequired(); b.Property(x=>x.Name).HasMaxLength(120).IsRequired(); b.Property(x=>x.RowVersion).IsRowVersion(); b.HasIndex(x=>new{x.TenantId,x.Code}).IsUnique(); b.HasIndex(x=>new{x.TenantId,x.BranchId,x.PosTerminalId,x.IsActive}); b.HasOne<Tenant>().WithMany().HasForeignKey(x=>x.TenantId).OnDelete(DeleteBehavior.Restrict); b.HasOne<Branch>().WithMany().HasForeignKey(x=>new{x.BranchId,x.TenantId}).HasPrincipalKey("Id","TenantId").OnDelete(DeleteBehavior.Restrict); b.HasOne<PosTerminal>().WithMany().HasForeignKey(x=>new{x.PosTerminalId,x.TenantId}).HasPrincipalKey("Id","TenantId").OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class PosCashSessionConfiguration : IEntityTypeConfiguration<PosCashSession>
{
    public void Configure(EntityTypeBuilder<PosCashSession> b)
    {
        b.HasKey(x=>x.Id); b.HasAlternateKey(x=>new{x.Id,x.TenantId}); b.Property(x=>x.CurrencyCode).HasMaxLength(3).IsRequired(); b.Property(x=>x.OpenedByUserId).HasMaxLength(450).IsRequired(); b.Property(x=>x.ClosedByUserId).HasMaxLength(450); b.Property(x=>x.ApprovedByUserId).HasMaxLength(450); b.Property(x=>x.ApprovalReason).HasMaxLength(500); b.Property(x=>x.OpeningFloat).HasPrecision(18,2); b.Property(x=>x.ExpectedCash).HasPrecision(18,2); b.Property(x=>x.DeclaredCash).HasPrecision(18,2); b.Property(x=>x.Variance).HasPrecision(18,2); b.Property(x=>x.RowVersion).IsRowVersion(); b.HasIndex(x=>new{x.TenantId,x.PosTerminalId}).IsUnique().HasFilter("[Status] IN (1, 2, 3)"); b.HasIndex(x=>new{x.TenantId,x.BranchId,x.OpenedAtUtc}); b.HasOne<PosCashDrawer>().WithMany().HasForeignKey(x=>new{x.PosCashDrawerId,x.TenantId}).HasPrincipalKey("Id","TenantId").OnDelete(DeleteBehavior.Restrict); b.HasOne<Branch>().WithMany().HasForeignKey(x=>new{x.BranchId,x.TenantId}).HasPrincipalKey("Id","TenantId").OnDelete(DeleteBehavior.Restrict); b.HasOne<PosTerminal>().WithMany().HasForeignKey(x=>new{x.PosTerminalId,x.TenantId}).HasPrincipalKey("Id","TenantId").OnDelete(DeleteBehavior.Restrict); b.HasMany(x=>x.Movements).WithOne().HasForeignKey(x=>new{x.PosCashSessionId,x.TenantId}).HasPrincipalKey(x=>new{x.Id,x.TenantId}).OnDelete(DeleteBehavior.Restrict); b.HasMany(x=>x.Counts).WithOne().HasForeignKey(x=>new{x.PosCashSessionId,x.TenantId}).HasPrincipalKey(x=>new{x.Id,x.TenantId}).OnDelete(DeleteBehavior.Restrict); b.Navigation(x=>x.Movements).UsePropertyAccessMode(PropertyAccessMode.Field); b.Navigation(x=>x.Counts).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
public sealed class PosCashMovementConfiguration : IEntityTypeConfiguration<PosCashMovement>
{
    public void Configure(EntityTypeBuilder<PosCashMovement> b) { b.HasKey(x=>x.Id); b.Property(x=>x.Amount).HasPrecision(18,2); b.Property(x=>x.RecordedByUserId).HasMaxLength(450).IsRequired(); b.Property(x=>x.Reason).HasMaxLength(500); b.HasIndex(x=>new{x.TenantId,x.PosCashSessionId,x.OccurredAtUtc}); }
}
public sealed class PosCashCountConfiguration : IEntityTypeConfiguration<PosCashCount>
{
    public void Configure(EntityTypeBuilder<PosCashCount> b) { b.HasKey(x=>x.Id); b.Property(x=>x.DeclaredCash).HasPrecision(18,2); b.Property(x=>x.CountedByUserId).HasMaxLength(450).IsRequired(); b.HasIndex(x=>new{x.TenantId,x.PosCashSessionId,x.CountedAtUtc}); }
}
