using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasPrecision(18, 6);
        builder.Property(x => x.Unit).IsRequired();
        builder.Property(x => x.StockQuantity).HasPrecision(18, 6);
        builder.Property(x => x.StockUnitSnapshot).IsRequired();
        builder.Property(x => x.UnitCodeSnapshot).HasMaxLength(20);
        builder.Property(x => x.StockUnitCodeSnapshot).HasMaxLength(20);
        builder.Property(x => x.UnitFactorSnapshot).HasPrecision(24, 12);
        builder.Property(x => x.StockUnitFactorSnapshot).HasPrecision(24, 12);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.IngredientProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<MeasurementUnit>()
            .WithMany()
            .HasForeignKey(x => x.MeasurementUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<MeasurementUnit>()
            .WithMany()
            .HasForeignKey(x => x.StockMeasurementUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ProductRecipeId, x.IngredientProductId })
            .IsUnique();
    }
}
