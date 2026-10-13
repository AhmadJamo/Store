using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class SupplierCreditNoteConfiguration : IEntityTypeConfiguration<SupplierCreditNote>, IEntityTypeConfiguration<SupplierCreditNoteLine>
{
    public void Configure(EntityTypeBuilder<SupplierCreditNote> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.CreditNoteNumber).HasMaxLength(50).IsRequired();
        b.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired(); b.Property(x => x.SupplierReference).HasMaxLength(100);
        b.Property(x => x.Reason).HasMaxLength(1000).IsRequired(); b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired();
        b.Property(x => x.PostedByUserId).HasMaxLength(450); b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => x.CreditNoteNumber).IsUnique();
        b.HasOne<VendorBill>().WithMany().HasForeignKey(x => x.VendorBillId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.SupplierCreditNoteId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<SupplierCreditNoteLine> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.Quantity).HasPrecision(18, 6); b.Property(x => x.UnitFactorToBase).HasPrecision(24, 12);
        b.Property(x => x.StockQuantity).HasPrecision(18, 6); b.Property(x => x.NetAmount).HasPrecision(18, 2);
        b.Property(x => x.TaxAmount).HasPrecision(18, 2); b.Property(x => x.GrossAmount).HasPrecision(18, 2);
        b.Property(x => x.RemovedInventoryCost).HasPrecision(18, 2); b.Property(x => x.ProductCodeSnapshot).HasMaxLength(100).IsRequired();
        b.Property(x => x.ProductNameSnapshot).HasMaxLength(250).IsRequired(); b.Property(x => x.UnitNameSnapshot).HasMaxLength(100).IsRequired();
        b.Property(x => x.TrackingAllocations).HasMaxLength(2000); b.HasIndex(x => new { x.SupplierCreditNoteId, x.VendorBillLineId }).IsUnique();
        b.HasOne<VendorBillLine>().WithMany().HasForeignKey(x => x.VendorBillLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<GoodsReceiptLine>().WithMany().HasForeignKey(x => x.GoodsReceiptLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Account>().WithMany().HasForeignKey(x => x.TaxInputAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}
