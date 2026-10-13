using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class CurrencyExchangeRateConfiguration : IEntityTypeConfiguration<CurrencyExchangeRate>
{
    public void Configure(EntityTypeBuilder<CurrencyExchangeRate> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsUnicode(false).IsRequired();
        builder.Property(x => x.CompanyCurrencyPerUnit).HasPrecision(24, 12);
        builder.Property(x => x.Source).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.CurrencyCode, x.EffectiveDate }).IsUnique();
    }
}
