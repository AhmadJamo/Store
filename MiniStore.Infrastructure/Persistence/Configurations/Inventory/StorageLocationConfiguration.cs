using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class StorageLocationConfiguration : IEntityTypeConfiguration<StorageLocation>
{
    public void Configure(EntityTypeBuilder<StorageLocation> builder)
    {
        builder.HasKey(location => location.Id);

        builder.Property(location => location.Code)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(location => location.Zone).HasMaxLength(50);
        builder.Property(location => location.Aisle).HasMaxLength(50);
        builder.Property(location => location.Rack).HasMaxLength(50);
        builder.Property(location => location.Level).HasMaxLength(50);
        builder.Property(location => location.Bin).HasMaxLength(50);

        builder.Property(location => location.MaximumQuantity)
            .HasPrecision(18, 3);

        builder.HasIndex(location => new
            {
                location.WarehouseId,
                location.Code
            })
            .IsUnique();

        builder.HasOne<Warehouse>()
            .WithMany()
            .HasForeignKey(location => location.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
