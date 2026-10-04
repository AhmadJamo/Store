using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class InventoryReservationConfiguration : IEntityTypeConfiguration<InventoryReservation>
{
    public void Configure(EntityTypeBuilder<InventoryReservation> builder)
    {
        builder.ToTable("InventoryReservations"); builder.HasKey(x => x.Id);
        builder.Property(x => x.SourceReference).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.ClosedByUserId).HasMaxLength(450);
        builder.Property(x => x.ReleaseReason).HasMaxLength(250);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.SourceType, x.SourceId }).IsUnique();
        builder.HasIndex(x => new { x.Status, x.CreatedAt });
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.InventoryReservationId).OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class InventoryReservationLineConfiguration : IEntityTypeConfiguration<InventoryReservationLine>
{
    public void Configure(EntityTypeBuilder<InventoryReservationLine> builder)
    {
        builder.ToTable("InventoryReservationLines", table =>
            table.HasCheckConstraint("CK_InventoryReservationLines_Quantity", "[Quantity] > 0"));
        builder.HasKey(x => x.Id); builder.Property(x => x.Quantity).HasPrecision(18, 6);
        builder.HasIndex(x => new { x.InventoryReservationId, x.ProductId, x.WarehouseId, x.StorageLocationId }).IsUnique();
        builder.HasIndex(x => new { x.InventoryReservationId, x.ProductId, x.WarehouseId }).IsUnique()
            .HasFilter("[StorageLocationId] IS NULL");
        builder.HasIndex(x => new { x.WarehouseId, x.StorageLocationId, x.ProductId });
        builder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<StorageLocation>().WithMany().HasForeignKey(x => x.StorageLocationId).OnDelete(DeleteBehavior.Restrict);
    }
}
