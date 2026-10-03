using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.IdempotencyKey).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.PostedByUserId).HasMaxLength(450);
        builder.Property(x => x.ReversedByUserId).HasMaxLength(450);
        builder.Property(x => x.SourceDocumentType).HasMaxLength(40);
        builder.Property(x => x.Reference).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasIndex(x => x.IdempotencyKey).IsUnique();
        builder.HasIndex(x => new { x.WarehouseId, x.PostedAt });
        builder.HasIndex(x => new { x.ProductId, x.PostedAt });
        builder.HasIndex(x => x.LocationMovementId).IsUnique().HasFilter("[LocationMovementId] IS NOT NULL");
        builder.HasIndex(x => new { x.SourceDocumentType, x.SourceDocumentId, x.SourceLineId, x.StageSequence }).IsUnique()
            .HasFilter("[SourceDocumentType] IS NOT NULL AND [SourceDocumentId] IS NOT NULL");
        builder.HasOne<LocationMovement>().WithMany().HasForeignKey(x => x.LocationMovementId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.RelatedWarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<StorageLocation>().WithMany().HasForeignKey(x => x.FromStorageLocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<StorageLocation>().WithMany().HasForeignKey(x => x.ToStorageLocationId).OnDelete(DeleteBehavior.Restrict);
    }
}
