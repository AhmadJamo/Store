using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class InvoiceSettingsConfiguration : IEntityTypeConfiguration<InvoiceSettings>
{
    public void Configure(EntityTypeBuilder<InvoiceSettings> builder)
    {
        builder.ToTable("InvoiceSettings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.WholesalePrefix).IsRequired().HasMaxLength(30);
        builder.Property(x => x.PosPrefix).IsRequired().HasMaxLength(30);
        builder.Property(x => x.NumberLength).IsRequired();
        builder.Property(x => x.NextWholesaleNumber).IsRequired();
        builder.Property(x => x.NextPosNumber).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
