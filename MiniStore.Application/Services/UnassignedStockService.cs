using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public class UnassignedStockService(IProductStockRepository warehouseStocks, IProductLocationStockRepository locationStocks,
    IProductRepository products, IWarehouseRepository warehouses, IStorageLocationRepository locations, IUnitOfWork unitOfWork)
{
    public async Task<List<UnassignedStockDto>> SearchAsync(int? warehouseId, string? query, string? sort)
    {
        var productList = await products.GetAllAsync(query); var ids = productList.Select(x => x.Id).ToHashSet();
        var warehouseList = await warehouses.GetAllAsync(); var allocated = await locationStocks.GetAllAsync();
        var rows = (await warehouseStocks.GetAllAsync()).Where(x => ids.Contains(x.ProductId) && (!warehouseId.HasValue || x.WarehouseId == warehouseId))
            .Select(x => new UnassignedStockDto { ProductId=x.ProductId, ProductName=productList.First(p=>p.Id==x.ProductId).Name, Barcode=productList.First(p=>p.Id==x.ProductId).Barcode, WarehouseId=x.WarehouseId, WarehouseName=warehouseList.FirstOrDefault(w=>w.Id==x.WarehouseId)?.Name??"Unknown", WarehouseQuantity=x.Quantity, AssignedQuantity=allocated.Where(a=>a.ProductId==x.ProductId&&a.WarehouseId==x.WarehouseId).Sum(a=>a.Quantity) })
            .Where(x => x.UnassignedQuantity > 0).ToList();
        return sort == "barcode" ? rows.OrderBy(x=>x.Barcode).ToList() : sort == "warehouse" ? rows.OrderBy(x=>x.WarehouseName).ThenBy(x=>x.ProductName).ToList() : rows.OrderBy(x=>x.ProductName).ToList();
    }

    public async Task AssignAsync(int productId, int warehouseId, int storageLocationId, decimal quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
        var location = await locations.GetByIdAsync(storageLocationId) ?? throw new InvalidOperationException("Storage location not found.");
        if (location.WarehouseId != warehouseId || location.Status != StorageLocationStatus.Active) throw new InvalidOperationException("Select an active location in the same warehouse.");
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var warehouseStock = await warehouseStocks.GetByProductAndWarehouseAsync(productId, warehouseId) ?? throw new InvalidOperationException("Warehouse stock not found.");
            var allocated = (await locationStocks.GetAllAsync()).Where(x=>x.ProductId==productId&&x.WarehouseId==warehouseId).Sum(x=>x.Quantity);
            if (quantity > warehouseStock.Quantity - allocated) throw new InvalidOperationException("Quantity exceeds the unassigned balance.");
            var target=await locationStocks.GetAsync(productId, storageLocationId);
            if (location.MaximumQuantity.HasValue && (target?.Quantity ?? 0) + quantity > location.MaximumQuantity.Value) throw new InvalidOperationException("Quantity exceeds the location capacity.");
            if(target is null){target=new ProductLocationStock(productId,warehouseId,storageLocationId);await locationStocks.AddAsync(target);} target.AddQuantity(quantity);
        });
    }
}
