using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class SaleConfiguration
    : IEntityTypeConfiguration<Sale>
{
    public void Configure(
        EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.InvoiceNumber)
            .IsUnique();

        builder.Property(x => x.Channel)
            .IsRequired();

        builder.Property(x => x.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.TotalAmount)
            .HasColumnType("decimal(18,2)");

        builder.HasOne<Warehouse>()
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
