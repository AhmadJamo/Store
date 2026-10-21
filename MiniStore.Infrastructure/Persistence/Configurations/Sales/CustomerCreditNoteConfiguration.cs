using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class CustomerCreditNoteConfiguration :
    IEntityTypeConfiguration<CustomerCreditNote>, IEntityTypeConfiguration<CustomerCreditNoteLine>
{
    public void Configure(EntityTypeBuilder<CustomerCreditNote> builder)
    {
        builder.ToTable("CustomerCreditNotes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CreditNoteNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.CreditNoteNumber).IsUnique();
        builder.Property(x => x.RequestKey).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.RequestKey).IsUnique();
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        builder.Property(x => x.ExchangeRate).HasPrecision(18, 8);
        builder.Property(x => x.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.PostedByUserId).HasMaxLength(450);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.Ignore(x => x.NetAmount);
        builder.Ignore(x => x.TaxAmount);
        builder.Ignore(x => x.TotalAmount);
        builder.Ignore(x => x.CompanyTotalAmount);
        builder.HasIndex(x => new { x.CustomerInvoiceId, x.Status });
        builder.HasOne<CustomerInvoice>().WithMany().HasForeignKey(x => x.CustomerInvoiceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<SalesOrder>().WithMany().HasForeignKey(x => x.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany().HasForeignKey(x => x.ReceivableAccountIdSnapshot).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany().HasForeignKey(x => x.SalesRevenueAccountIdSnapshot).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.CustomerCreditNoteId).OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<CustomerCreditNoteLine> builder)
    {
        builder.ToTable("CustomerCreditNoteLines", table =>
            table.HasCheckConstraint("CK_CustomerCreditNoteLines_Quantity", "[Quantity] > 0 AND [StockQuantity] > 0"));
        builder.HasKey(x => x.Id);
        foreach (var name in new[] { nameof(CustomerCreditNoteLine.Quantity), nameof(CustomerCreditNoteLine.StockQuantity) })
            builder.Property(name).HasPrecision(18, 6);
        foreach (var name in new[] { nameof(CustomerCreditNoteLine.NetAmount), nameof(CustomerCreditNoteLine.TaxAmount), nameof(CustomerCreditNoteLine.GrossAmount), nameof(CustomerCreditNoteLine.CompanyNetAmount), nameof(CustomerCreditNoteLine.CompanyTaxAmount), nameof(CustomerCreditNoteLine.CompanyGrossAmount) })
            builder.Property(name).HasPrecision(18, 2);
        builder.Property(x => x.ProductCodeSnapshot).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ProductNameSnapshot).HasMaxLength(200).IsRequired();
        builder.Property(x => x.UnitNameSnapshot).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.CustomerCreditNoteId, x.CustomerInvoiceLineId, x.CustomerReturnReceiptLineId }).IsUnique();
        builder.HasOne<CustomerInvoiceLine>().WithMany().HasForeignKey(x => x.CustomerInvoiceLineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<CustomerReturnReceiptLine>().WithMany().HasForeignKey(x => x.CustomerReturnReceiptLineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany().HasForeignKey(x => x.TaxOutputAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}
