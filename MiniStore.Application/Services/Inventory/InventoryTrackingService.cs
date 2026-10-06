using MiniStore.Application.DTOs.Inventory.Tracking;using MiniStore.Domain.Entities;using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public sealed class InventoryTrackingService(IInventoryTrackingRepository tracking,IInventoryRecallRepository recalls,IProductRepository products,IWarehouseRepository warehouses,
 IStorageLocationRepository locations,IInventoryBalanceRepository balances,IProductLocationStockRepository locationStocks,IPurchaseRepository purchases,ISupplierRepository suppliers,
 ISaleRepository sales,ICustomerRepository customers,IUnitOfWork unitOfWork)
{
 public async Task<InventoryTrackingPageDto> GetPageAsync(OpenTrackingAllocationDto? input=null)
 {
  var ps=await products.GetAllAsync(null);var ws=await warehouses.GetAllAsync();var ls=await locations.SearchAsync(null,null);
  var productMap=ps.ToDictionary(x=>x.Id);var names=ps.ToDictionary(x=>x.Id,x=>x.Name);var warehouseNames=ws.ToDictionary(x=>x.Id,x=>x.Name);var locationNames=ls.ToDictionary(x=>x.Id,x=>$"{x.Code} — {x.Name}");
  var bs=await tracking.GetBalancesAsync();var ts=await tracking.GetTransactionsAsync();var recallRows=await recalls.GetAllAsync();var communicationRows=await recalls.GetCommunicationsAsync();
  var purchaseMap=(await purchases.GetAllAsync()).ToDictionary(x=>x.InvoiceNumber,StringComparer.OrdinalIgnoreCase);var supplierMap=(await suppliers.GetAllAsync()).ToDictionary(x=>x.Id);
  var saleMap=(await sales.GetAllAsync()).ToDictionary(x=>x.InvoiceNumber,StringComparer.OrdinalIgnoreCase);var customerMap=(await customers.GetAllAsync()).ToDictionary(x=>x.Id);
  var today=DateOnly.FromDateTime(DateTime.Today);var rows=bs.Select(x=>{var days=x.ExpirationDate.HasValue?x.ExpirationDate.Value.DayNumber-today.DayNumber:(int?)null;var warning=productMap.GetValueOrDefault(x.ProductId)?.ExpirationWarningDays??30;var state=!days.HasValue?"No expiration":days<0?"Expired":days<=warning?"Expiring soon":"Valid";return new TrackingBalanceRowDto{Id=x.Id,Product=names.GetValueOrDefault(x.ProductId,"Unknown"),Warehouse=warehouseNames.GetValueOrDefault(x.WarehouseId,"Unknown"),Location=x.StorageLocationId.HasValue?locationNames.GetValueOrDefault(x.StorageLocationId.Value,"Unknown"):"Unassigned (system)",Policy=x.Policy,Identifier=x.Identifier,Quantity=x.Quantity,ExpirationDate=x.ExpirationDate,Status=x.Status,ExpirationState=state,DaysUntilExpiration=days};}).ToList();
  var alerts=rows.Where(x=>x.Quantity>0&&x.ExpirationDate.HasValue&&x.ExpirationState is "Expired" or "Expiring soon").OrderBy(x=>x.DaysUntilExpiration).Select(x=>new ExpirationAlertRowDto{Product=x.Product,Warehouse=x.Warehouse,Location=x.Location,Identifier=x.Identifier,Quantity=x.Quantity,ExpirationDate=x.ExpirationDate!.Value,DaysUntilExpiration=x.DaysUntilExpiration!.Value,Severity=x.ExpirationState,Status=x.Status}).ToList();
  return new(){Input=input??new(),Products=ps.Where(x=>x.InventoryBehavior==ProductInventoryBehavior.Stocked&&x.TrackingPolicy==ProductTrackingPolicy.None).Select(x=>(x.Id,x.Name)).ToList(),TrackedProducts=ps.Where(x=>x.TrackingPolicy!=ProductTrackingPolicy.None).Select(x=>(x.Id,x.Name)).ToList(),
   Warehouses=ws.Select(x=>(x.Id,x.Name)).ToList(),Locations=ls.Where(x=>x.Status==StorageLocationStatus.Active).Select(x=>(x.Id,x.WarehouseId,$"{x.Code} — {x.Name}")).ToList(),
   Balances=rows,ExpirationAlerts=alerts,ExpiredCount=alerts.Count(x=>x.Severity=="Expired"),ExpiringSoonCount=alerts.Count(x=>x.Severity=="Expiring soon"),
   Transactions=ts.Take(200).Select(x=>new TrackingTransactionRowDto{Product=names.GetValueOrDefault(x.ProductId,"Unknown"),Identifier=x.Identifier,Quantity=x.Quantity,Type=x.Type,Reference=x.SourceReference,CreatedAt=x.CreatedAt}).ToList(),Recalls=recallRows.Select(x=>new InventoryRecallRowDto{Id=x.Id,Reference=x.Reference,Product=names.GetValueOrDefault(x.ProductId,"Unknown"),Identifier=x.Identifier,Reason=x.Reason,Status=x.Status,CreatedAt=x.CreatedAt,ClosedAt=x.ClosedAt,ClosureNotes=x.ClosureNotes,Impacts=BuildRecallImpacts(ts.Where(t=>t.ProductId==x.ProductId&&t.Identifier==x.Identifier),purchaseMap,supplierMap,saleMap,customerMap),Communications=communicationRows.Where(c=>c.InventoryRecallId==x.Id).Select(c=>new RecallCommunicationRowDto{PartyName=c.PartyName,ChannelAddress=c.ChannelAddress,Channel=c.Channel,Outcome=c.Outcome,Notes=c.Notes,CreatedAt=c.CreatedAt}).ToList()}).ToList()};
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
  if(!expirationDate.HasValue&&product.DefaultShelfLifeDays.HasValue)expirationDate=(manufactureDate??DateOnly.FromDateTime(DateTime.Today)).AddDays(product.DefaultShelfLifeDays.Value);
  if(product.RequireExpirationDate&&!expirationDate.HasValue)throw new InvalidOperationException("An expiration date is required for this product.");
  if(expirationDate.HasValue&&expirationDate<DateOnly.FromDateTime(DateTime.Today))throw new InvalidOperationException("Expired inventory cannot be received into available stock.");
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
  var today=DateOnly.FromDateTime(DateTime.Today);
  var candidates=(await tracking.GetAvailableForUpdateAsync(product.Id,fromWarehouseId)).Where(x=>x.StorageLocationId==fromLocationId&&(!x.ExpirationDate.HasValue||x.ExpirationDate>=today)).ToList();
  if(candidates.Sum(x=>x.Quantity)<quantity)throw new InvalidOperationException($"Insufficient tracked stock in the selected source position for product '{product.Name}'.");
  var remaining=quantity;
  foreach(var source in candidates){if(remaining<=0)break;var take=Math.Min(source.Quantity,remaining);var oldWarehouse=source.WarehouseId;var oldLocation=source.StorageLocationId;
   if(source.Policy==ProductTrackingPolicy.Serial){source.Relocate(toWarehouseId,toLocationId);await tracking.AddTransactionAsync(new InventoryTrackingTransaction(source.Id,product.Id,oldWarehouse,oldLocation,source.Identifier,-1,InventoryTrackingTransactionType.Transfer,reference,userId));await tracking.AddTransactionAsync(new InventoryTrackingTransaction(source.Id,product.Id,toWarehouseId,toLocationId,source.Identifier,1,InventoryTrackingTransactionType.Transfer,reference,userId));}
   else{source.Remove(take);var destination=await tracking.GetBalanceAsync(product.Id,toWarehouseId,toLocationId,source.Identifier);if(destination is null){destination=new InventoryTrackingBalance(product.Id,toWarehouseId,toLocationId,source.Policy,source.Identifier,take,source.ManufactureDate,source.ExpirationDate,reference);await tracking.AddBalanceAsync(destination);}else{if(destination.ManufactureDate!=source.ManufactureDate||destination.ExpirationDate!=source.ExpirationDate)throw new InvalidOperationException("Destination lot dates do not match the source lot.");destination.Add(take);}await tracking.AddTransactionAsync(new InventoryTrackingTransaction(source,-take,InventoryTrackingTransactionType.Transfer,reference,userId));await tracking.AddTransactionAsync(new InventoryTrackingTransaction(destination,take,InventoryTrackingTransactionType.Transfer,reference,userId));}
   remaining-=take;
  }
 }

 public async Task ReturnSaleAsync(Product product,int warehouseId,decimal quantity,string? allocationText,string originalReference,string returnReference,string userId)
 {
  if(product.TrackingPolicy==ProductTrackingPolicy.None)return;
  var allocations=ParseAllocations(product,allocationText,quantity);
  var history=await tracking.GetTransactionsAsync();
  foreach(var allocation in allocations)
  {
   var issued=-history.Where(x=>x.ProductId==product.Id&&x.Identifier==allocation.Identifier&&x.Type==InventoryTrackingTransactionType.Issue&&x.SourceReference==originalReference).Sum(x=>Math.Min(0,x.Quantity));
   var previouslyReturned=history.Where(x=>x.ProductId==product.Id&&x.Identifier==allocation.Identifier&&x.Type==InventoryTrackingTransactionType.Return&&x.SourceReference.EndsWith($"|{originalReference}",StringComparison.Ordinal)).Sum(x=>x.Quantity);
   if(allocation.Quantity>issued-previouslyReturned)throw new InvalidOperationException("The selected lot or serial was not issued by the original sale, or it was already returned.");
   var matches=await tracking.GetByIdentifierForUpdateAsync(product.Id,allocation.Identifier);
   if(matches.Count==0)throw new InvalidOperationException("The selected lot or serial was not found.");
   if(product.TrackingPolicy==ProductTrackingPolicy.Serial)
   {
    var serial=matches.Single();
    if(serial.Quantity!=0)throw new InvalidOperationException("The selected serial is already available in inventory.");
    serial.Restore(1);serial.Relocate(warehouseId,null);
    await tracking.AddTransactionAsync(new InventoryTrackingTransaction(serial,1,InventoryTrackingTransactionType.Return,$"{returnReference}|{originalReference}",userId));
   }
   else
   {
    var source=matches[0];var destination=await tracking.GetBalanceAsync(product.Id,warehouseId,null,allocation.Identifier);
    if(destination is null){destination=new InventoryTrackingBalance(product.Id,warehouseId,null,ProductTrackingPolicy.Lot,allocation.Identifier,allocation.Quantity,source.ManufactureDate,source.ExpirationDate,returnReference);await tracking.AddBalanceAsync(destination);}else destination.Restore(allocation.Quantity);
    await tracking.AddTransactionAsync(new InventoryTrackingTransaction(destination,allocation.Quantity,InventoryTrackingTransactionType.Return,$"{returnReference}|{originalReference}",userId));
   }
  }
 }

 public async Task ReturnPurchaseAsync(Product product,int warehouseId,decimal quantity,string? allocationText,string originalReference,string returnReference,string userId)
 {
  if(product.TrackingPolicy==ProductTrackingPolicy.None)return;
  var allocations=ParseAllocations(product,allocationText,quantity);
  var history=await tracking.GetTransactionsAsync();
  foreach(var allocation in allocations)
  {
   var received=history.Where(x=>x.ProductId==product.Id&&x.Identifier==allocation.Identifier&&x.Type==InventoryTrackingTransactionType.Receipt&&x.SourceReference==originalReference).Sum(x=>x.Quantity);
   var previouslyReturned=-history.Where(x=>x.ProductId==product.Id&&x.Identifier==allocation.Identifier&&x.Type==InventoryTrackingTransactionType.Return&&x.Quantity<0&&x.SourceReference.EndsWith($"|{originalReference}",StringComparison.Ordinal)).Sum(x=>x.Quantity);
   if(allocation.Quantity>received-previouslyReturned)throw new InvalidOperationException("The selected lot or serial was not received by the original purchase, or it was already returned.");
   var candidates=(await tracking.GetAvailableForUpdateAsync(product.Id,warehouseId)).Where(x=>x.Identifier==allocation.Identifier).ToList();
   if(candidates.Sum(x=>x.Quantity)<allocation.Quantity)throw new InvalidOperationException("The selected lot or serial is not available in the purchase warehouse.");
   var remaining=allocation.Quantity;
   foreach(var balance in candidates){if(remaining<=0)break;var take=Math.Min(balance.Quantity,remaining);balance.Remove(take);if(balance.StorageLocationId.HasValue){var located=await locationStocks.GetAsync(product.Id,balance.StorageLocationId.Value)??throw new InvalidOperationException("Tracked location stock is missing.");located.RemoveQuantity(take);}await tracking.AddTransactionAsync(new InventoryTrackingTransaction(balance,-take,InventoryTrackingTransactionType.Return,$"{returnReference}|{originalReference}",userId));remaining-=take;}
  }
 }

 public Task QuarantineAsync(long balanceId,string reason,string userId)=>ChangeQuarantineAsync(balanceId,reason,userId,true);
 public Task ReleaseAsync(long balanceId,string reason,string userId)=>ChangeQuarantineAsync(balanceId,reason,userId,false);
 private async Task ChangeQuarantineAsync(long balanceId,string reason,string userId,bool quarantine)
 {
  var normalizedReason=reason?.Trim()??string.Empty;
  if(normalizedReason.Length<3||normalizedReason.Length>90)throw new ArgumentException("Quarantine reason must be between 3 and 90 characters.");
  await unitOfWork.ExecuteInTransactionAsync(async()=>
  {
   var balance=await tracking.GetByIdForUpdateAsync(balanceId)??throw new InvalidOperationException("Tracked inventory balance was not found.");
   if(quarantine)balance.Quarantine();else balance.Release();
   await tracking.AddTransactionAsync(new InventoryTrackingTransaction(balance,0,
    quarantine?InventoryTrackingTransactionType.Quarantine:InventoryTrackingTransactionType.Release,
    normalizedReason,userId));
  });
 }

 public async Task CreateRecallAsync(string reference,int productId,string identifier,string reason,string userId)
 {
  var product=await products.GetByIdAsync(productId)??throw new InvalidOperationException("Recall product was not found.");
  if(product.TrackingPolicy==ProductTrackingPolicy.None)throw new InvalidOperationException("Recall requires a lot- or serial-tracked product.");
  var normalized=identifier?.Trim().ToUpperInvariant()??string.Empty;
  var recall=new InventoryRecall(reference,productId,normalized,reason,userId);
  await unitOfWork.ExecuteInTransactionAsync(async()=>
  {
   var matches=await tracking.GetByIdentifierForUpdateAsync(productId,normalized);
   if(matches.Count==0)throw new InvalidOperationException("The recalled lot or serial was not found.");
   if(await recalls.HasActiveAsync(productId,normalized))throw new InvalidOperationException("An active recall already exists for this lot or serial.");
   if(await recalls.ReferenceExistsAsync(recall.Reference))throw new InvalidOperationException("Recall reference already exists.");
   await recalls.AddAsync(recall);
   foreach(var balance in matches.Where(x=>x.Quantity>0&&x.Status==InventoryTrackingStatus.Available))
   {balance.Quarantine();await tracking.AddTransactionAsync(new InventoryTrackingTransaction(balance,0,InventoryTrackingTransactionType.Quarantine,$"Recall {recall.Reference}",userId));}
  });
 }
 public async Task CloseRecallAsync(long recallId,string notes,string userId)
 {
  await unitOfWork.ExecuteInTransactionAsync(async()=>{var recall=await recalls.GetByIdForUpdateAsync(recallId)??throw new InvalidOperationException("Inventory recall was not found.");recall.Close(notes,userId);});
 }
 public async Task RecordRecallCommunicationAsync(long recallId,string partyName,string? channelAddress,RecallCommunicationChannel channel,RecallCommunicationOutcome outcome,string notes,string userId)
 {
  await unitOfWork.ExecuteInTransactionAsync(async()=>{var recall=await recalls.GetByIdForUpdateAsync(recallId)??throw new InvalidOperationException("Inventory recall was not found.");if(recall.Status!=InventoryRecallStatus.Active)throw new InvalidOperationException("Communication can only be recorded for an active recall.");await recalls.AddCommunicationAsync(new InventoryRecallCommunication(recallId,partyName,channelAddress,channel,outcome,notes,userId));});
 }

 private static List<(string Identifier,decimal Quantity)> ParseAllocations(Product product,string? value,decimal expectedQuantity)
 {
  var lines=(value??string.Empty).Split([',',';','\n','\r'],StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries);
  if(lines.Length==0)throw new InvalidOperationException("Lot or serial allocations are required for the tracked return.");
  var parsed=new List<(string Identifier,decimal Quantity)>();
  foreach(var line in lines){var separator=line.LastIndexOf(':');var identifier=separator>0?line[..separator].Trim():line.Trim();var amount=1m;if(separator>0&&!decimal.TryParse(line[(separator+1)..].Trim(),System.Globalization.NumberStyles.Number,System.Globalization.CultureInfo.InvariantCulture,out amount))throw new InvalidOperationException("Use LOT:QUANTITY format for lot allocations.");if(string.IsNullOrWhiteSpace(identifier)||amount<=0)throw new InvalidOperationException("Every tracked return allocation requires an identifier and positive quantity.");if(product.TrackingPolicy==ProductTrackingPolicy.Serial&&amount!=1)throw new InvalidOperationException("Every returned serial must have quantity one.");parsed.Add((identifier.ToUpperInvariant(),amount));}
  var grouped=parsed.GroupBy(x=>x.Identifier,StringComparer.OrdinalIgnoreCase).Select(x=>(x.Key,x.Sum(y=>y.Quantity))).ToList();
  if(grouped.Sum(x=>x.Item2)!=expectedQuantity)throw new InvalidOperationException("Tracked return allocations must exactly equal the return quantity.");
  if(product.TrackingPolicy==ProductTrackingPolicy.Serial&&grouped.Count!=parsed.Count)throw new InvalidOperationException("Returned serial numbers must be unique.");
  return grouped;
 }

 private static List<string> SplitIdentifiers(string? value)=>(value??string.Empty).Split([',',';','\n','\r'],StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries).Select(x=>x.ToUpperInvariant()).ToList();
 private static List<RecallImpactRowDto> BuildRecallImpacts(IEnumerable<InventoryTrackingTransaction> transactions,IReadOnlyDictionary<string,Purchase> purchaseMap,IReadOnlyDictionary<int,Supplier> supplierMap,IReadOnlyDictionary<string,Sale> saleMap,IReadOnlyDictionary<int,Customer> customerMap)
 {
  return transactions.Where(x=>x.Type is InventoryTrackingTransactionType.Receipt or InventoryTrackingTransactionType.Issue or InventoryTrackingTransactionType.Transfer or InventoryTrackingTransactionType.Return)
   .GroupBy(x=>new{x.Type,x.SourceReference}).Select(group=>{var reference=group.Key.SourceReference;var type=group.Key.Type switch{InventoryTrackingTransactionType.Receipt=>"Purchase",InventoryTrackingTransactionType.Issue=>"Sale",InventoryTrackingTransactionType.Transfer=>"Transfer",_=>"Return"};var party="—";string? contact=null;if(group.Key.Type==InventoryTrackingTransactionType.Receipt&&purchaseMap.TryGetValue(reference,out var purchase)&&supplierMap.TryGetValue(purchase.SupplierId,out var supplier)){party=supplier.Name;contact=supplier.Phone;}else if(group.Key.Type==InventoryTrackingTransactionType.Issue&&saleMap.TryGetValue(reference,out var sale)){party=sale.CustomerId.HasValue&&customerMap.TryGetValue(sale.CustomerId.Value,out var customer)?customer.Name:"Walk-in customer";}var quantity=group.Key.Type==InventoryTrackingTransactionType.Transfer?group.Where(x=>x.Quantity>0).Sum(x=>x.Quantity):group.Sum(x=>Math.Abs(x.Quantity));return new RecallImpactRowDto{DocumentType=type,Reference=reference,Party=party,Contact=contact,Quantity=quantity,LastActivityAt=group.Max(x=>x.CreatedAt)};}).OrderByDescending(x=>x.LastActivityAt).Take(50).ToList();
 }
}
