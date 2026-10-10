using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;
namespace MiniStore.Infrastructure.Persistence.Configurations;
public sealed class PurchaseMatchConfiguration : IEntityTypeConfiguration<PurchaseMatchRun>, IEntityTypeConfiguration<PurchaseMatchException>, IEntityTypeConfiguration<PurchaseMatchingSettings>
{
    public void Configure(EntityTypeBuilder<PurchaseMatchRun> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.QuantityTolerancePercent).HasPrecision(9,4); b.Property(x => x.PriceTolerancePercent).HasPrecision(9,4);
        b.Property(x => x.RunByUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.OverrideReason).HasMaxLength(1000); b.Property(x => x.OverriddenByUserId).HasMaxLength(450);
        b.HasOne<VendorBill>().WithOne().HasForeignKey<PurchaseMatchRun>(x => x.VendorBillId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Exceptions).WithOne().HasForeignKey(x => x.PurchaseMatchRunId).OnDelete(DeleteBehavior.Cascade);
    }
    public void Configure(EntityTypeBuilder<PurchaseMatchException> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.ExpectedValue).HasPrecision(24,8); b.Property(x => x.ActualValue).HasPrecision(24,8);
        b.Property(x => x.VariancePercent).HasPrecision(18,6); b.Property(x => x.TolerancePercent).HasPrecision(9,4);
        b.Property(x => x.ProductCodeSnapshot).HasMaxLength(100).IsRequired(); b.Property(x => x.ProductNameSnapshot).HasMaxLength(250).IsRequired();
        b.HasIndex(x => new { x.PurchaseMatchRunId, x.VendorBillLineId, x.Type }).IsUnique();
        b.HasOne<VendorBillLine>().WithMany().HasForeignKey(x => x.VendorBillLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PurchaseOrderLine>().WithMany().HasForeignKey(x => x.PurchaseOrderLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<GoodsReceiptLine>().WithMany().HasForeignKey(x => x.GoodsReceiptLineId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<PurchaseMatchingSettings> b)
    {
        b.HasKey(x => x.Id); b.HasIndex(x => x.SingletonKey).IsUnique(); b.Property(x => x.QuantityTolerancePercent).HasPrecision(9,4);
        b.Property(x => x.PriceTolerancePercent).HasPrecision(9,4); b.Property(x => x.RowVersion).IsRowVersion();
    }
}
