using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class SupplierProductPurchasingInfoConfiguration
    : IEntityTypeConfiguration<SupplierProductPurchasingInfo>
{
    public void Configure(EntityTypeBuilder<SupplierProductPurchasingInfo> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property<int>("TenantId");
        builder.Property(x => x.SupplierProductCode).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SupplierDescription).HasMaxLength(300);
        builder.Property(x => x.MinimumOrderQuantity).HasPrecision(18, 6);
        builder.Property(x => x.OrderMultiple).HasPrecision(18, 6);
        builder.Property(x => x.UnitPrice).HasPrecision(19, 4);
        builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(3).IsUnicode(false);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.SupplierId, x.ProductId, x.PurchaseMeasurementUnitId }).IsUnique();
        builder.HasIndex("TenantId", nameof(SupplierProductPurchasingInfo.ProductId),
            nameof(SupplierProductPurchasingInfo.IsActive), nameof(SupplierProductPurchasingInfo.Priority));
        builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<MeasurementUnit>().WithMany().HasForeignKey(x => x.PurchaseMeasurementUnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
