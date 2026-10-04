using MiniStore.Application.DTOs.Inventory.Tracking;using MiniStore.Domain.Entities;using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public sealed class InventoryTrackingService(IInventoryTrackingRepository tracking,IProductRepository products,IWarehouseRepository warehouses,
 IStorageLocationRepository locations,IInventoryBalanceRepository balances,IProductLocationStockRepository locationStocks,IUnitOfWork unitOfWork)
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

 public async Task ReceiveAsync(Product product,int warehouseId,decimal quantity,string? lotNumber,string? serialNumbers,DateOnly? manufactureDate,DateOnly? expirationDate,string reference,string userId)
 {
  if(product.TrackingPolicy==ProductTrackingPolicy.None)return;
  if(product.TrackingPolicy==ProductTrackingPolicy.Lot)
  {
   if(string.IsNullOrWhiteSpace(lotNumber))throw new InvalidOperationException("A lot number is required for this tracked purchase item.");
   var existing=await tracking.GetBalanceAsync(product.Id,warehouseId,null,lotNumber);
   if(existing is null){existing=new InventoryTrackingBalance(product.Id,warehouseId,null,product.TrackingPolicy,lotNumber,quantity,manufactureDate,expirationDate,reference);await tracking.AddBalanceAsync(existing);}
   else{if(existing.ManufactureDate!=manufactureDate||existing.ExpirationDate!=expirationDate)throw new InvalidOperationException("Existing lot dates do not match this receipt.");existing.Add(quantity);}
   await tracking.AddTransactionAsync(new InventoryTrackingTransaction(existing,quantity,InventoryTrackingTransactionType.Receipt,reference,userId));return;
  }
  if(quantity!=decimal.Truncate(quantity))throw new InvalidOperationException("Serial-tracked receipt quantity must be a whole number.");
  var identifiers=SplitIdentifiers(serialNumbers);
  if(identifiers.Count!=(int)quantity||identifiers.Distinct(StringComparer.OrdinalIgnoreCase).Count()!=identifiers.Count)throw new InvalidOperationException("Provide one unique serial number for every received unit.");
  foreach(var identifier in identifiers){if(await tracking.IdentifierExistsAsync(product.Id,ProductTrackingPolicy.Serial,identifier))throw new InvalidOperationException($"Serial '{identifier}' already exists.");var balance=new InventoryTrackingBalance(product.Id,warehouseId,null,ProductTrackingPolicy.Serial,identifier,1,manufactureDate,expirationDate,reference);await tracking.AddBalanceAsync(balance);await tracking.AddTransactionAsync(new InventoryTrackingTransaction(balance,1,InventoryTrackingTransactionType.Receipt,reference,userId));}
 }

 public async Task IssueAsync(Product product,int warehouseId,decimal quantity,string reference,string userId)
 {
  if(product.TrackingPolicy==ProductTrackingPolicy.None)return;
  if(product.TrackingPolicy==ProductTrackingPolicy.Serial&&quantity!=decimal.Truncate(quantity))throw new InvalidOperationException("Serial-tracked issue quantity must be a whole number.");
  var candidates=await tracking.GetAvailableForUpdateAsync(product.Id,warehouseId);var today=DateOnly.FromDateTime(DateTime.Today);
  var usable=candidates.Where(x=>!x.ExpirationDate.HasValue||x.ExpirationDate>=today).ToList();
  if(usable.Sum(x=>x.Quantity)<quantity)throw new InvalidOperationException($"Insufficient non-expired tracked stock for product '{product.Name}'.");
  var remaining=quantity;
  foreach(var balance in usable){if(remaining<=0)break;var take=Math.Min(balance.Quantity,remaining);balance.Remove(take);if(balance.StorageLocationId.HasValue){var located=await locationStocks.GetAsync(product.Id,balance.StorageLocationId.Value)??throw new InvalidOperationException("Tracked location stock is missing.");located.RemoveQuantity(take);}await tracking.AddTransactionAsync(new InventoryTrackingTransaction(balance,-take,InventoryTrackingTransactionType.Issue,reference,userId));remaining-=take;}
 }

 public async Task TransferAsync(Product product,int fromWarehouseId,int toWarehouseId,int? fromLocationId,int? toLocationId,decimal quantity,string reference,string userId)
 {
  if(product.TrackingPolicy==ProductTrackingPolicy.None)return;
  if(product.TrackingPolicy==ProductTrackingPolicy.Serial&&quantity!=decimal.Truncate(quantity))throw new InvalidOperationException("Serial-tracked transfer quantity must be a whole number.");
  var candidates=(await tracking.GetAvailableForUpdateAsync(product.Id,fromWarehouseId)).Where(x=>x.StorageLocationId==fromLocationId).ToList();
  if(candidates.Sum(x=>x.Quantity)<quantity)throw new InvalidOperationException($"Insufficient tracked stock in the selected source position for product '{product.Name}'.");
  var remaining=quantity;
  foreach(var source in candidates){if(remaining<=0)break;var take=Math.Min(source.Quantity,remaining);var oldWarehouse=source.WarehouseId;var oldLocation=source.StorageLocationId;
   if(source.Policy==ProductTrackingPolicy.Serial){source.Relocate(toWarehouseId,toLocationId);await tracking.AddTransactionAsync(new InventoryTrackingTransaction(source.Id,product.Id,oldWarehouse,oldLocation,source.Identifier,-1,InventoryTrackingTransactionType.Transfer,reference,userId));await tracking.AddTransactionAsync(new InventoryTrackingTransaction(source.Id,product.Id,toWarehouseId,toLocationId,source.Identifier,1,InventoryTrackingTransactionType.Transfer,reference,userId));}
   else{source.Remove(take);var destination=await tracking.GetBalanceAsync(product.Id,toWarehouseId,toLocationId,source.Identifier);if(destination is null){destination=new InventoryTrackingBalance(product.Id,toWarehouseId,toLocationId,source.Policy,source.Identifier,take,source.ManufactureDate,source.ExpirationDate,reference);await tracking.AddBalanceAsync(destination);}else{if(destination.ManufactureDate!=source.ManufactureDate||destination.ExpirationDate!=source.ExpirationDate)throw new InvalidOperationException("Destination lot dates do not match the source lot.");destination.Add(take);}await tracking.AddTransactionAsync(new InventoryTrackingTransaction(source,-take,InventoryTrackingTransactionType.Transfer,reference,userId));await tracking.AddTransactionAsync(new InventoryTrackingTransaction(destination,take,InventoryTrackingTransactionType.Transfer,reference,userId));}
   remaining-=take;
  }
 }

 private static List<string> SplitIdentifiers(string? value)=>(value??string.Empty).Split([',',';','\n','\r'],StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries).Select(x=>x.ToUpperInvariant()).ToList();
}
