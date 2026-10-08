using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class PurchaseSourcingConfiguration : IEntityTypeConfiguration<PurchaseSourcingEvent>,
    IEntityTypeConfiguration<PurchaseSourcingLine>, IEntityTypeConfiguration<PurchaseSupplierInvitation>
{
    public void Configure(EntityTypeBuilder<PurchaseSourcingEvent> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SourcingNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.SentByUserId).HasMaxLength(450);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => x.SourcingNumber).IsUnique();
        builder.HasIndex(x => x.PurchaseRequestId).IsUnique();
        builder.HasOne<PurchaseRequest>().WithMany().HasForeignKey(x => x.PurchaseRequestId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.PurchaseSourcingEventId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Invitations).WithOne().HasForeignKey(x => x.PurchaseSourcingEventId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<PurchaseSourcingLine> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RequestedQuantity).HasPrecision(18, 6);
        builder.Property(x => x.UnitFactorToBase).HasPrecision(24, 12);
        builder.Property(x => x.StockQuantity).HasPrecision(18, 6);
        builder.Property(x => x.ProductCodeSnapshot).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ProductNameSnapshot).HasMaxLength(250).IsRequired();
        builder.Property(x => x.UnitNameSnapshot).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasIndex(x => new { x.PurchaseSourcingEventId, x.PurchaseRequestLineId }).IsUnique();
        builder.HasOne<PurchaseRequestLine>().WithMany().HasForeignKey(x => x.PurchaseRequestLineId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<MeasurementUnit>().WithMany().HasForeignKey(x => x.MeasurementUnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<PurchaseSupplierInvitation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Message).HasMaxLength(1000);
        builder.HasIndex(x => new { x.PurchaseSourcingEventId, x.SupplierId }).IsUnique();
        builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
    }
}
