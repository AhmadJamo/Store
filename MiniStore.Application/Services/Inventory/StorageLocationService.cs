using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class StorageLocationService(
    IStorageLocationRepository locationRepository,
    IWarehouseRepository warehouseRepository,
    IProductStockRepository productStockRepository,
    IProductRepository productRepository)
{
    public async Task<WarehouseInventorySearchDto> SearchAsync(int? warehouseId, string? query)
    {
        var warehouses = await warehouseRepository.GetAllAsync();
        var products = await productRepository.GetAllAsync(query);
        var stocks = await productStockRepository.GetAllAsync();

        var productsById = products.ToDictionary(product => product.Id);
        var warehouseNamesById = warehouses.ToDictionary(
            warehouse => warehouse.Id,
            warehouse => warehouse.Name);

        var productRows = stocks
            .Where(stock =>
                (!warehouseId.HasValue || stock.WarehouseId == warehouseId) &&
                productsById.ContainsKey(stock.ProductId))
            .Select(stock => new WarehouseProductSearchRowDto
            {
                ProductId = stock.ProductId,
                ProductName = productsById[stock.ProductId].Name,
                Barcode = productsById[stock.ProductId].Barcode,
                WarehouseName = warehouseNamesById.GetValueOrDefault(
                    stock.WarehouseId,
                    "Unknown"),
                Quantity = stock.Quantity
            })
            .ToList();

        return new WarehouseInventorySearchDto
        {
            Locations = await locationRepository.SearchAsync(warehouseId, query),
            Products = productRows
        };
    }

    public async Task CreateAsync(CreateStorageLocationDto dto)
    {
        if (await warehouseRepository.GetByIdAsync(dto.WarehouseId) is null)
        {
            throw new InvalidOperationException("Warehouse not found.");
        }

        var normalizedCode = dto.Code.Trim().ToUpperInvariant();
        if (await locationRepository.CodeExistsAsync(dto.WarehouseId, normalizedCode))
        {
            throw new InvalidOperationException(
                "Location code already exists in this warehouse.");
        }

        var location = new StorageLocation(
            dto.WarehouseId,
            dto.Code,
            dto.Zone,
            dto.Aisle,
            dto.Rack,
            dto.Level,
            dto.Bin,
            dto.Type,
            dto.MaximumQuantity);

        await locationRepository.AddAsync(location);
        await locationRepository.SaveChangesAsync();
    }
}
