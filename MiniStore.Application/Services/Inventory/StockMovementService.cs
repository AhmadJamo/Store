using MiniStore.Application.DTOs.StockMovements;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class StockMovementService(
    IStockMovementRepository movements,
    ILocationMovementRepository legacyMovements,
    IProductRepository products,
    IWarehouseRepository warehouses,
    IStorageLocationRepository locations)
{
    public async Task<bool> IsDuplicateAsync(string idempotencyKey, StockMovementType type,
        int productId, int warehouseId, int? fromLocationId, int toLocationId, decimal quantity)
    {
        var existing = await movements.GetByIdempotencyKeyAsync(NormalizeKey(idempotencyKey));
        if (existing is null) return false;
        if (existing.Type != type || existing.ProductId != productId || existing.WarehouseId != warehouseId ||
            existing.FromStorageLocationId != fromLocationId || existing.ToStorageLocationId != toLocationId ||
            existing.Quantity != quantity)
            throw new InvalidOperationException("The idempotency key was already used for a different stock movement.");
        return true;
    }

    public async Task RecordPostedAsync(LocationMovement legacyMovement, string idempotencyKey)
    {
        var key = NormalizeKey(idempotencyKey);
        if (await movements.GetByIdempotencyKeyAsync(key) is not null) return;
        await legacyMovements.AddAsync(legacyMovement);
        await legacyMovements.SaveChangesAsync();
        var type = legacyMovement.Type == LocationMovementType.Putaway
            ? StockMovementType.Putaway : StockMovementType.Relocation;
        await movements.AddAsync(new StockMovement(
            legacyMovement.Id, legacyMovement.ProductId, legacyMovement.WarehouseId,
            legacyMovement.FromStorageLocationId, legacyMovement.ToStorageLocationId,
            legacyMovement.Quantity, type, key, legacyMovement.CreatedByUserId,
            legacyMovement.Reference, legacyMovement.Notes));
    }

    public async Task<StockMovementPageDto> GetPageAsync()
    {
        var rows = await movements.GetAllAsync();
        var legacy = await legacyMovements.GetAllAsync();
        var legacyIds = legacy.Select(x => x.Id).ToHashSet();
        var productNames = (await products.GetAllAsync(null)).ToDictionary(x => x.Id, x => x.Name);
        var warehouseNames = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var locationCodes = (await locations.SearchAsync(null, null)).ToDictionary(x => x.Id, x => x.Code);
        return new StockMovementPageDto
        {
            LegacyMovementCount = legacy.Count,
            LinkedMovementCount = rows.Count,
            IsReconciled = rows.All(x => legacyIds.Contains(x.LocationMovementId)),
            Rows = rows.Select(x => new StockMovementRowDto
            {
                Id = x.Id, LocationMovementId = x.LocationMovementId,
                ProductName = productNames.GetValueOrDefault(x.ProductId, "Unknown Product"),
                WarehouseName = warehouseNames.GetValueOrDefault(x.WarehouseId, "Unknown Warehouse"),
                FromLocation = x.FromStorageLocationId.HasValue
                    ? locationCodes.GetValueOrDefault(x.FromStorageLocationId.Value, "Unknown Location") : "Unassigned",
                ToLocation = locationCodes.GetValueOrDefault(x.ToStorageLocationId, "Unknown Location"),
                Quantity = x.Quantity, Type = x.Type, Status = x.Status,
                Reference = x.Reference, PostedAt = x.PostedAt
            }).ToList()
        };
    }

    private static string NormalizeKey(string key)
    {
        var normalized = key?.Trim() ?? string.Empty;
        if (normalized.Length is < 16 or > 100)
            throw new ArgumentException("A valid idempotency key is required.");
        return normalized;
    }
}
