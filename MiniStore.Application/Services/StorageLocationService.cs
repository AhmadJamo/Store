using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class StorageLocationService(IStorageLocationRepository locations, IWarehouseRepository warehouses,
    IProductStockRepository stocks, IProductRepository products)
{
    public async Task<WarehouseInventorySearchDto> SearchAsync(int? warehouseId, string? query)
    {
        var allWarehouses = await warehouses.GetAllAsync();
        var allProducts = await products.GetAllAsync(query);
        var productIds = allProducts.Select(x => x.Id).ToHashSet();
        var rows = (await stocks.GetAllAsync())
            .Where(x => (!warehouseId.HasValue || x.WarehouseId == warehouseId) && productIds.Contains(x.ProductId))
            .Select(x => new WarehouseProductSearchRowDto
            {
                ProductId = x.ProductId,
                ProductName = allProducts.First(p => p.Id == x.ProductId).Name,
                Barcode = allProducts.First(p => p.Id == x.ProductId).Barcode,
                WarehouseName = allWarehouses.FirstOrDefault(w => w.Id == x.WarehouseId)?.Name ?? "Unknown",
                Quantity = x.Quantity
            }).ToList();
        return new WarehouseInventorySearchDto { Locations = await locations.SearchAsync(warehouseId, query), Products = rows };
    }

    public async Task CreateAsync(CreateStorageLocationDto dto)
    {
        if (await warehouses.GetByIdAsync(dto.WarehouseId) is null) throw new InvalidOperationException("Warehouse not found.");
        if (await locations.CodeExistsAsync(dto.WarehouseId, dto.Code.Trim().ToUpperInvariant())) throw new InvalidOperationException("Location code already exists in this warehouse.");
        await locations.AddAsync(new StorageLocation(dto.WarehouseId, dto.Code, dto.Zone, dto.Aisle, dto.Rack, dto.Level, dto.Bin, dto.Type, dto.MaximumQuantity));
        await locations.SaveChangesAsync();
    }
}
