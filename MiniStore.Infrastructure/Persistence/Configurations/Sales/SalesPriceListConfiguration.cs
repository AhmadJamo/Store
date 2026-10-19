using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;
namespace MiniStore.Infrastructure.Persistence.Configurations;
public sealed class SalesPriceListConfiguration:IEntityTypeConfiguration<SalesPriceList>
{
 public void Configure(EntityTypeBuilder<SalesPriceList> b)
 {
  b.HasKey(x=>x.Id);b.Property(x=>x.Code).HasMaxLength(30).IsRequired();b.Property(x=>x.Name).HasMaxLength(150).IsRequired();b.Property(x=>x.CurrencyCode).HasMaxLength(3).IsRequired();b.Property(x=>x.RoundingIncrement).HasPrecision(18,6);b.Property(x=>x.RowVersion).IsRowVersion();
  b.HasIndex(x=>x.Code).IsUnique();b.HasIndex(x=>new{x.IsActive,x.Name});
  b.HasMany(x=>x.Rules).WithOne().HasForeignKey(x=>x.SalesPriceListId).OnDelete(DeleteBehavior.Restrict);
  b.Navigation(x=>x.Rules).UsePropertyAccessMode(PropertyAccessMode.Field);
 }
}
public sealed class SalesPriceRuleConfiguration:IEntityTypeConfiguration<SalesPriceRule>
{
 public void Configure(EntityTypeBuilder<SalesPriceRule> b)
 {
  b.HasKey(x=>x.Id);b.Property(x=>x.MinimumQuantity).HasPrecision(24,6);b.Property(x=>x.Value).HasPrecision(24,8);b.Property(x=>x.RowVersion).IsRowVersion();
  b.HasIndex(x=>new{x.SalesPriceListId,x.IsActive,x.Priority});
  b.HasIndex(x=>new{x.SalesPriceListId,x.Scope,x.MeasurementUnitId,x.MinimumQuantity,x.Priority,x.EffectiveFrom}).IsUnique().HasFilter("[Scope] = 1");
  b.HasIndex(x=>new{x.SalesPriceListId,x.Scope,x.ProductCategoryId,x.MeasurementUnitId,x.MinimumQuantity,x.Priority,x.EffectiveFrom}).IsUnique().HasFilter("[Scope] = 2");
  b.HasIndex(x=>new{x.SalesPriceListId,x.Scope,x.ProductId,x.MeasurementUnitId,x.MinimumQuantity,x.Priority,x.EffectiveFrom}).IsUnique().HasFilter("[Scope] = 3");
  b.HasOne<Product>().WithMany().HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.Restrict);
  b.HasOne<ProductCategory>().WithMany().HasForeignKey(x=>x.ProductCategoryId).OnDelete(DeleteBehavior.Restrict);
  b.HasOne<MeasurementUnit>().WithMany().HasForeignKey(x=>x.MeasurementUnitId).OnDelete(DeleteBehavior.Restrict);
 }
}
