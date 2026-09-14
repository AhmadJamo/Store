using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class BranchWarehouseAccessConfiguration
    : IEntityTypeConfiguration<BranchWarehouseAccess>
{
    public void Configure(EntityTypeBuilder<BranchWarehouseAccess> builder)
    {
        builder.HasKey(x => new { x.BranchId, x.WarehouseId });
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.Priority });
    }
}
