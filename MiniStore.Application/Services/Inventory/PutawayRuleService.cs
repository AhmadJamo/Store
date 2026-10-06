using MiniStore.Domain.Entities;using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public sealed class PutawayRuleService(IPutawayRuleRepository rules,IStorageLocationRepository locations)
{
 public async Task<StorageLocation?> SuggestAsync(Product product,int warehouseId)
 {
  var candidates=(await rules.GetWarehouseRulesAsync(warehouseId)).Where(x=>x.IsActive&&(x.ProductId==product.Id||(!x.ProductId.HasValue&&x.ProductCategoryId==product.ProductCategoryId)||(!x.ProductId.HasValue&&!x.ProductCategoryId.HasValue))).OrderByDescending(x=>x.ProductId.HasValue).ThenByDescending(x=>x.ProductCategoryId.HasValue).ThenBy(x=>x.Priority);
  foreach(var rule in candidates){var location=await locations.GetByIdAsync(rule.StorageLocationId);if(location is {Status:StorageLocationStatus.Active,IsReceivable:true}&&location.WarehouseId==warehouseId)return location;}
  return (await locations.GetWarehouseLocationsAsync(warehouseId)).FirstOrDefault(x=>x.Status==StorageLocationStatus.Active&&x.IsReceivable);
 }
}
