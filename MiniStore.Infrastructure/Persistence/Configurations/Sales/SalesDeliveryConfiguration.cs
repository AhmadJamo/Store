using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class SalesDeliveryConfiguration : IEntityTypeConfiguration<SalesDelivery>,
    IEntityTypeConfiguration<SalesDeliveryLine>, IEntityTypeConfiguration<SalesDeliveryHistory>
{
    public void Configure(EntityTypeBuilder<SalesDelivery> b)
    {
        b.ToTable("SalesDeliveries"); b.HasKey(x => x.Id);
        b.Property(x => x.DeliveryNumber).HasMaxLength(50).IsRequired(); b.HasIndex(x => x.DeliveryNumber).IsUnique();
        b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.PostedByUserId).HasMaxLength(450);
        b.Property(x => x.CancelledByUserId).HasMaxLength(450); b.Property(x => x.CancellationReason).HasMaxLength(500);
        b.Property(x => x.Notes).HasMaxLength(1000); b.Property(x => x.RowVersion).IsRowVersion();
        b.Ignore(x => x.TotalCostOfGoodsSold);
        b.HasIndex(x => new { x.SalesOrderId, x.Status });
        b.HasIndex(x => x.InventoryReservationId).IsUnique().HasFilter("[Status] = 1");
        b.HasOne<SalesOrder>().WithMany().HasForeignKey(x => x.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<InventoryReservation>().WithMany().HasForeignKey(x => x.InventoryReservationId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.SalesDeliveryId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.History).WithOne().HasForeignKey(x => x.SalesDeliveryId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<SalesDeliveryLine> b)
    {
        b.ToTable("SalesDeliveryLines", t => t.HasCheckConstraint("CK_SalesDeliveryLines_Quantity", "[StockQuantity] > 0"));
        b.HasKey(x => x.Id); b.Property(x => x.StockQuantity).HasPrecision(18, 6); b.Property(x => x.UnitCost).HasPrecision(18, 8);
        b.Property(x => x.CostOfGoodsSold).HasPrecision(18, 8); b.Property(x => x.ProductCodeSnapshot).HasMaxLength(100).IsRequired();
        b.Property(x => x.ProductNameSnapshot).HasMaxLength(200).IsRequired(); b.Property(x => x.TrackingAllocations).HasMaxLength(4000);
        b.HasIndex(x => new { x.SalesDeliveryId, x.InventoryReservationLineId }).IsUnique();
        b.HasOne<SalesOrderLine>().WithMany().HasForeignKey(x => x.SalesOrderLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<InventoryReservationLine>().WithMany().HasForeignKey(x => x.InventoryReservationLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<StorageLocation>().WithMany().HasForeignKey(x => x.StorageLocationId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<SalesDeliveryHistory> b)
    {
        b.ToTable("SalesDeliveryHistories"); b.HasKey(x => x.Id); b.Property(x => x.UserId).HasMaxLength(450).IsRequired();
        b.Property(x => x.Reason).HasMaxLength(500); b.HasIndex(x => new { x.SalesDeliveryId, x.CreatedAtUtc });
    }
}
