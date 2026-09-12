using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class DiscountSettingsConfiguration
    : IEntityTypeConfiguration<DiscountSettings>
{
    public void Configure(EntityTypeBuilder<DiscountSettings> builder)
    {
        builder.ToTable("DiscountSettings", table =>
        {
            table.HasCheckConstraint(
                "CK_DiscountSettings_SingletonKey",
                "[SingletonKey] = 1");
        });

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.SingletonKey)
            .IsUnique();

        builder.Property(x => x.SingletonKey)
            .IsRequired();

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.Property(x => x.Enabled)
            .IsRequired();

        builder.Property(x => x.AllowLineDiscount)
            .IsRequired();

        builder.Property(x => x.AllowInvoiceDiscount)
            .IsRequired();

        builder.Property(x => x.AllowPercentageDiscount)
            .IsRequired();

        builder.Property(x => x.AllowFixedAmountDiscount)
            .IsRequired();

        builder.Property(x => x.DefaultDiscountType)
            .IsRequired();

        builder.Property(x => x.MaxLineDiscountPercent)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.MaxLineDiscountAmount)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.MaxInvoiceDiscountPercent)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.MaxInvoiceDiscountAmount)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.AllowDiscountAboveLimit)
            .IsRequired();

        builder.Property(x => x.DiscountOverridePermission)
            .IsRequired()
            .HasMaxLength(200);
    }
}