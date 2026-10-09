using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class GoodsReceiptConfiguration : IEntityTypeConfiguration<GoodsReceipt>, IEntityTypeConfiguration<GoodsReceiptLine>
{
    public void Configure(EntityTypeBuilder<GoodsReceipt> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ReceiptNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.PostedByUserId).HasMaxLength(450);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => x.ReceiptNumber).IsUnique();
        builder.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.GoodsReceiptId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<GoodsReceiptLine> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ReceivedQuantity).HasPrecision(18, 6);
        builder.Property(x => x.UnitFactorToBase).HasPrecision(24, 12);
        builder.Property(x => x.StockQuantity).HasPrecision(18, 6);
        builder.Property(x => x.UnitCost).HasPrecision(24, 8);
        builder.Property(x => x.ProductCodeSnapshot).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ProductNameSnapshot).HasMaxLength(250).IsRequired();
        builder.Property(x => x.UnitNameSnapshot).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LotNumber).HasMaxLength(100);
        builder.Property(x => x.SerialNumbers).HasMaxLength(2000);
        builder.HasIndex(x => new { x.GoodsReceiptId, x.PurchaseOrderLineId }).IsUnique();
        builder.HasOne<PurchaseOrderLine>().WithMany().HasForeignKey(x => x.PurchaseOrderLineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<MeasurementUnit>().WithMany().HasForeignKey(x => x.MeasurementUnitId).OnDelete(DeleteBehavior.Restrict);
    }
}
