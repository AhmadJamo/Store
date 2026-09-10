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
    }
}