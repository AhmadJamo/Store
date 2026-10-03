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

    public async Task PlanTransferAsync(StockTransfer transfer, string userId)
    {
        if ((await movements.GetBySourceAsync("StockTransfer", transfer.Id)).Count > 0) return;
        foreach (var movement in CreateTransferMovements(transfer, userId))
            await movements.AddAsync(movement);
    }

    private static List<StockMovement> CreateTransferMovements(StockTransfer transfer, string userId)
    {
        var result = new List<StockMovement>();
        foreach (var item in transfer.Items)
        {
            var prefix = $"TRANSFER:{transfer.Id}:{item.Id}";
            result.Add(StockMovement.PlanTransfer(transfer.Id, item.Id, item.ProductId,
                transfer.FromWarehouseId, transfer.ToWarehouseId, item.SourceLocationId, null,
                item.Quantity, StockMovementType.TransferOutbound, 1, $"{prefix}:OUTBOUND", userId, transfer.TransferNumber));
            result.Add(StockMovement.PlanTransfer(transfer.Id, item.Id, item.ProductId,
                transfer.FromWarehouseId, transfer.ToWarehouseId, null, null,
                item.Quantity, StockMovementType.TransferTransit, 2, $"{prefix}:TRANSIT", userId, transfer.TransferNumber));
            result.Add(StockMovement.PlanTransfer(transfer.Id, item.Id, item.ProductId,
                transfer.ToWarehouseId, transfer.FromWarehouseId, null, item.DestinationLocationId,
                item.Quantity, StockMovementType.TransferInbound, 3, $"{prefix}:INBOUND", userId, transfer.TransferNumber));
        }
        return result;
    }

    public async Task PostTransferAsync(StockTransfer transfer, string userId)
    {
        var rows = await movements.GetBySourceAsync("StockTransfer", transfer.Id);
        if (rows.Count == 0)
        {
            rows = CreateTransferMovements(transfer, userId);
            foreach (var movement in rows) await movements.AddAsync(movement);
        }
        foreach (var movement in rows) movement.Post(userId);
    }

    public async Task ReverseTransferAsync(int transferId, string userId)
    {
        var rows = await movements.GetBySourceAsync("StockTransfer", transferId);
        if (rows.Count == 0) return;
        foreach (var movement in rows) movement.Reverse(userId);
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
            LinkedMovementCount = rows.Count(x => x.LocationMovementId.HasValue),
            TotalMovementCount = rows.Count,
            IsReconciled = rows.Where(x => x.LocationMovementId.HasValue)
                .All(x => legacyIds.Contains(x.LocationMovementId!.Value)),
            Rows = rows.Select(x => new StockMovementRowDto
            {
                Id = x.Id, LocationMovementId = x.LocationMovementId,
                ProductName = productNames.GetValueOrDefault(x.ProductId, "Unknown Product"),
                WarehouseName = warehouseNames.GetValueOrDefault(x.WarehouseId, "Unknown Warehouse"),
                RelatedWarehouseName = x.RelatedWarehouseId.HasValue
                    ? warehouseNames.GetValueOrDefault(x.RelatedWarehouseId.Value, "Unknown Warehouse") : null,
                FromLocation = x.FromStorageLocationId.HasValue
                    ? locationCodes.GetValueOrDefault(x.FromStorageLocationId.Value, "Unknown Location") : "Unassigned",
                ToLocation = x.ToStorageLocationId.HasValue
                    ? locationCodes.GetValueOrDefault(x.ToStorageLocationId.Value, "Unknown Location") : "—",
                Quantity = x.Quantity, Type = x.Type, Status = x.Status,
                Reference = x.Reference, CreatedAt = x.CreatedAt, PostedAt = x.PostedAt,
                SourceDocumentType = x.SourceDocumentType, SourceDocumentId = x.SourceDocumentId,
                StageSequence = x.StageSequence
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
