using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class DocumentNumberSettingsConfiguration
    : IEntityTypeConfiguration<DocumentNumberSettings>
{
    public void Configure(
        EntityTypeBuilder<DocumentNumberSettings> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StockTransferPrefix)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.NextStockTransferNumber)
            .IsRequired();

        builder.Property(x => x.NumberLength)
            .IsRequired();
    }
}