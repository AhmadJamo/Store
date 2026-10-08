using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class PurchaseRequestConfiguration : IEntityTypeConfiguration<PurchaseRequest>,
    IEntityTypeConfiguration<PurchaseRequestLine>, IEntityTypeConfiguration<PurchaseRequestHistory>
{
    public void Configure(EntityTypeBuilder<PurchaseRequest> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.RequestNumber).HasMaxLength(50).IsRequired();
        b.Property(x => x.Justification).HasMaxLength(500).IsRequired(); b.Property(x => x.Notes).HasMaxLength(1000);
        b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.SubmittedByUserId).HasMaxLength(450);
        b.Property(x => x.CancelledByUserId).HasMaxLength(450); b.Property(x => x.CancellationReason).HasMaxLength(500);
        b.Property(x => x.RowVersion).IsRowVersion(); b.HasIndex(x => x.RequestNumber).IsUnique();
        b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.PurchaseRequestId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.History).WithOne().HasForeignKey(x => x.PurchaseRequestId).OnDelete(DeleteBehavior.Cascade);
    }
    public void Configure(EntityTypeBuilder<PurchaseRequestLine> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.RequestedQuantity).HasPrecision(18,6);
        b.Property(x => x.UnitFactorToBase).HasPrecision(24,12); b.Property(x => x.StockQuantity).HasPrecision(18,6);
        b.Property(x => x.Notes).HasMaxLength(500);
        b.HasIndex(x => new { x.PurchaseRequestId, x.ProductId, x.MeasurementUnitId }).IsUnique();
        b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<MeasurementUnit>().WithMany().HasForeignKey(x => x.MeasurementUnitId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SuggestedSupplierId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<PurchaseRequestHistory> b)
    { b.HasKey(x => x.Id); b.Property(x => x.UserId).HasMaxLength(450).IsRequired(); b.Property(x => x.Reason).HasMaxLength(500); b.HasIndex(x => new { x.PurchaseRequestId, x.CreatedAtUtc }); }
}
