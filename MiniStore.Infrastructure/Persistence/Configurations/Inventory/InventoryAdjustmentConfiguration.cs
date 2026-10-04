using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;
namespace MiniStore.Infrastructure.Persistence.Configurations;
public sealed class InventoryAdjustmentConfiguration : IEntityTypeConfiguration<InventoryAdjustment>
{
    public void Configure(EntityTypeBuilder<InventoryAdjustment> b)
    {
        b.ToTable("InventoryAdjustments"); b.HasKey(x => x.Id);
        b.Property(x => x.AdjustmentNumber).HasMaxLength(50).IsRequired(); b.Property(x => x.Reason).HasMaxLength(250).IsRequired();
        b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.CountedByUserId).HasMaxLength(450);
        b.Property(x => x.ApprovedByUserId).HasMaxLength(450); b.Property(x => x.PostedByUserId).HasMaxLength(450); b.Property(x => x.CancelledByUserId).HasMaxLength(450);
        b.Property(x => x.RowVersion).IsRowVersion(); b.HasIndex(x => x.AdjustmentNumber).IsUnique(); b.HasIndex(x => new { x.WarehouseId, x.Status, x.CreatedAt });
        b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<StorageLocation>().WithMany().HasForeignKey(x => x.StorageLocationId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.InventoryAdjustmentId).OnDelete(DeleteBehavior.Restrict);
        b.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
public sealed class InventoryAdjustmentLineConfiguration : IEntityTypeConfiguration<InventoryAdjustmentLine>
{
    public void Configure(EntityTypeBuilder<InventoryAdjustmentLine> b)
    {
        b.ToTable("InventoryAdjustmentLines", t => t.HasCheckConstraint("CK_InventoryAdjustmentLines_Counted", "[CountedQuantity] IS NULL OR [CountedQuantity] >= 0"));
        b.HasKey(x => x.Id); b.Property(x => x.ExpectedQuantity).HasPrecision(18, 6); b.Property(x => x.CountedQuantity).HasPrecision(18, 6); b.Ignore(x => x.VarianceQuantity);
        b.HasIndex(x => new { x.InventoryAdjustmentId, x.ProductId }).IsUnique();
        b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}
