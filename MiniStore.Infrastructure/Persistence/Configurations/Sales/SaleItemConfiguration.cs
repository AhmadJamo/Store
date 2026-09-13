using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class SaleItemConfiguration
: IEntityTypeConfiguration<SaleItem>
{
    public void Configure(
    EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");


    builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasColumnType("decimal(18,3)")
            .IsRequired();

        builder.Property(x => x.SalePrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.GrossTotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.DiscountType)
            .IsRequired();

        builder.Property(x => x.DiscountValue)
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        builder.Property(x => x.DiscountAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Total)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
