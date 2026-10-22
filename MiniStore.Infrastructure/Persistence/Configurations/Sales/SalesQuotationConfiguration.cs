using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class SalesQuotationConfiguration : IEntityTypeConfiguration<SalesQuotation>,
    IEntityTypeConfiguration<SalesQuotationLine>, IEntityTypeConfiguration<SalesQuotationHistory>
{
    public void Configure(EntityTypeBuilder<SalesQuotation> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.QuotationNumber).HasMaxLength(50).IsRequired();
        b.Property(x => x.RequestKey).HasMaxLength(36).IsRequired();
        b.Property(x => x.CustomerCode).HasMaxLength(50).IsRequired(); b.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
        b.Property(x => x.CustomerLegalName).HasMaxLength(200); b.Property(x => x.CustomerTaxNumber).HasMaxLength(100);
        b.Property(x => x.AddressLabel).HasMaxLength(100); b.Property(x => x.AddressLine1).HasMaxLength(250); b.Property(x => x.AddressLine2).HasMaxLength(250);
        b.Property(x => x.City).HasMaxLength(100); b.Property(x => x.StateOrProvince).HasMaxLength(100); b.Property(x => x.PostalCode).HasMaxLength(30); b.Property(x => x.CountryCode).HasMaxLength(2);
        b.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired(); b.Property(x => x.ExchangeRate).HasPrecision(24, 12); b.Property(x => x.ExchangeRateSource).HasMaxLength(100).IsRequired();
        b.Property(x => x.NetAmount).HasPrecision(24, 8); b.Property(x => x.TaxAmount).HasPrecision(24, 8); b.Property(x => x.TotalAmount).HasPrecision(24, 8);
        b.Property(x => x.Notes).HasMaxLength(1000); b.Property(x => x.TemplateCodeSnapshot).HasMaxLength(50); b.Property(x => x.TemplateNameSnapshot).HasMaxLength(150);
        b.Property(x => x.IntroductionSnapshot).HasMaxLength(4000); b.Property(x => x.TermsAndConditionsSnapshot).HasMaxLength(8000); b.Property(x => x.FooterNotesSnapshot).HasMaxLength(4000);
        b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => x.QuotationNumber).IsUnique(); b.HasIndex(x => x.RequestKey).IsUnique(); b.HasIndex(x => new { x.Status, x.CreatedAtUtc });
        b.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<CustomerAddress>().WithMany().HasForeignKey(x => x.CustomerAddressId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<SalesQuotationTemplate>().WithMany().HasForeignKey(x => x.SalesQuotationTemplateId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.SalesQuotationId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.History).WithOne().HasForeignKey(x => x.SalesQuotationId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<SalesQuotationLine> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.ProductCode).HasMaxLength(50).IsRequired(); b.Property(x => x.ProductName).HasMaxLength(200).IsRequired();
        b.Property(x => x.UnitCode).HasMaxLength(30).IsRequired(); b.Property(x => x.UnitName).HasMaxLength(100).IsRequired();
        b.Property(x => x.UnitFactorToStock).HasPrecision(24, 12); b.Property(x => x.Quantity).HasPrecision(24, 6); b.Property(x => x.StockQuantity).HasPrecision(24, 6);
        b.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired(); b.Property(x => x.PriceSourceCode).HasMaxLength(80).IsRequired();
        b.Property(x => x.UnitPrice).HasPrecision(24, 8); b.Property(x => x.CompanyUnitPrice).HasPrecision(24, 8); b.Property(x => x.NetAmount).HasPrecision(24, 8);
        b.Property(x => x.TaxName).HasMaxLength(150); b.Property(x => x.TaxRatePercent).HasPrecision(9, 4); b.Property(x => x.TaxSourceCode).HasMaxLength(80).IsRequired(); b.Property(x => x.TaxExemptionReason).HasMaxLength(500);
        b.Property(x => x.TaxAmount).HasPrecision(24, 8); b.Property(x => x.TotalAmount).HasPrecision(24, 8); b.Property(x => x.Notes).HasMaxLength(500);
        b.Property(x => x.TemplateSectionSnapshot).HasMaxLength(150); b.Property(x => x.TemplateOptionGroupSnapshot).HasMaxLength(150);
        b.HasIndex(x => new { x.SalesQuotationId, x.ProductId, x.MeasurementUnitId }).IsUnique();
        b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<MeasurementUnit>().WithMany().HasForeignKey(x => x.MeasurementUnitId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<SalesPriceList>().WithMany().HasForeignKey(x => x.SalesPriceListId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<SalesPriceRule>().WithMany().HasForeignKey(x => x.SalesPriceRuleId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<TaxRate>().WithMany().HasForeignKey(x => x.TaxRateId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<SalesQuotationTemplateItem>().WithMany().HasForeignKey(x => x.SalesQuotationTemplateItemId).OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<SalesQuotationHistory> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.UserId).HasMaxLength(450).IsRequired(); b.Property(x => x.Reason).HasMaxLength(500);
        b.HasIndex(x => new { x.SalesQuotationId, x.CreatedAtUtc });
    }
}
