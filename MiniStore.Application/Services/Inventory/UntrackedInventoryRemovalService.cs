using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed record InventoryLocationRemovalCandidate(
    int? StorageLocationId,
    decimal Available,
    int Sequence);

public sealed record InventoryLocationRemovalAllocation(
    int? StorageLocationId,
    decimal Quantity);

public static class UntrackedInventoryRemovalAllocator
{
    public static IReadOnlyList<InventoryLocationRemovalAllocation> Plan(
        IEnumerable<InventoryLocationRemovalCandidate> candidates,
        decimal quantity,
        InventoryPickingStrategy strategy,
        bool allowUnassignedShortfall = false)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
        if (!Enum.IsDefined(strategy)) throw new ArgumentException("Invalid inventory picking strategy.");

        var usable = candidates.Where(x => x.Available > 0);
        var ordered = strategy == InventoryPickingStrategy.MinimizeLocations
            ? usable.OrderByDescending(x => x.Available)
                .ThenBy(x => x.StorageLocationId.HasValue ? 0 : 1)
                .ThenBy(x => x.Sequence)
            : usable.OrderBy(x => x.StorageLocationId.HasValue ? 0 : 1)
                .ThenBy(x => x.Sequence)
                .ThenBy(x => x.StorageLocationId ?? int.MaxValue);

        var remaining = quantity;
        var plan = new List<InventoryLocationRemovalAllocation>();
        foreach (var candidate in ordered)
        {
            if (remaining <= 0) break;
            var take = Math.Min(candidate.Available, remaining);
            plan.Add(new InventoryLocationRemovalAllocation(candidate.StorageLocationId, take));
            remaining -= take;
        }

        if (remaining > 0 && allowUnassignedShortfall)
        {
            plan.Add(new InventoryLocationRemovalAllocation(null, remaining));
            remaining = 0;
        }

        if (remaining > 0)
            throw new InvalidOperationException("Insufficient available stock in pickable inventory positions.");

        return plan;
    }
}

public sealed class UntrackedInventoryRemovalService(
    IWarehouseRepository warehouses,
    IStorageLocationRepository locations,
    IInventoryBalanceRepository balances,
    IProductStockRepository warehouseStocks,
    IProductLocationStockRepository locationStocks)
{
    public async Task RemoveAsync(
        Product product,
        int warehouseId,
        decimal quantity,
        bool allowUnassignedShortfall = false)
    {
        if (product.TrackingPolicy != ProductTrackingPolicy.None) return;

        var warehouse = await warehouses.GetByIdAsync(warehouseId)
            ?? throw new InvalidOperationException("Warehouse not found.");
        if (warehouse.ControlMode == InventoryControlMode.Simple) return;

        var warehouseLocations = await locations.GetWarehouseLocationsAsync(warehouseId);
        var pickableLocations = warehouseLocations
            .Where(x => x.Status == StorageLocationStatus.Active && x.IsPickable)
            .ToDictionary(x => x.Id);
        var reservedByLocation = (await balances.GetAllAsync())
            .Where(x => x.ProductId == product.Id && x.WarehouseId == warehouseId)
            .ToDictionary(x => x.StorageLocationId ?? 0, x => x.Reserved);
        var currentLocationStocks = await locationStocks.GetWarehouseProductAsync(product.Id, warehouseId);
        var warehouseStock = await warehouseStocks.GetByProductAndWarehouseAsync(product.Id, warehouseId)
            ?? throw new InvalidOperationException("Warehouse stock not found.");
        var positionBalances = currentLocationStocks
            .Where(x => pickableLocations.ContainsKey(x.StorageLocationId))
            .Select(x => new InventoryLocationRemovalCandidate(
                x.StorageLocationId,
                x.Quantity - reservedByLocation.GetValueOrDefault(x.StorageLocationId),
                pickableLocations[x.StorageLocationId].Sequence))
            .ToList();
        positionBalances.Add(new InventoryLocationRemovalCandidate(
            null,
            warehouseStock.Quantity - currentLocationStocks.Sum(x => x.Quantity) -
                reservedByLocation.GetValueOrDefault(0),
            int.MaxValue));

        var plan = UntrackedInventoryRemovalAllocator.Plan(
            positionBalances,
            quantity,
            warehouse.PickingStrategy,
            allowUnassignedShortfall);

        foreach (var allocation in plan.Where(x => x.StorageLocationId.HasValue))
        {
            var stock = currentLocationStocks.SingleOrDefault(
                x => x.StorageLocationId == allocation.StorageLocationId!.Value)
                ?? throw new InvalidOperationException("Location stock was not found.");
            stock.RemoveQuantity(allocation.Quantity);
        }
    }
}
