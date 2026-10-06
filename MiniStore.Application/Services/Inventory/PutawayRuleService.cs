using MiniStore.Application.DTOs.Warehouses;using MiniStore.Domain.Entities;using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public sealed class PutawayRuleService(IPutawayRuleRepository rules,IStorageLocationRepository locations,IWarehouseRepository warehouses,IProductRepository products,IProductCategoryRepository categories,IProductLocationStockRepository locationStocks)
{
 public async Task<StorageLocation?> SuggestAsync(Product product,int warehouseId,decimal quantity)
 {
  var warehouse=await warehouses.GetByIdAsync(warehouseId)??throw new InvalidOperationException("Warehouse not found.");
  var occupied=(await locationStocks.GetAllAsync()).GroupBy(x=>x.StorageLocationId).ToDictionary(x=>x.Key,x=>x.Sum(y=>y.Quantity));
  var candidates=(await rules.GetWarehouseRulesAsync(warehouseId)).Where(x=>x.IsActive&&(x.ProductId==product.Id||(!x.ProductId.HasValue&&x.ProductCategoryId==product.ProductCategoryId)||(!x.ProductId.HasValue&&!x.ProductCategoryId.HasValue))).OrderByDescending(x=>x.ProductId.HasValue).ThenByDescending(x=>x.ProductCategoryId.HasValue).ThenBy(x=>x.Priority);
  foreach(var rule in candidates){var location=await locations.GetByIdAsync(rule.StorageLocationId);if(IsAvailable(location,warehouse,quantity,occupied))return location;}
  return (await locations.GetWarehouseLocationsAsync(warehouseId)).FirstOrDefault(x=>IsAvailable(x,warehouse,quantity,occupied));
 }

 public async Task<List<PutawayRuleDto>> GetAllAsync()
 {
  var ruleRows=await rules.GetAllAsync();var warehouseRows=(await warehouses.GetAllAsync()).ToDictionary(x=>x.Id);var locationRows=(await locations.SearchAsync(null,null)).ToDictionary(x=>x.Id);var productRows=(await products.GetAllAsync(null)).ToDictionary(x=>x.Id);var categoryRows=(await categories.GetAllAsync()).ToDictionary(x=>x.Id);
  return ruleRows.Select(x=>new PutawayRuleDto{Id=x.Id,WarehouseId=x.WarehouseId,WarehouseName=warehouseRows.GetValueOrDefault(x.WarehouseId)?.Name??"—",StorageLocationId=x.StorageLocationId,StorageLocationCode=locationRows.GetValueOrDefault(x.StorageLocationId)?.Code??"—",Target=x.ProductId.HasValue?productRows.GetValueOrDefault(x.ProductId.Value)?.Name??"—":x.ProductCategoryId.HasValue?categoryRows.GetValueOrDefault(x.ProductCategoryId.Value)?.Name??"—":"Default",Priority=x.Priority,IsActive=x.IsActive}).ToList();
 }

 public async Task CreateAsync(CreatePutawayRuleDto input)
 {
  var warehouse=await warehouses.GetByIdAsync(input.WarehouseId)??throw new InvalidOperationException("Warehouse not found.");var location=await locations.GetByIdAsync(input.StorageLocationId)??throw new InvalidOperationException("Storage location not found.");
  if(warehouse.ControlMode==InventoryControlMode.Simple)throw new InvalidOperationException("A simple warehouse does not use exact storage locations.");
  if(location.WarehouseId!=warehouse.Id||location.Status!=StorageLocationStatus.Active||!location.IsReceivable)throw new InvalidOperationException("Select an active receivable location in the same warehouse.");
  if(input.ProductId.HasValue&&await products.GetByIdAsync(input.ProductId.Value) is null)throw new InvalidOperationException("Product not found.");
  if(input.ProductCategoryId.HasValue&&await categories.GetByIdAsync(input.ProductCategoryId.Value) is null)throw new InvalidOperationException("Product category not found.");
  var duplicate=(await rules.GetWarehouseRulesAsync(input.WarehouseId)).Any(x=>x.StorageLocationId==input.StorageLocationId&&x.ProductId==input.ProductId&&x.ProductCategoryId==input.ProductCategoryId);
  if(duplicate)throw new InvalidOperationException("An identical putaway rule already exists.");
  await rules.AddAsync(new PutawayRule(input.WarehouseId,input.StorageLocationId,input.ProductId,input.ProductCategoryId,input.Priority));await rules.SaveChangesAsync();
 }

 public async Task SetActiveAsync(int id,bool active){var rule=await rules.GetByIdAsync(id)??throw new InvalidOperationException("Putaway rule not found.");rule.SetActive(active);await rules.SaveChangesAsync();}

 private static bool IsAvailable(StorageLocation? location,Warehouse warehouse,decimal quantity,IReadOnlyDictionary<int,decimal> occupied)=>location is {Status:StorageLocationStatus.Active,IsReceivable:true}&&location.WarehouseId==warehouse.Id&&(!warehouse.EnforceLocationCapacity||!location.MaximumQuantity.HasValue||occupied.GetValueOrDefault(location.Id)+quantity<=location.MaximumQuantity.Value);
}
