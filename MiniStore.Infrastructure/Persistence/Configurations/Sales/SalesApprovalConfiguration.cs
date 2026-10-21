using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class SalesApprovalConfiguration : IEntityTypeConfiguration<SalesApprovalRule>,
    IEntityTypeConfiguration<SalesApprovalRuleStep>, IEntityTypeConfiguration<SalesOrderApproval>,
    IEntityTypeConfiguration<SalesOrderApprovalFinding>, IEntityTypeConfiguration<SalesOrderApprovalStep>
{
    public void Configure(EntityTypeBuilder<SalesApprovalRule> builder)
    {
        builder.ToTable("SalesApprovalRules", table => table.HasCheckConstraint("CK_SalesApprovalRules_ThresholdPriority", "[Threshold] >= 0 AND [Priority] BETWEEN 1 AND 9999"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Threshold).HasPrecision(18, 6);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.IsActive, x.Priority, x.Metric });
        builder.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Steps).WithOne().HasForeignKey(x => x.SalesApprovalRuleId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<SalesApprovalRuleStep> builder)
    {
        builder.ToTable("SalesApprovalRuleSteps");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ApproverRoleName).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.SalesApprovalRuleId, x.Sequence }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<SalesOrderApproval> builder)
    {
        builder.ToTable("SalesOrderApprovals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RequestedByUserId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.SalesOrderId, x.RequestedAtUtc });
        builder.HasIndex(x => x.SalesOrderId).HasFilter("[Status] = 2").IsUnique();
        builder.HasOne<SalesOrder>().WithMany().HasForeignKey(x => x.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Findings).WithOne().HasForeignKey(x => x.SalesOrderApprovalId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Steps).WithOne().HasForeignKey(x => x.SalesOrderApprovalId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<SalesOrderApprovalFinding> builder)
    {
        builder.ToTable("SalesOrderApprovalFindings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RuleNameSnapshot).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Threshold).HasPrecision(18, 6);
        builder.Property(x => x.ActualValue).HasPrecision(18, 6);
        builder.HasOne<SalesApprovalRule>().WithMany().HasForeignKey(x => x.SalesApprovalRuleId).OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<SalesOrderApprovalStep> builder)
    {
        builder.ToTable("SalesOrderApprovalSteps");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ApproverRoleName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.DecidedByUserId).HasMaxLength(450);
        builder.Property(x => x.Note).HasMaxLength(500);
        builder.HasIndex(x => new { x.SalesOrderApprovalId, x.Sequence }).IsUnique();
    }
}
