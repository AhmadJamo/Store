using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class InventorySettingsConfiguration : IEntityTypeConfiguration<InventorySettings>
{
    public void Configure(EntityTypeBuilder<InventorySettings> builder)
    {
        builder.ToTable("InventorySettings", table =>
        {
            table.HasCheckConstraint(
                "CK_InventorySettings_SingletonKey",
                "[SingletonKey] = 1");
        });

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.SingletonKey).IsUnique();
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        builder.Property(x => x.DefaultWarehouseType).HasConversion<int>().IsRequired();
        builder.Property(x => x.DefaultControlMode).HasConversion<int>().IsRequired();
        builder.Property(x => x.DefaultPickingStrategy).HasConversion<int>().IsRequired();
    }
}
