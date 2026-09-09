using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class PurchaseItemConfiguration
    : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(
        EntityTypeBuilder<PurchaseItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 3);

        builder.Property(x => x.PurchasePrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.Total)
            .HasPrecision(18, 2);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}