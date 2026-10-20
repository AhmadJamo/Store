using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class CustomerInvoiceConfiguration : IEntityTypeConfiguration<CustomerInvoice>,
    IEntityTypeConfiguration<CustomerInvoiceLine>, IEntityTypeConfiguration<CustomerInvoiceSourceAllocation>,
    IEntityTypeConfiguration<CustomerInvoiceHistory>
{
    public void Configure(EntityTypeBuilder<CustomerInvoice> b)
    {
        b.ToTable("CustomerInvoices"); b.HasKey(x => x.Id);
        b.Property(x => x.InvoiceNumber).HasMaxLength(50).IsRequired(); b.Property(x => x.RequestKey).HasMaxLength(100).IsRequired();
        b.Property(x => x.CustomerCode).HasMaxLength(50).IsRequired(); b.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
        b.Property(x => x.CustomerLegalName).HasMaxLength(200); b.Property(x => x.CustomerTaxNumber).HasMaxLength(100);
        b.Property(x => x.AddressLabel).HasMaxLength(100); b.Property(x => x.AddressLine1).HasMaxLength(250);
        b.Property(x => x.AddressLine2).HasMaxLength(250); b.Property(x => x.City).HasMaxLength(100);
        b.Property(x => x.StateOrProvince).HasMaxLength(100); b.Property(x => x.PostalCode).HasMaxLength(30);
        b.Property(x => x.CountryCode).HasMaxLength(2); b.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        b.Property(x => x.ExchangeRate).HasPrecision(18, 8); b.Property(x => x.ExchangeRateSource).HasMaxLength(100).IsRequired();
        b.Property(x => x.Notes).HasMaxLength(1000); b.Property(x => x.CancellationReason).HasMaxLength(500);
        b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.ApprovedByUserId).HasMaxLength(450);
        b.Property(x => x.CancelledByUserId).HasMaxLength(450); b.Property(x => x.NetAmount).HasPrecision(18, 2);
        b.Property(x => x.TaxAmount).HasPrecision(18, 2); b.Property(x => x.TotalAmount).HasPrecision(18, 2);
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => x.InvoiceNumber).IsUnique(); b.HasIndex(x => x.RequestKey).IsUnique();
        b.HasIndex(x => new { x.SalesOrderId, x.Status });
        b.HasOne<SalesOrder>().WithMany().HasForeignKey(x => x.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<CustomerAddress>().WithMany().HasForeignKey(x => x.CustomerAddressId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.CustomerInvoiceId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.History).WithOne().HasForeignKey(x => x.CustomerInvoiceId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<CustomerInvoiceLine> b)
    {
        b.ToTable("CustomerInvoiceLines", t => t.HasCheckConstraint("CK_CustomerInvoiceLines_Quantity", "[Quantity] > 0 AND [StockQuantity] > 0"));
        b.HasKey(x => x.Id); b.Property(x => x.ProductCode).HasMaxLength(100).IsRequired();
        b.Property(x => x.ProductName).HasMaxLength(200).IsRequired(); b.Property(x => x.UnitCode).HasMaxLength(30).IsRequired();
        b.Property(x => x.UnitName).HasMaxLength(100).IsRequired(); b.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        b.Property(x => x.PriceSourceCode).HasMaxLength(100).IsRequired(); b.Property(x => x.TaxName).HasMaxLength(100);
        b.Property(x => x.TaxSourceCode).HasMaxLength(100).IsRequired(); b.Property(x => x.TaxExemptionReason).HasMaxLength(500);
        b.Property(x => x.Notes).HasMaxLength(1000); b.Property(x => x.UnitFactorToStock).HasPrecision(18, 8);
        b.Property(x => x.Quantity).HasPrecision(18, 6); b.Property(x => x.StockQuantity).HasPrecision(18, 6);
        b.Property(x => x.UnitPrice).HasPrecision(18, 6); b.Property(x => x.CompanyUnitPrice).HasPrecision(18, 6);
        b.Property(x => x.NetAmount).HasPrecision(18, 2); b.Property(x => x.TaxRatePercent).HasPrecision(9, 4);
        b.Property(x => x.TaxAmount).HasPrecision(18, 2); b.Property(x => x.TotalAmount).HasPrecision(18, 2);
        b.HasIndex(x => new { x.CustomerInvoiceId, x.SalesOrderLineId }).IsUnique();
        b.HasOne<SalesOrderLine>().WithMany().HasForeignKey(x => x.SalesOrderLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<MeasurementUnit>().WithMany().HasForeignKey(x => x.MeasurementUnitId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<SalesPriceList>().WithMany().HasForeignKey(x => x.SalesPriceListId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<SalesPriceRule>().WithMany().HasForeignKey(x => x.SalesPriceRuleId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<TaxRate>().WithMany().HasForeignKey(x => x.TaxRateId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.SourceAllocations).WithOne().HasForeignKey(x => x.CustomerInvoiceLineId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<CustomerInvoiceSourceAllocation> b)
    {
        b.ToTable("CustomerInvoiceSourceAllocations", t => t.HasCheckConstraint("CK_CustomerInvoiceSourceAllocations_Quantity", "[StockQuantity] > 0"));
        b.HasKey(x => x.Id); b.Property(x => x.StockQuantity).HasPrecision(18, 6);
        b.HasIndex(x => new { x.CustomerInvoiceLineId, x.SalesDeliveryLineId }).IsUnique();
        b.HasOne<SalesDelivery>().WithMany().HasForeignKey(x => x.SalesDeliveryId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<SalesDeliveryLine>().WithMany().HasForeignKey(x => x.SalesDeliveryLineId).OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<CustomerInvoiceHistory> b)
    {
        b.ToTable("CustomerInvoiceHistories"); b.HasKey(x => x.Id);
        b.Property(x => x.UserId).HasMaxLength(450).IsRequired(); b.Property(x => x.Reason).HasMaxLength(500);
        b.HasIndex(x => new { x.CustomerInvoiceId, x.CreatedAtUtc });
    }
}
