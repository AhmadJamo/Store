using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class StockTransferItemConfiguration
    : IEntityTypeConfiguration<StockTransferItem>
{
    public void Configure(
        EntityTypeBuilder<StockTransferItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<StockTransfer>()
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.StockTransferId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.StockTransferId,
            x.ProductId
        })
        .IsUnique();
    }
}