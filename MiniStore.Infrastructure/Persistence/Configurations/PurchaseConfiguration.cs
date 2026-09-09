using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class PurchaseConfiguration
    : IEntityTypeConfiguration<Purchase>
{
    public void Configure(
        EntityTypeBuilder<Purchase> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.InvoiceNumber)
            .IsUnique();

        builder.HasOne<Supplier>()
            .WithMany()
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Warehouse>()
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}