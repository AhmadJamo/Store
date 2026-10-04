using MiniStore.Application.DTOs.Inventory.Tracking;using MiniStore.Domain.Entities;using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public sealed class InventoryTrackingService(IInventoryTrackingRepository tracking,IProductRepository products,IWarehouseRepository warehouses,
 IStorageLocationRepository locations,IInventoryBalanceRepository balances,IUnitOfWork unitOfWork)
{
 public async Task<InventoryTrackingPageDto> GetPageAsync(OpenTrackingAllocationDto? input=null)
 {
  var ps=await products.GetAllAsync(null);var ws=await warehouses.GetAllAsync();var ls=await locations.SearchAsync(null,null);
  var names=ps.ToDictionary(x=>x.Id,x=>x.Name);var warehouseNames=ws.ToDictionary(x=>x.Id,x=>x.Name);var locationNames=ls.ToDictionary(x=>x.Id,x=>$"{x.Code} — {x.Name}");
  var bs=await tracking.GetBalancesAsync();var ts=await tracking.GetTransactionsAsync();
  return new(){Input=input??new(),Products=ps.Where(x=>x.InventoryBehavior==ProductInventoryBehavior.Stocked&&x.TrackingPolicy==ProductTrackingPolicy.None).Select(x=>(x.Id,x.Name)).ToList(),
   Warehouses=ws.Select(x=>(x.Id,x.Name)).ToList(),Locations=ls.Where(x=>x.Status==StorageLocationStatus.Active).Select(x=>(x.Id,x.WarehouseId,$"{x.Code} — {x.Name}")).ToList(),
   Balances=bs.Select(x=>new TrackingBalanceRowDto{Product=names.GetValueOrDefault(x.ProductId,"Unknown"),Warehouse=warehouseNames.GetValueOrDefault(x.WarehouseId,"Unknown"),Location=x.StorageLocationId.HasValue?locationNames.GetValueOrDefault(x.StorageLocationId.Value,"Unknown"):"Unassigned (system)",Policy=x.Policy,Identifier=x.Identifier,Quantity=x.Quantity,ExpirationDate=x.ExpirationDate,Status=x.Status}).ToList(),
   Transactions=ts.Take(200).Select(x=>new TrackingTransactionRowDto{Product=names.GetValueOrDefault(x.ProductId,"Unknown"),Identifier=x.Identifier,Quantity=x.Quantity,Type=x.Type,Reference=x.SourceReference,CreatedAt=x.CreatedAt}).ToList()};
 }
 public async Task OpenAsync(OpenTrackingAllocationDto dto,string userId)
 {
  if(dto.Policy is not(ProductTrackingPolicy.Lot or ProductTrackingPolicy.Serial))throw new ArgumentException("Select lot or serial tracking.");
  if(dto.Lines.Count==0)throw new ArgumentException("At least one tracking allocation is required.");
  var product=await products.GetByIdAsync(dto.ProductId)??throw new ArgumentException("Selected product was not found.");
  if(product.TrackingPolicy!=ProductTrackingPolicy.None)throw new InvalidOperationException("Product tracking has already been activated.");
  var current=(await balances.GetAllAsync()).Where(x=>x.ProductId==product.Id&&x.OnHand!=0).ToList();
  if(current.Count==0)throw new InvalidOperationException("Opening allocation requires an existing non-zero stock balance.");
  if(current.Any(x=>x.OnHand<0))throw new InvalidOperationException("Negative stock must be reconciled before tracking can be activated.");
  var duplicateIdentifiers=dto.Lines.GroupBy(x=>dto.Policy==ProductTrackingPolicy.Serial
      ?x.Identifier.Trim().ToUpperInvariant()
      :$"{x.Identifier.Trim().ToUpperInvariant()}|{x.WarehouseId}|{x.StorageLocationId}").Where(x=>x.Count()>1).Select(x=>x.Key).ToList();
  if(duplicateIdentifiers.Count>0)throw new InvalidOperationException("Lot or serial identifiers must be unique in the opening allocation.");
  foreach(var line in dto.Lines)
  {
   if(line.StorageLocationId.HasValue){var location=await locations.GetByIdAsync(line.StorageLocationId.Value)??throw new ArgumentException("Tracking location was not found.");if(location.WarehouseId!=line.WarehouseId)throw new InvalidOperationException("Tracking location must belong to its warehouse.");}
   if(dto.Policy==ProductTrackingPolicy.Serial&&line.Quantity!=1)throw new ArgumentException("Every serial allocation must have quantity one.");
   if(await tracking.IdentifierExistsAsync(product.Id,dto.Policy,line.Identifier))throw new InvalidOperationException("A lot or serial identifier already exists for this product.");
  }
  var requested=dto.Lines.GroupBy(x=>(x.WarehouseId,x.StorageLocationId)).ToDictionary(x=>x.Key,x=>x.Sum(y=>y.Quantity));
  var expected=current.ToDictionary(x=>(x.WarehouseId,x.StorageLocationId),x=>x.OnHand);
  if(requested.Count!=expected.Count||expected.Any(x=>!requested.TryGetValue(x.Key,out var q)||q!=x.Value))throw new InvalidOperationException("Opening allocations must exactly cover every current warehouse and location balance for the product.");
  await unitOfWork.ExecuteInTransactionAsync(async()=>
  {
   foreach(var line in dto.Lines){var balance=new InventoryTrackingBalance(product.Id,line.WarehouseId,line.StorageLocationId,dto.Policy,line.Identifier,line.Quantity,line.ManufactureDate,line.ExpirationDate,dto.SourceReference);await tracking.AddBalanceAsync(balance);await tracking.AddTransactionAsync(new InventoryTrackingTransaction(balance,line.Quantity,InventoryTrackingTransactionType.OpeningAllocation,dto.SourceReference,userId));}
   product.ConfigureLogistics(product.ProductCategoryId,product.NetWeight,product.GrossWeight,product.WeightMeasurementUnitId,product.Length,product.Width,product.Height,product.DimensionMeasurementUnitId,dto.Policy,product.HandlingRequirements);
  });
 }
}
