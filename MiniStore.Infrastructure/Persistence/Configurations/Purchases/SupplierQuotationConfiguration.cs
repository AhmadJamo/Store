using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class SupplierQuotationConfiguration : IEntityTypeConfiguration<SupplierQuotation>,
    IEntityTypeConfiguration<SupplierQuotationLine>
{
    public void Configure(EntityTypeBuilder<SupplierQuotation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.QuotationNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.SupplierReference).HasMaxLength(100);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.PaymentTermsSnapshot).HasMaxLength(500);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => x.QuotationNumber).IsUnique();
        builder.HasIndex(x => new { x.PurchaseSourcingEventId, x.SupplierId }).IsUnique();
        builder.HasOne<PurchaseSourcingEvent>().WithMany().HasForeignKey(x => x.PurchaseSourcingEventId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.SupplierQuotationId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<SupplierQuotationLine> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.QuotedQuantity).HasPrecision(18, 6);
        builder.Property(x => x.UnitPrice).HasPrecision(24, 8);
        builder.Property(x => x.DiscountPercent).HasPrecision(9, 4);
        builder.Property(x => x.TaxPercent).HasPrecision(9, 4);
        builder.Property(x => x.ProductCodeSnapshot).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ProductNameSnapshot).HasMaxLength(250).IsRequired();
        builder.Property(x => x.UnitNameSnapshot).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.SupplierQuotationId, x.PurchaseSourcingLineId }).IsUnique();
        builder.HasOne<PurchaseSourcingLine>().WithMany().HasForeignKey(x => x.PurchaseSourcingLineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
