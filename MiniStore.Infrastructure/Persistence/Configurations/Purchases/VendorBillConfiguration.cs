using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;
namespace MiniStore.Infrastructure.Persistence.Configurations;
public sealed class VendorBillConfiguration : IEntityTypeConfiguration<VendorBill>, IEntityTypeConfiguration<VendorBillLine>, IEntityTypeConfiguration<VendorBillAttachment>
{
    public void Configure(EntityTypeBuilder<VendorBill> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.BillNumber).HasMaxLength(50).IsRequired();
        b.Property(x => x.SupplierInvoiceNumber).HasMaxLength(100).IsRequired(); b.Property(x => x.NormalizedSupplierInvoiceNumber).HasMaxLength(100).IsRequired();
        b.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired(); b.Property(x => x.CompanyCurrencyCode).HasMaxLength(3).IsRequired(); b.Property(x => x.ExchangeRate).HasPrecision(24,12); b.Property(x => x.ExchangeRateSource).HasMaxLength(100).IsRequired(); b.Property(x => x.Notes).HasMaxLength(1000);
        b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.ApprovedByUserId).HasMaxLength(450); b.Property(x => x.PostedByUserId).HasMaxLength(450); b.Property(x => x.CancelledByUserId).HasMaxLength(450); b.Property(x => x.CancellationReason).HasMaxLength(1000); b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => x.BillNumber).IsUnique(); b.HasIndex(x => new { x.SupplierId, x.NormalizedSupplierInvoiceNumber }).IsUnique().HasFilter("[Status] <> 3");
        b.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.VendorBillId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.Attachments).WithOne().HasForeignKey(x => x.VendorBillId).OnDelete(DeleteBehavior.Cascade);
    }
    public void Configure(EntityTypeBuilder<VendorBillLine> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.Quantity).HasPrecision(18, 6); b.Property(x => x.UnitPrice).HasPrecision(24, 8); b.Property(x => x.ReceiptUnitCost).HasPrecision(24, 8);
        b.Property(x => x.ReceiptClearingQuantity).HasPrecision(18, 6); b.Property(x => x.BillingBasisQuantity).HasPrecision(18, 6);
        b.Property(x => x.TaxPercent).HasPrecision(9, 4); b.Property(x => x.NetAmount).HasPrecision(18, 2); b.Property(x => x.TaxAmount).HasPrecision(18, 2);
        b.Property(x => x.GrossAmount).HasPrecision(18, 2); b.Property(x => x.ReceiptClearingAmount).HasPrecision(18, 2); b.Property(x => x.CompanyNetAmount).HasPrecision(18,2); b.Property(x => x.CompanyTaxAmount).HasPrecision(18,2); b.Property(x => x.CompanyGrossAmount).HasPrecision(18,2); b.Property(x => x.ReceiptExchangeRate).HasPrecision(24,12);
        b.Property(x => x.ProductCodeSnapshot).HasMaxLength(100).IsRequired(); b.Property(x => x.ProductNameSnapshot).HasMaxLength(250).IsRequired(); b.Property(x => x.UnitNameSnapshot).HasMaxLength(100).IsRequired();
        b.HasIndex(x => new { x.VendorBillId, x.GoodsReceiptLineId }).IsUnique()
            .HasFilter("[GoodsReceiptLineId] IS NOT NULL");
        b.HasIndex(x => new { x.VendorBillId, x.PurchaseServiceAcceptanceLineId }).IsUnique()
            .HasFilter("[PurchaseServiceAcceptanceLineId] IS NOT NULL");
        b.HasIndex(x => new { x.VendorBillId, x.PurchaseOrderLineId }).IsUnique()
            .HasFilter("[GoodsReceiptLineId] IS NULL AND [PurchaseServiceAcceptanceLineId] IS NULL");
        b.HasOne<GoodsReceiptLine>().WithMany().HasForeignKey(x => x.GoodsReceiptLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PurchaseServiceAcceptanceLine>().WithMany().HasForeignKey(x => x.PurchaseServiceAcceptanceLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PurchaseOrderLine>().WithMany().HasForeignKey(x => x.PurchaseOrderLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Account>().WithMany().HasForeignKey(x => x.PurchaseExpenseAccountIdSnapshot).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<TaxRate>().WithMany().HasForeignKey(x => x.TaxRateId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Account>().WithMany().HasForeignKey(x => x.TaxInputAccountId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<VendorBillAttachment> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        b.Property(x => x.ContentType).HasMaxLength(150).IsRequired(); b.Property(x => x.Content).IsRequired();
        b.Property(x => x.UploadedByUserId).HasMaxLength(450).IsRequired();
        b.HasIndex(x => new { x.VendorBillId, x.Id }).IsUnique();
    }
}
