using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Barcode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PurchasePrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.SalePrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.WholesalePrice)
            .HasPrecision(18, 2);
    }
}
