using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class GeneralSettingsConfiguration
    : IEntityTypeConfiguration<GeneralSettings>
{
    public void Configure(
        EntityTypeBuilder<GeneralSettings> builder)
    {
        builder.ToTable("GeneralSettings", table =>
        {
            table.HasCheckConstraint(
                "CK_GeneralSettings_SingletonKey",
                "[SingletonKey] = 1");
            table.HasCheckConstraint(
                "CK_GeneralSettings_DocumentNameStyle",
                "[DocumentNameStyle] BETWEEN 1 AND 3");
        });

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.SingletonKey).IsUnique();

       





        builder.Property(x => x.RowVersion)
    .IsRowVersion()
    .IsConcurrencyToken();



        builder.Property(x => x.CompanyName)
            .IsRequired()

            .HasMaxLength(200);

        builder.Property(x => x.CompanyNameArabic)
            .HasMaxLength(200);

        builder.Property(x => x.LegalName).HasMaxLength(200);
        builder.Property(x => x.LegalNameArabic).HasMaxLength(200);
        builder.Property(x => x.CommercialRegistrationNumber).HasMaxLength(100);
        builder.Property(x => x.TaxRegistrationNumber).HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(254);
        builder.Property(x => x.Website).HasMaxLength(500);
        builder.Property(x => x.SecondaryPhone).HasMaxLength(50);
        builder.Property(x => x.AddressArabic).HasMaxLength(500);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.CityArabic).HasMaxLength(100);
        builder.Property(x => x.StateOrProvince).HasMaxLength(100);
        builder.Property(x => x.PostalCode).HasMaxLength(30);
        builder.Property(x => x.CountryCode).HasMaxLength(2).IsFixedLength();
        builder.Property(x => x.DocumentNameStyle).HasConversion<int>().IsRequired();

        builder.Property(x => x.Phone)
            .HasMaxLength(50);

        builder.Property(x => x.Address)
            .HasMaxLength(500);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.DecimalPlaces)
            .IsRequired();

        builder.Property(x => x.QuantityDecimalPlaces)
            .IsRequired();

        builder.Property(x => x.DateFormat)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.TimeZone)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DefaultLanguage)
            .HasConversion<int>()
            .IsRequired();

        builder.HasMany(x => x.IdentityFields)
            .WithOne()
            .HasForeignKey(x => x.GeneralSettingsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Logo)
            .WithOne()
            .HasForeignKey<OrganizationLogo>(x => x.GeneralSettingsId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
