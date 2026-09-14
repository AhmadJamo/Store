using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Type).HasConversion<int>().IsRequired();
        builder.Property(x => x.ControlMode).HasConversion<int>().IsRequired();
        builder.Property(x => x.PickingStrategy).HasConversion<int>().IsRequired();
        builder.Property(x => x.AllowPosSales).IsRequired();
        builder.Property(x => x.EnforceLocationCapacity).IsRequired();
        builder.Property(x => x.RequireSourceLocationForTransfers).IsRequired();
        builder.Property(x => x.RequireDestinationLocationForTransfers).IsRequired();

        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany().HasForeignKey(x => x.InventoryAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}
