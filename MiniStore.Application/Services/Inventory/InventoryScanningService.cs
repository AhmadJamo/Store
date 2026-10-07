using MiniStore.Application.DTOs.Inventory.Scanning;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class InventoryScanningService(
    IProductRepository products,
    IStorageLocationRepository locations,
    IWarehouseRepository warehouses,
    IInventoryBalanceRepository balances)
{
    public async Task<InventoryScanPageDto> ResolveAsync(InventoryScanQueryDto query)
    {
        var productToken = InventoryScanResolver.Normalize(query.ProductScan);
        var locationToken = InventoryScanResolver.Normalize(query.LocationScan);
        var productRows = await products.GetAllAsync(null);
        var locationRows = await locations.SearchAsync(null, null);
        var warehouseRows = await warehouses.GetAllAsync();

        var productMatches = string.IsNullOrEmpty(productToken)
            ? []
            : productRows.Where(x =>
                    string.Equals(x.Barcode, productToken, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.ProductCode, productToken, StringComparison.OrdinalIgnoreCase))
                .ToList();
        var locationMatches = string.IsNullOrEmpty(locationToken)
            ? []
            : locationRows.Where(x =>
                    string.Equals(x.Barcode, locationToken, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.Code, locationToken, StringComparison.OrdinalIgnoreCase))
                .ToList();

        var productResult = ProductResult(productToken, productMatches);
        var locationResult = LocationResult(locationToken, locationMatches, warehouseRows);
        var balanceRows = new List<InventoryScanBalanceDto>();

        if (productResult.Status == InventoryScanStatus.Resolved &&
            locationResult.Status is InventoryScanStatus.Empty or InventoryScanStatus.Resolved)
        {
            var warehouseMap = warehouseRows.ToDictionary(x => x.Id, x => x.Name);
            var locationMap = locationRows.ToDictionary(x => x.Id);
            var resolvedLocation = locationResult.Status == InventoryScanStatus.Resolved
                ? locationMatches.Single()
                : null;

            balanceRows = (await balances.GetAllAsync())
                .Where(x => x.ProductId == productResult.EntityId)
                .Where(x => resolvedLocation is null || x.StorageLocationId == resolvedLocation.Id)
                .Select(x => new InventoryScanBalanceDto
                {
                    WarehouseName = warehouseMap.GetValueOrDefault(x.WarehouseId, "Unknown Warehouse"),
                    LocationName = x.StorageLocationId.HasValue && locationMap.TryGetValue(x.StorageLocationId.Value, out var location)
                        ? string.IsNullOrWhiteSpace(location.Name) ? location.Code : $"{location.Code} — {location.Name}"
                        : "Unassigned",
                    IsUnassigned = !x.StorageLocationId.HasValue,
                    OnHand = x.OnHand,
                    Reserved = x.Reserved,
                    Available = x.Available
                })
                .OrderBy(x => x.WarehouseName)
                .ThenBy(x => x.LocationName)
                .ToList();
        }

        return new InventoryScanPageDto
        {
            Query = new InventoryScanQueryDto { ProductScan = productToken, LocationScan = locationToken },
            Product = productResult,
            Location = locationResult,
            Balances = balanceRows
        };
    }

    private static InventoryScanResultDto ProductResult(string token, List<Product> matches)
    {
        var status = InventoryScanResolver.ResolveCount(token, matches.Count);
        var match = status == InventoryScanStatus.Resolved ? matches.Single() : null;
        return new InventoryScanResultDto
        {
            NormalizedValue = token,
            Status = status,
            EntityId = match?.Id,
            DisplayName = match?.Name ?? string.Empty,
            SecondaryText = match?.ProductCode
        };
    }

    private static InventoryScanResultDto LocationResult(
        string token,
        List<StorageLocation> matches,
        List<Warehouse> warehouses)
    {
        var status = InventoryScanResolver.ResolveCount(token, matches.Count);
        var match = status == InventoryScanStatus.Resolved ? matches.Single() : null;
        var warehouseMap = warehouses.ToDictionary(x => x.Id, x => x.Name);
        return new InventoryScanResultDto
        {
            NormalizedValue = token,
            Status = status,
            EntityId = match?.Id,
            DisplayName = match is null ? string.Empty : $"{match.Code} — {match.Name}",
            SecondaryText = match is null ? null : warehouseMap.GetValueOrDefault(match.WarehouseId, "Unknown Warehouse")
        };
    }
}
