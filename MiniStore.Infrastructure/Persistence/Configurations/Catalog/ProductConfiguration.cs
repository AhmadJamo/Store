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
            .HasMaxLength(100);

        builder.Property(x => x.ProductCode)
            .HasMaxLength(20)
            .HasComputedColumnSql("'PRD-' + RIGHT('00000000' + CONVERT(varchar(8), [Id]), 8)", stored: true);

        builder.HasIndex(x => x.ProductCode).IsUnique();
        builder.HasIndex(x => x.Barcode)
            .IsUnique()
            .HasFilter("[Barcode] IS NOT NULL");

        builder.Property(x => x.PurchasePrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.SalePrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.WholesalePrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.InventoryBehavior).IsRequired();
        builder.Property(x => x.ProductType).IsRequired();
        builder.Property(x => x.StockUnit).IsRequired();
        builder.Property(x => x.AllowNegativeRecipeConsumption).IsRequired();
        builder.Property(x => x.IsSellableInPos).IsRequired();
        builder.Property(x => x.IsSellableInSales).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasOne<MeasurementUnit>()
            .WithMany()
            .HasForeignKey(x => x.MeasurementUnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
