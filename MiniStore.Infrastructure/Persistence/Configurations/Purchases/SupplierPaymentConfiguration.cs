using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class SupplierPaymentConfiguration : IEntityTypeConfiguration<SupplierPayment>, IEntityTypeConfiguration<SupplierPaymentLine>
{
    public void Configure(EntityTypeBuilder<SupplierPayment> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.PaymentNumber).HasMaxLength(50).IsRequired(); b.HasIndex(x => x.PaymentNumber).IsUnique();
        b.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired(); b.Property(x => x.ExternalReference).HasMaxLength(100);
        b.Property(x => x.Notes).HasMaxLength(500); b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.PostedByUserId).HasMaxLength(450);
        b.Property(x => x.RowVersion).IsRowVersion(); b.Ignore(x => x.TotalAmount);
        b.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PaymentMethod>().WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.SupplierPaymentId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<SupplierPaymentLine> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.VendorBillNumberSnapshot).HasMaxLength(50).IsRequired();
        b.Property(x => x.SupplierInvoiceNumberSnapshot).HasMaxLength(100).IsRequired();
        b.HasIndex(x => new { x.SupplierPaymentId, x.VendorBillId }).IsUnique();
        b.HasOne<VendorBill>().WithMany().HasForeignKey(x => x.VendorBillId).OnDelete(DeleteBehavior.Restrict);
    }
}
