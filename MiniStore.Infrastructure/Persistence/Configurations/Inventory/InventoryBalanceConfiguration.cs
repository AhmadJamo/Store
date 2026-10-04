using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class InventoryBalanceConfiguration : IEntityTypeConfiguration<InventoryBalance>
{
    public void Configure(EntityTypeBuilder<InventoryBalance> builder)
    {
        builder.ToTable("InventoryBalances", table =>
        {
            table.HasCheckConstraint("CK_InventoryBalances_Reserved", "[Reserved] >= 0 AND (([OnHand] >= 0 AND [Reserved] <= [OnHand]) OR ([OnHand] < 0 AND [Reserved] = 0))");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OnHand).HasPrecision(18, 6);
        builder.Property(x => x.Reserved).HasPrecision(18, 6);
        builder.Ignore(x => x.Available);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.ProductId, x.WarehouseId, x.StorageLocationId }).IsUnique();
        builder.HasIndex(x => new { x.ProductId, x.WarehouseId }).IsUnique()
            .HasFilter("[StorageLocationId] IS NULL");
        builder.HasIndex(x => new { x.WarehouseId, x.StorageLocationId });
        builder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<StorageLocation>().WithMany().HasForeignKey(x => x.StorageLocationId).OnDelete(DeleteBehavior.Restrict);
    }
}
