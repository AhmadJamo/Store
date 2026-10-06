using Microsoft.EntityFrameworkCore;using Microsoft.EntityFrameworkCore.Metadata.Builders;using MiniStore.Domain.Entities;
namespace MiniStore.Infrastructure.Persistence.Configurations;
public sealed class PutawayRuleConfiguration:IEntityTypeConfiguration<PutawayRule>
{
 public void Configure(EntityTypeBuilder<PutawayRule>b){b.HasKey(x=>x.Id);b.HasIndex(x=>new{x.WarehouseId,x.Priority});b.HasOne<Warehouse>().WithMany().HasForeignKey(x=>x.WarehouseId).OnDelete(DeleteBehavior.Restrict);b.HasOne<StorageLocation>().WithMany().HasForeignKey(x=>x.StorageLocationId).OnDelete(DeleteBehavior.Restrict);b.HasOne<Product>().WithMany().HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.Restrict);b.HasOne<ProductCategory>().WithMany().HasForeignKey(x=>x.ProductCategoryId).OnDelete(DeleteBehavior.Restrict);}
}
