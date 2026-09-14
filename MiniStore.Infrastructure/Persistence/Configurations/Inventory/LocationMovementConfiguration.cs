using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class LocationMovementConfiguration : IEntityTypeConfiguration<LocationMovement>
{
    public void Configure(EntityTypeBuilder<LocationMovement> builder)
    {
        builder.ToTable("LocationMovements");
        builder.HasKey(movement => movement.Id);

        builder.Property(movement => movement.Quantity)
            .HasPrecision(18, 3);

        builder.Property(movement => movement.Reference)
            .HasMaxLength(100);

        builder.Property(movement => movement.Notes)
            .HasMaxLength(500);

        builder.Property(movement => movement.CreatedByUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.HasIndex(movement => new
        {
            movement.WarehouseId,
            movement.CreatedAt
        });

        builder.HasIndex(movement => new
        {
            movement.ProductId,
            movement.CreatedAt
        });

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(movement => movement.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Warehouse>()
            .WithMany()
            .HasForeignKey(movement => movement.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<StorageLocation>()
            .WithMany()
            .HasForeignKey(movement => movement.FromStorageLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<StorageLocation>()
            .WithMany()
            .HasForeignKey(movement => movement.ToStorageLocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
