using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>, IEntityTypeConfiguration<PurchaseOrderLine>, IEntityTypeConfiguration<PurchaseOrderLifecycleHistory>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.OrderNumber).HasMaxLength(50).IsRequired();
        b.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired(); b.Property(x => x.CompanyCurrencyCode).HasMaxLength(3).IsRequired();
        b.Property(x => x.ExchangeRate).HasPrecision(24, 12); b.Property(x => x.ExchangeRateSource).HasMaxLength(100).IsRequired();
        b.Property(x => x.PaymentTermsSnapshot).HasMaxLength(500); b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired();
        b.Property(x => x.ApprovedByUserId).HasMaxLength(450); b.Property(x => x.ConfirmedByUserId).HasMaxLength(450);
        b.Property(x => x.CancellationReason).HasMaxLength(1000); b.Property(x => x.ClosureReason).HasMaxLength(1000);
        b.Property(x => x.ClosedByUserId).HasMaxLength(450); b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => x.OrderNumber).IsUnique(); b.HasIndex(x => x.SupplierQuotationId).IsUnique().HasFilter("[PurchaseAgreementId] IS NULL");
        b.HasOne<PurchaseSourcingEvent>().WithMany().HasForeignKey(x => x.PurchaseSourcingEventId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<SupplierQuotation>().WithMany().HasForeignKey(x => x.SupplierQuotationId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PurchaseAgreement>().WithMany().HasForeignKey(x => x.PurchaseAgreementId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.History).WithOne().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<PurchaseOrderLine> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.OrderedQuantity).HasPrecision(18, 6); b.Property(x => x.UnitFactorToBase).HasPrecision(24, 12);
        b.Property(x => x.StockQuantity).HasPrecision(18, 6); b.Property(x => x.UnitPrice).HasPrecision(24, 8);
        b.Property(x => x.DiscountPercent).HasPrecision(9, 4); b.Property(x => x.TaxPercent).HasPrecision(9, 4);
        b.Property(x => x.ProductCodeSnapshot).HasMaxLength(100).IsRequired(); b.Property(x => x.ProductNameSnapshot).HasMaxLength(250).IsRequired(); b.Property(x => x.UnitNameSnapshot).HasMaxLength(100).IsRequired();
        b.HasIndex(x => new { x.PurchaseOrderId, x.SupplierQuotationLineId }).IsUnique();
        b.HasOne<SupplierQuotationLine>().WithMany().HasForeignKey(x => x.SupplierQuotationLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PurchaseAgreementLine>().WithMany().HasForeignKey(x => x.PurchaseAgreementLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<MeasurementUnit>().WithMany().HasForeignKey(x => x.MeasurementUnitId).OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<PurchaseOrderLifecycleHistory> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.UserId).HasMaxLength(450).IsRequired(); b.Property(x => x.Reason).HasMaxLength(1000);
        b.HasIndex(x => new { x.PurchaseOrderId, x.CreatedAtUtc });
    }
}
