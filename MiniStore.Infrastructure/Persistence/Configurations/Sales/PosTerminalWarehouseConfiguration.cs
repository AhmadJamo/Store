using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class PosTerminalWarehouseConfiguration
    : IEntityTypeConfiguration<PosTerminalWarehouse>
{
    public void Configure(EntityTypeBuilder<PosTerminalWarehouse> builder)
    {
        builder.HasKey(x => new { x.PosTerminalId, x.WarehouseId });
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.PosTerminalId, x.Priority });
    }
}
