using MiniStore.Application.DTOs.Inventory.Adjustments;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public sealed class InventoryAdjustmentService(
    IInventoryAdjustmentRepository adjustments, IWarehouseRepository warehouses,
    IStorageLocationRepository locations, IProductRepository products,
    IProductStockRepository stocks, IProductLocationStockRepository locationStocks,
    IStockTransactionRepository transactions, IStockMovementRepository movements,
    DocumentNumberService numbers, IUnitOfWork unitOfWork)
{
    public async Task<List<InventoryAdjustmentRowDto>> GetAllAsync() => await MapRowsAsync(await adjustments.GetAllAsync());
    public async Task<InventoryAdjustmentCreatePageDto> GetCreatePageAsync(CreateInventoryAdjustmentDto? input = null)
    {
        var allWarehouses = await warehouses.GetAllAsync(); var allLocations = await locations.SearchAsync(null, null); var allProducts = await products.GetAllAsync(null);
        return new() { Input = input ?? new(), Warehouses = allWarehouses.Select(x => (x.Id, x.Name)).ToList(),
            Locations = allLocations.Where(x => x.Status == StorageLocationStatus.Active && x.IsCountable).Select(x => (x.Id, x.WarehouseId, $"{x.Code} — {x.Name}")).ToList(),
            Products = allProducts.Where(x => x.IsActive && x.InventoryBehavior == ProductInventoryBehavior.Stocked).Select(x => (x.Id, x.Name)).ToList() };
    }
    public async Task<long> CreateAsync(CreateInventoryAdjustmentDto dto, string userId)
    {
        var warehouse = await warehouses.GetByIdAsync(dto.WarehouseId) ?? throw new ArgumentException("Selected warehouse was not found.");
        if (dto.ProductIds.Count == 0 || dto.ProductIds.Distinct().Count() != dto.ProductIds.Count) throw new ArgumentException("Select one or more unique products.");
        if (dto.StorageLocationId.HasValue)
        {
            var location = await locations.GetByIdAsync(dto.StorageLocationId.Value) ?? throw new ArgumentException("Selected location was not found.");
            if (location.WarehouseId != warehouse.Id || location.Status != StorageLocationStatus.Active || !location.IsCountable)
                throw new InvalidOperationException("The selected location is not active and countable in this warehouse.");
        }
        InventoryAdjustment? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            created = new InventoryAdjustment(await numbers.GenerateAsync(DocumentNumberType.InventoryAdjustment, DateTime.Today),
                warehouse.Id, dto.StorageLocationId, dto.IsBlindCount, dto.Reason, userId);
            foreach (var productId in dto.ProductIds)
            {
                var product = await products.GetByIdAsync(productId) ?? throw new ArgumentException("Selected product was not found.");
                if (product.InventoryBehavior != ProductInventoryBehavior.Stocked) throw new InvalidOperationException("Prepared products cannot be physically counted as finished stock.");
                var balance = await adjustments.GetBalanceAsync(productId, warehouse.Id, dto.StorageLocationId);
                created.AddLine(productId, balance?.OnHand ?? 0m);
            }
            await adjustments.AddAsync(created);
        });
        return created!.Id;
    }
    public async Task<InventoryAdjustmentDetailsDto?> GetDetailsAsync(long id)
    {
        var item = await adjustments.GetByIdAsync(id); if (item is null) return null;
        var row = (await MapRowsAsync([item])).Single(); var names = (await products.GetAllAsync(null)).ToDictionary(x => x.Id, x => x.Name);
        return new InventoryAdjustmentDetailsDto { Id=row.Id, Number=row.Number, Warehouse=row.Warehouse, Location=row.Location, Status=row.Status,
            IsBlindCount=row.IsBlindCount, Reason=row.Reason, CreatedAt=row.CreatedAt, LineCount=row.LineCount,
            Lines=item.Lines.Select(x => new InventoryAdjustmentLineDto { ProductId=x.ProductId, ProductName=names.GetValueOrDefault(x.ProductId, $"Product #{x.ProductId}"),
                ExpectedQuantity=item.IsBlindCount && item.Status == InventoryAdjustmentStatus.Draft ? null : x.ExpectedQuantity,
                CountedQuantity=x.CountedQuantity, VarianceQuantity=x.CountedQuantity.HasValue ? x.VarianceQuantity : null }).ToList() };
    }
    public async Task RecordCountsAsync(long id, RecordInventoryCountsDto dto, string userId) => await unitOfWork.ExecuteInTransactionAsync(async () =>
    { var item = await adjustments.GetByIdAsync(id) ?? throw new InvalidOperationException("Inventory adjustment was not found."); item.RecordCounts(dto.Lines.Select(x => (x.ProductId, x.CountedQuantity)), userId); });
    public async Task RecordScannedLineAsync(string number, int productId, int? locationId, decimal countedQuantity, string userId) => await unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Inventory adjustment number is required.");
        var item = await adjustments.GetByNumberAsync(number.Trim()) ?? throw new InvalidOperationException("Inventory adjustment was not found.");
        if (item.Status != InventoryAdjustmentStatus.Draft) throw new InvalidOperationException("Only draft adjustments can be edited.");
        if (item.StorageLocationId != locationId) throw new InvalidOperationException("The scanned location does not match the inventory adjustment position.");
        item.RecordLineCount(productId, countedQuantity, userId);
    });
    public async Task ApproveAsync(long id, string userId) => await unitOfWork.ExecuteInTransactionAsync(async () =>
    { var item = await adjustments.GetByIdAsync(id) ?? throw new InvalidOperationException("Inventory adjustment was not found."); item.Approve(userId); });
    public async Task CancelAsync(long id, string userId) => await unitOfWork.ExecuteInTransactionAsync(async () =>
    { var item = await adjustments.GetByIdAsync(id) ?? throw new InvalidOperationException("Inventory adjustment was not found."); item.Cancel(userId); });
    public async Task PostAsync(long id, string userId)
    {
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var item = await adjustments.GetByIdAsync(id) ?? throw new InvalidOperationException("Inventory adjustment was not found.");
            if (item.Status != InventoryAdjustmentStatus.Approved) throw new InvalidOperationException("Only an approved inventory adjustment can be posted.");
            foreach (var line in item.Lines)
            {
                var balance = await adjustments.GetBalanceAsync(line.ProductId, item.WarehouseId, item.StorageLocationId);
                var current = balance?.OnHand ?? 0m;
                if (current != line.ExpectedQuantity) throw new InvalidOperationException("Stock changed after the count started. Cancel this adjustment and create a new count.");
                if (!line.CountedQuantity.HasValue) throw new InvalidOperationException("A counted quantity is required for every line.");
                if (line.CountedQuantity.Value < (balance?.Reserved ?? 0m)) throw new InvalidOperationException("Counted quantity cannot be lower than the quantity reserved at this inventory position.");
                var variance = line.VarianceQuantity; if (variance == 0) continue;
                var stock = await stocks.GetByProductAndWarehouseAsync(line.ProductId, item.WarehouseId);
                if (stock is null) { if (variance < 0) throw new InvalidOperationException("Warehouse stock was not found."); stock = new ProductStock(line.ProductId, item.WarehouseId); await stocks.AddAsync(stock); }
                InventoryCostMovement cost;
                if (variance > 0) cost = stock.AddQuantity(variance); else cost = stock.RemoveQuantity(-variance);
                if (item.StorageLocationId.HasValue)
                {
                    var located = await locationStocks.GetAsync(line.ProductId, item.StorageLocationId.Value);
                    if (located is null) { if (variance < 0) throw new InvalidOperationException("Location stock was not found."); located = new ProductLocationStock(line.ProductId, item.WarehouseId, item.StorageLocationId.Value); await locationStocks.AddAsync(located); }
                    if (variance > 0) located.AddQuantity(variance); else located.RemoveQuantity(-variance);
                }
                await transactions.AddAsync(new StockTransaction(line.ProductId, item.WarehouseId, variance,
                    variance > 0 ? StockTransactionType.AdjustmentIn : StockTransactionType.AdjustmentOut, item.AdjustmentNumber, cost));
                await movements.AddAsync(StockMovement.PostAdjustment(item.Id, line.Id, line.ProductId, item.WarehouseId,
                    item.StorageLocationId, variance, userId, item.AdjustmentNumber));
            }
            item.Post(userId);
        });
    }
    private async Task<List<InventoryAdjustmentRowDto>> MapRowsAsync(List<InventoryAdjustment> source)
    {
        var warehouseNames=(await warehouses.GetAllAsync()).ToDictionary(x=>x.Id,x=>x.Name); var locationNames=(await locations.SearchAsync(null,null)).ToDictionary(x=>x.Id,x=>$"{x.Code} — {x.Name}");
        return source.Select(x=>new InventoryAdjustmentRowDto { Id=x.Id, Number=x.AdjustmentNumber, Warehouse=warehouseNames.GetValueOrDefault(x.WarehouseId,"Unknown"),
            Location=x.StorageLocationId.HasValue?locationNames.GetValueOrDefault(x.StorageLocationId.Value,"Unknown"):"Unassigned (system)", Status=x.Status,
            IsBlindCount=x.IsBlindCount, Reason=x.Reason, CreatedAt=x.CreatedAt, LineCount=x.Lines.Count }).ToList();
    }
}
