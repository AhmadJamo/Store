using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class StorageLocationConfiguration : IEntityTypeConfiguration<StorageLocation>
{
    public void Configure(EntityTypeBuilder<StorageLocation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Zone).HasMaxLength(50);
        builder.Property(x => x.Aisle).HasMaxLength(50);
        builder.Property(x => x.Rack).HasMaxLength(50);
        builder.Property(x => x.Level).HasMaxLength(50);
        builder.Property(x => x.Bin).HasMaxLength(50);
        builder.Property(x => x.MaximumQuantity).HasPrecision(18, 3);
        builder.HasIndex(x => new { x.WarehouseId, x.Code }).IsUnique();
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
    }
}
