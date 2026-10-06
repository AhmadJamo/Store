using Microsoft.EntityFrameworkCore;using Microsoft.EntityFrameworkCore.Metadata.Builders;using MiniStore.Domain.Entities;
namespace MiniStore.Infrastructure.Persistence.Configurations;
public sealed class ReplenishmentRuleConfiguration:IEntityTypeConfiguration<ReplenishmentRule>
{
 public void Configure(EntityTypeBuilder<ReplenishmentRule>b){b.HasKey(x=>x.Id);b.Property<int>("TenantId");b.Property(x=>x.MinimumQuantity).HasPrecision(18,3);b.Property(x=>x.MaximumQuantity).HasPrecision(18,3);b.Property(x=>x.SafetyStock).HasPrecision(18,3);b.HasIndex("TenantId",nameof(ReplenishmentRule.ProductId),nameof(ReplenishmentRule.WarehouseId)).IsUnique();b.HasOne<Product>().WithMany().HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.Restrict);b.HasOne<Warehouse>().WithMany().HasForeignKey(x=>x.WarehouseId).OnDelete(DeleteBehavior.Restrict);b.HasOne<Warehouse>().WithMany().HasForeignKey(x=>x.PreferredSourceWarehouseId).OnDelete(DeleteBehavior.Restrict);}
}
