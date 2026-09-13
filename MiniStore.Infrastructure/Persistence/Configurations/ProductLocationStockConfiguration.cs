using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;
namespace MiniStore.Infrastructure.Persistence.Configurations;
public class ProductLocationStockConfiguration : IEntityTypeConfiguration<ProductLocationStock>
{
    public void Configure(EntityTypeBuilder<ProductLocationStock> builder)
    {
        builder.HasKey(x => x.Id); builder.Property(x => x.Quantity).HasPrecision(18,3); builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.ProductId, x.StorageLocationId }).IsUnique();
        builder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<StorageLocation>().WithMany().HasForeignKey(x => x.StorageLocationId).OnDelete(DeleteBehavior.Restrict);
    }
}
