using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", table =>
        {
            table.HasCheckConstraint("CK_Products_DefaultShelfLifeDays",
                "[DefaultShelfLifeDays] IS NULL OR ([DefaultShelfLifeDays] BETWEEN 1 AND 36500)");
            table.HasCheckConstraint("CK_Products_ExpirationWarningDays",
                "[ExpirationWarningDays] BETWEEN 0 AND 3650");
        });
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
        builder.Property(x => x.NetWeight).HasPrecision(18, 6);
        builder.Property(x => x.GrossWeight).HasPrecision(18, 6);
        builder.Property(x => x.Length).HasPrecision(18, 6);
        builder.Property(x => x.Width).HasPrecision(18, 6);
        builder.Property(x => x.Height).HasPrecision(18, 6);
        builder.Property(x => x.TrackingPolicy).IsRequired();
        builder.Property(x => x.ExpirationWarningDays).IsRequired().HasDefaultValue(30);
        builder.Property(x => x.HandlingRequirements).IsRequired();
        builder.Property(x => x.VariantSignature).HasMaxLength(64);
        builder.Property(x => x.VariantLabel).HasMaxLength(300);
        builder.HasIndex(x => new { x.ProductTemplateId, x.VariantSignature })
            .IsUnique().HasFilter("[ProductTemplateId] IS NOT NULL AND [VariantSignature] IS NOT NULL");

        builder.HasOne<ProductCategory>()
            .WithMany()
            .HasForeignKey(x => x.ProductCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ProductTemplate>()
            .WithMany()
            .HasForeignKey(x => x.ProductTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<MeasurementUnit>()
            .WithMany()
            .HasForeignKey(x => x.WeightMeasurementUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<MeasurementUnit>()
            .WithMany()
            .HasForeignKey(x => x.DimensionMeasurementUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<MeasurementUnit>()
            .WithMany()
            .HasForeignKey(x => x.MeasurementUnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
