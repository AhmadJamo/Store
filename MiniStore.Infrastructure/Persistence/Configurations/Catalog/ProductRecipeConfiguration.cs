using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class ProductRecipeConfiguration : IEntityTypeConfiguration<ProductRecipe>
{
    public void Configure(EntityTypeBuilder<ProductRecipe> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.YieldQuantity).HasPrecision(18, 6);
        builder.Property(x => x.CreatedByUserId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Ingredients)
            .WithOne()
            .HasForeignKey(x => x.ProductRecipeId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Ingredients)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => new { x.ProductId, x.VersionNumber })
            .IsUnique();
        builder.HasIndex(x => new { x.ProductId, x.IsActive })
            .IsUnique()
            .HasFilter("[IsActive] = 1");
    }
}
