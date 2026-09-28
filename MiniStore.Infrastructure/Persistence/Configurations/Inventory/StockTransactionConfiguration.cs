using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class StockTransactionConfiguration
    : IEntityTypeConfiguration<StockTransaction>
{
    public void Configure(
        EntityTypeBuilder<StockTransaction> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 6);

        builder.Property(x => x.QuantityBefore).HasPrecision(18, 6);
        builder.Property(x => x.QuantityAfter).HasPrecision(18, 6);
        builder.Property(x => x.AverageUnitCostBefore).HasPrecision(24, 8);
        builder.Property(x => x.AverageUnitCostAfter).HasPrecision(24, 8);
        builder.Property(x => x.InventoryValueBefore).HasPrecision(24, 8);
        builder.Property(x => x.InventoryValueAfter).HasPrecision(24, 8);
        builder.Property(x => x.UnitCost).HasPrecision(24, 8);
        builder.Property(x => x.TransactionValue).HasPrecision(24, 8);
        builder.Property(x => x.CostVariance).HasPrecision(24, 8);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Reference)
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Warehouse>()
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.ProductId,
            x.WarehouseId
        });
    }
}
