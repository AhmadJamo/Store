using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class CustomerPaymentConfiguration : IEntityTypeConfiguration<CustomerPayment>, IEntityTypeConfiguration<CustomerPaymentLine>
{
    public void Configure(EntityTypeBuilder<CustomerPayment> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.PaymentNumber).HasMaxLength(50).IsRequired(); b.HasIndex(x => x.PaymentNumber).IsUnique();
        b.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired(); b.Property(x => x.CompanyCurrencyCode).HasMaxLength(3).IsRequired();
        b.Property(x => x.ExchangeRate).HasPrecision(24, 12); b.Property(x => x.ExchangeRateSource).HasMaxLength(100).IsRequired();
        b.Property(x => x.ExternalReference).HasMaxLength(100); b.Property(x => x.Notes).HasMaxLength(1000);
        b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.PostedByUserId).HasMaxLength(450); b.Property(x => x.ReversedByUserId).HasMaxLength(450); b.Property(x => x.ReversalReason).HasMaxLength(500);
        b.Property(x => x.RowVersion).IsRowVersion(); b.Ignore(x => x.TotalAmount); b.Ignore(x => x.CompanyCurrencyAmount); b.Ignore(x => x.ReceivableCarryingAmount);
        b.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PaymentMethod>().WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Account>().WithMany().HasForeignKey(x => x.PaymentMethodAccountIdSnapshot).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.CustomerPaymentId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<CustomerPaymentLine> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.CustomerInvoiceNumberSnapshot).HasMaxLength(50).IsRequired();
        b.Property(x => x.Amount).HasPrecision(18, 2); b.Property(x => x.CompanyCurrencyAmount).HasPrecision(18, 2); b.Property(x => x.ReceivableCarryingAmount).HasPrecision(18, 2);
        b.HasIndex(x => new { x.CustomerPaymentId, x.CustomerInvoiceId }).IsUnique();
        b.HasOne<CustomerInvoice>().WithMany().HasForeignKey(x => x.CustomerInvoiceId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Account>().WithMany().HasForeignKey(x => x.ReceivableAccountIdSnapshot).OnDelete(DeleteBehavior.Restrict);
    }
}
