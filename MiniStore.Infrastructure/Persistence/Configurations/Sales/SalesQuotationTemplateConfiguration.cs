using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class SalesQuotationTemplateConfiguration : IEntityTypeConfiguration<SalesQuotationTemplate>,
    IEntityTypeConfiguration<SalesQuotationTemplateItem>
{
    public void Configure(EntityTypeBuilder<SalesQuotationTemplate> b)
    {
        b.ToTable("SalesQuotationTemplates", t => t.HasCheckConstraint("CK_SalesQuotationTemplates_VersionValidity", "[Version] BETWEEN 1 AND 9999 AND [ValidityDays] BETWEEN 0 AND 3650"));
        b.HasKey(x => x.Id); b.Property(x => x.Code).HasMaxLength(50).IsRequired(); b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.Property(x => x.Description).HasMaxLength(1000); b.Property(x => x.Category).HasMaxLength(100); b.Property(x => x.Tags).HasMaxLength(500);
        b.Property(x => x.Introduction).HasMaxLength(4000); b.Property(x => x.TermsAndConditions).HasMaxLength(8000); b.Property(x => x.FooterNotes).HasMaxLength(4000);
        b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => new { x.Code, x.Version }).IsUnique(); b.HasIndex(x => new { x.Status, x.Category });
        b.HasIndex(x => x.IsDefault).HasFilter("[IsDefault] = 1 AND [Status] = 2").IsUnique();
        b.HasOne<SalesQuotationTemplate>().WithMany().HasForeignKey(x => x.SourceTemplateId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<SalesPriceList>().WithMany().HasForeignKey(x => x.SalesPriceListId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<DocumentDesign>().WithMany().HasForeignKey(x => x.DocumentDesignId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.SalesQuotationTemplateId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<SalesQuotationTemplateItem> b)
    {
        b.ToTable("SalesQuotationTemplateItems", t => t.HasCheckConstraint("CK_SalesQuotationTemplateItems_Quantity", "[DefaultQuantity] > 0 AND [MinimumQuantity] > 0 AND [MaximumQuantity] >= [MinimumQuantity] AND [DefaultQuantity] BETWEEN [MinimumQuantity] AND [MaximumQuantity]"));
        b.HasKey(x => x.Id); b.Property(x => x.SectionTitle).HasMaxLength(150).IsRequired();
        b.Property(x => x.ProductNameSnapshot).HasMaxLength(200).IsRequired(); b.Property(x => x.UnitNameSnapshot).HasMaxLength(100).IsRequired();
        b.Property(x => x.DefaultQuantity).HasPrecision(24, 6); b.Property(x => x.MinimumQuantity).HasPrecision(24, 6); b.Property(x => x.MaximumQuantity).HasPrecision(24, 6);
        b.Property(x => x.OptionGroupCode).HasMaxLength(50); b.Property(x => x.OptionGroupName).HasMaxLength(150); b.Property(x => x.Notes).HasMaxLength(500);
        b.HasIndex(x => new { x.SalesQuotationTemplateId, x.Sequence }).IsUnique();
        b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<MeasurementUnit>().WithMany().HasForeignKey(x => x.MeasurementUnitId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<TaxRate>().WithMany().HasForeignKey(x => x.TaxRateId).OnDelete(DeleteBehavior.Restrict);
    }
}
