using MiniStore.Domain.Entities;

namespace MiniStore.Application.Services;

public static class InventoryRemovalAllocator
{
    public static IReadOnlyList<InventoryTrackingBalance> Order(
        IEnumerable<InventoryTrackingBalance> candidates,
        InventoryPickingStrategy strategy,
        IReadOnlyDictionary<int, int>? locationSequences = null)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        if (!Enum.IsDefined(strategy))
            throw new ArgumentException("Invalid inventory picking strategy.", nameof(strategy));

        locationSequences ??= new Dictionary<int, int>();
        int LocationSequence(InventoryTrackingBalance balance) =>
            balance.StorageLocationId.HasValue &&
            locationSequences.TryGetValue(balance.StorageLocationId.Value, out var sequence)
                ? sequence
                : int.MaxValue;

        IOrderedEnumerable<InventoryTrackingBalance> ordered = strategy switch
        {
            InventoryPickingStrategy.Fefo => candidates
                .OrderBy(x => x.ExpirationDate.HasValue ? 0 : 1)
                .ThenBy(x => x.ExpirationDate),
            InventoryPickingStrategy.LocationPriority => candidates
                .OrderBy(LocationSequence)
                .ThenBy(x => x.StorageLocationId ?? int.MaxValue),
            InventoryPickingStrategy.MinimizeLocations => candidates
                .OrderByDescending(x => x.Quantity)
                .ThenBy(LocationSequence),
            _ => candidates.OrderBy(x => x.ReceivedAt)
        };

        return ordered
            .ThenBy(x => x.ReceivedAt)
            .ThenBy(x => x.Identifier, StringComparer.Ordinal)
            .ToList();
    }
}
