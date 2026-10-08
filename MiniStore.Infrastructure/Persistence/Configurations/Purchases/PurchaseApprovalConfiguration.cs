using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class PurchaseApprovalConfiguration : IEntityTypeConfiguration<PurchaseApprovalRule>,
    IEntityTypeConfiguration<PurchaseApprovalRuleStep>, IEntityTypeConfiguration<PurchaseApprovalInstance>,
    IEntityTypeConfiguration<PurchaseApprovalStep>
{
    public void Configure(EntityTypeBuilder<PurchaseApprovalRule> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => new { x.WarehouseId, x.MinimumPriority, x.MaximumPriority, x.IsActive });
        b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Steps).WithOne().HasForeignKey(x => x.PurchaseApprovalRuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<PurchaseApprovalRuleStep> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.ApproverRoleName).HasMaxLength(100).IsRequired();
        b.HasIndex(x => new { x.PurchaseApprovalRuleId, x.Sequence }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<PurchaseApprovalInstance> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.RuleNameSnapshot).HasMaxLength(150).IsRequired();
        b.Property(x => x.RequestedByUserId).HasMaxLength(450).IsRequired();
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => x.PurchaseRequestId).IsUnique();
        b.HasOne<PurchaseRequest>().WithMany().HasForeignKey(x => x.PurchaseRequestId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PurchaseApprovalRule>().WithMany().HasForeignKey(x => x.PurchaseApprovalRuleId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Steps).WithOne().HasForeignKey(x => x.PurchaseApprovalInstanceId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<PurchaseApprovalStep> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.ApproverRoleName).HasMaxLength(100).IsRequired();
        b.Property(x => x.DecidedByUserId).HasMaxLength(450);
        b.Property(x => x.Note).HasMaxLength(500);
        b.HasIndex(x => new { x.PurchaseApprovalInstanceId, x.Sequence }).IsUnique();
    }
}
