using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class ProductLocationStockConfiguration : IEntityTypeConfiguration<ProductLocationStock>
{
    public void Configure(EntityTypeBuilder<ProductLocationStock> builder)
    {
        builder.HasKey(stock => stock.Id);

        builder.Property(stock => stock.Quantity)
            .HasPrecision(18, 3);

        builder.Property(stock => stock.RowVersion)
            .IsRowVersion();

        builder.HasIndex(stock => new
            {
                stock.ProductId,
                stock.StorageLocationId
            })
            .IsUnique();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(stock => stock.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Warehouse>()
            .WithMany()
            .HasForeignKey(stock => stock.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<StorageLocation>()
            .WithMany()
            .HasForeignKey(stock => stock.StorageLocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
