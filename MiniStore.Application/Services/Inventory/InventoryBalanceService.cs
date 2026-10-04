using MiniStore.Application.DTOs.Inventory.Balances;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class InventoryBalanceService(
    IInventoryBalanceRepository balances,
    IProductRepository products,
    IWarehouseRepository warehouses,
    IStorageLocationRepository locations)
{
    public async Task<InventoryBalancePageDto> GetPageAsync(int? warehouseId, string? search)
    {
        var productRows = await products.GetAllAsync(search);
        var warehouseRows = await warehouses.GetAllAsync();
        var locationRows = await locations.SearchAsync(null, null);
        var productMap = productRows.ToDictionary(x => x.Id);
        var warehouseMap = warehouseRows.ToDictionary(x => x.Id, x => x.Name);
        var locationMap = locationRows.ToDictionary(x => x.Id, x => string.IsNullOrWhiteSpace(x.Name) ? x.Code : $"{x.Code} — {x.Name}");
        var rows = (await balances.GetAllAsync())
            .Where(x => (!warehouseId.HasValue || x.WarehouseId == warehouseId) && productMap.ContainsKey(x.ProductId))
            .Select(x => new InventoryBalanceRowDto
            {
                ProductName = productMap[x.ProductId].Name,
                ProductCode = productMap[x.ProductId].ProductCode,
                WarehouseId = x.WarehouseId,
                WarehouseName = warehouseMap.GetValueOrDefault(x.WarehouseId, "Unknown Warehouse"),
                LocationName = x.StorageLocationId.HasValue
                    ? locationMap.GetValueOrDefault(x.StorageLocationId.Value, "Unknown Location") : "Unassigned",
                IsUnassigned = !x.StorageLocationId.HasValue,
                OnHand = x.OnHand, Reserved = x.Reserved, Available = x.Available
            }).OrderBy(x => x.WarehouseName).ThenBy(x => x.ProductName).ThenByDescending(x => x.IsUnassigned).ToList();
        return new InventoryBalancePageDto
        {
            Rows = rows, WarehouseId = warehouseId, Search = search?.Trim() ?? string.Empty,
            Warehouses = warehouseRows.OrderBy(x => x.Name).Select(x => (x.Id, x.Name)).ToList()
        };
    }
}
