using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class GoodsReceiptReturnConfiguration : IEntityTypeConfiguration<GoodsReceiptReturn>, IEntityTypeConfiguration<GoodsReceiptReturnLine>
{
    public void Configure(EntityTypeBuilder<GoodsReceiptReturn> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.ReturnNumber).HasMaxLength(50).IsRequired();
        b.Property(x => x.Reason).HasMaxLength(1000).IsRequired(); b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired();
        b.Property(x => x.PostedByUserId).HasMaxLength(450); b.Property(x => x.RowVersion).IsRowVersion(); b.HasIndex(x => x.ReturnNumber).IsUnique();
        b.HasOne<GoodsReceipt>().WithMany().HasForeignKey(x => x.GoodsReceiptId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.GoodsReceiptReturnId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<GoodsReceiptReturnLine> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.ReturnQuantity).HasPrecision(18, 6); b.Property(x => x.UnitFactorToBase).HasPrecision(24, 12);
        b.Property(x => x.StockQuantity).HasPrecision(18, 6); b.Property(x => x.OriginalUnitCost).HasPrecision(24, 8);
        b.Property(x => x.OriginalInventoryValue).HasPrecision(18, 2); b.Property(x => x.RemovedInventoryCost).HasPrecision(18, 2);
        b.Property(x => x.ProductCodeSnapshot).HasMaxLength(100).IsRequired(); b.Property(x => x.ProductNameSnapshot).HasMaxLength(250).IsRequired();
        b.Property(x => x.UnitNameSnapshot).HasMaxLength(100).IsRequired(); b.Property(x => x.TrackingAllocations).HasMaxLength(2000);
        b.HasIndex(x => new { x.GoodsReceiptReturnId, x.GoodsReceiptLineId }).IsUnique();
        b.HasOne<GoodsReceiptLine>().WithMany().HasForeignKey(x => x.GoodsReceiptLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}
