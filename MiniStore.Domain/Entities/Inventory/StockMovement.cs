namespace MiniStore.Domain.Entities;

public enum StockMovementType { Putaway = 1, Relocation = 2, TransferOutbound = 3, TransferTransit = 4, TransferInbound = 5, AdjustmentIn = 6, AdjustmentOut = 7, ReceiptIn = 8, ReceiptReturnOut = 9 }
public enum StockMovementStatus { Planned = 0, Posted = 1, Reversed = 2 }

public sealed class StockMovement
{
    private StockMovement() { IdempotencyKey = CreatedByUserId = string.Empty; }
    public long Id { get; private set; }
    public long? LocationMovementId { get; private set; }
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public int? FromStorageLocationId { get; private set; }
    public int? ToStorageLocationId { get; private set; }
    public int? RelatedWarehouseId { get; private set; }
    public decimal Quantity { get; private set; }
    public StockMovementType Type { get; private set; }
    public StockMovementStatus Status { get; private set; }
    public string IdempotencyKey { get; private set; }
    public string CreatedByUserId { get; private set; }
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }
    public string? SourceDocumentType { get; private set; }
    public int? SourceDocumentId { get; private set; }
    public int? SourceLineId { get; private set; }
    public int? StageSequence { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PostedAt { get; private set; }
    public string? PostedByUserId { get; private set; }
    public DateTime? ReversedAt { get; private set; }
    public string? ReversedByUserId { get; private set; }

    public StockMovement(long locationMovementId, int productId, int warehouseId,
        int? fromStorageLocationId, int toStorageLocationId, decimal quantity,
        StockMovementType type, string idempotencyKey, string createdByUserId,
        string? reference = null, string? notes = null)
    {
        if (locationMovementId <= 0) throw new ArgumentException("Location movement is required.");
        if (productId <= 0) throw new ArgumentException("Product is required.");
        if (warehouseId <= 0) throw new ArgumentException("Warehouse is required.");
        if (toStorageLocationId <= 0) throw new ArgumentException("Destination location is required.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
        if (!Enum.IsDefined(type)) throw new ArgumentException("Stock movement type is invalid.");
        if (type == StockMovementType.Putaway && fromStorageLocationId.HasValue)
            throw new ArgumentException("Putaway must start from unassigned stock.");
        if (type == StockMovementType.Relocation && !fromStorageLocationId.HasValue)
            throw new ArgumentException("Relocation requires a source location.");
        if (fromStorageLocationId == toStorageLocationId)
            throw new ArgumentException("Source and destination locations must be different.");
        var normalizedKey = idempotencyKey?.Trim() ?? string.Empty;
        if (normalizedKey.Length is < 16 or > 100)
            throw new ArgumentException("A valid idempotency key is required.");
        if (string.IsNullOrWhiteSpace(createdByUserId) || createdByUserId.Trim().Length > 450)
            throw new ArgumentException("Movement user is required.");
        if (reference?.Trim().Length > 100) throw new ArgumentException("Reference cannot exceed 100 characters.");
        if (notes?.Trim().Length > 500) throw new ArgumentException("Notes cannot exceed 500 characters.");

        LocationMovementId = locationMovementId;
        ProductId = productId;
        WarehouseId = warehouseId;
        FromStorageLocationId = fromStorageLocationId;
        ToStorageLocationId = toStorageLocationId;
        Quantity = quantity;
        Type = type;
        Status = StockMovementStatus.Posted;
        IdempotencyKey = normalizedKey;
        CreatedByUserId = createdByUserId.Trim();
        Reference = Normalize(reference);
        Notes = Normalize(notes);
        CreatedAt = DateTime.UtcNow;
        PostedAt = CreatedAt;
        PostedByUserId = CreatedByUserId;
    }

    public static StockMovement PlanTransfer(int stockTransferId, int sourceLineId, int productId,
        int warehouseId, int relatedWarehouseId, int? fromLocationId, int? toLocationId,
        decimal quantity, StockMovementType type, int stageSequence, string idempotencyKey, string userId,
        string reference)
    {
        if (stockTransferId <= 0 || sourceLineId <= 0) throw new ArgumentException("Transfer and line are required.");
        if (productId <= 0) throw new ArgumentException("Product is required.");
        if (type is not (StockMovementType.TransferOutbound or StockMovementType.TransferTransit or StockMovementType.TransferInbound))
            throw new ArgumentException("Transfer movement type is invalid.");
        if (warehouseId <= 0 || relatedWarehouseId <= 0 || warehouseId == relatedWarehouseId)
            throw new ArgumentException("Transfer warehouses are invalid.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
        if ((type == StockMovementType.TransferOutbound && (toLocationId.HasValue || stageSequence != 1)) ||
            (type == StockMovementType.TransferTransit && (fromLocationId.HasValue || toLocationId.HasValue || stageSequence != 2)) ||
            (type == StockMovementType.TransferInbound && (fromLocationId.HasValue || stageSequence != 3)))
            throw new ArgumentException("Transfer movement stage locations are invalid.");
        var key = idempotencyKey?.Trim() ?? string.Empty;
        if (key.Length is < 16 or > 100) throw new ArgumentException("A valid idempotency key is required.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Movement user is required.");
        if (reference?.Trim().Length > 100) throw new ArgumentException("Reference cannot exceed 100 characters.");
        var movement = new StockMovement
        {
            ProductId = productId, WarehouseId = warehouseId, RelatedWarehouseId = relatedWarehouseId,
            FromStorageLocationId = fromLocationId, ToStorageLocationId = toLocationId,
            Quantity = quantity, Type = type, Status = StockMovementStatus.Planned,
            IdempotencyKey = key, CreatedByUserId = userId.Trim(), Reference = Normalize(reference),
            SourceDocumentType = "StockTransfer", SourceDocumentId = stockTransferId,
            SourceLineId = sourceLineId, StageSequence = stageSequence, CreatedAt = DateTime.UtcNow
        };
        return movement;
    }

    public static StockMovement PostAdjustment(long adjustmentId, long sourceLineId, int productId,
        int warehouseId, int? locationId, decimal variance, string userId, string reference)
    {
        if (adjustmentId <= 0 || sourceLineId <= 0) throw new ArgumentException("Adjustment and line are required.");
        if (productId <= 0 || warehouseId <= 0) throw new ArgumentException("Product and warehouse are required.");
        if (variance == 0) throw new ArgumentException("Adjustment variance cannot be zero.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Movement user is required.");
        return new StockMovement
        {
            ProductId = productId, WarehouseId = warehouseId,
            FromStorageLocationId = variance < 0 ? locationId : null,
            ToStorageLocationId = variance > 0 ? locationId : null,
            Quantity = Math.Abs(variance),
            Type = variance > 0 ? StockMovementType.AdjustmentIn : StockMovementType.AdjustmentOut,
            Status = StockMovementStatus.Posted,
            IdempotencyKey = $"ADJUSTMENT:{adjustmentId}:{sourceLineId}",
            CreatedByUserId = userId.Trim(), PostedByUserId = userId.Trim(),
            Reference = Normalize(reference), SourceDocumentType = "InventoryAdjustment",
            SourceDocumentId = checked((int)adjustmentId), SourceLineId = checked((int)sourceLineId),
            StageSequence = 1, CreatedAt = DateTime.UtcNow, PostedAt = DateTime.UtcNow
        };
    }

    public static StockMovement PostGoodsReceipt(int goodsReceiptId, int sourceLineId, int productId, int warehouseId,
        decimal quantity, string userId, string reference)
    {
        if (goodsReceiptId <= 0 || sourceLineId <= 0) throw new ArgumentException("Goods receipt and line are required.");
        if (productId <= 0 || warehouseId <= 0) throw new ArgumentException("Product and warehouse are required.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(reference)) throw new ArgumentException("Receipt user and reference are required.");
        return new StockMovement
        {
            ProductId = productId, WarehouseId = warehouseId, Quantity = quantity, Type = StockMovementType.ReceiptIn,
            Status = StockMovementStatus.Posted, IdempotencyKey = $"GRN:{goodsReceiptId}:{sourceLineId}", CreatedByUserId = userId.Trim(),
            PostedByUserId = userId.Trim(), Reference = Normalize(reference), SourceDocumentType = "GoodsReceipt",
            SourceDocumentId = goodsReceiptId, SourceLineId = sourceLineId, StageSequence = 1, CreatedAt = DateTime.UtcNow, PostedAt = DateTime.UtcNow
        };
    }

    public static StockMovement PostGoodsReceiptReturn(int returnId, int sourceLineId, int productId, int warehouseId,
        decimal quantity, string userId, string reference)
    {
        if (returnId <= 0 || sourceLineId <= 0) throw new ArgumentException("Goods receipt return and line are required.");
        if (productId <= 0 || warehouseId <= 0 || quantity <= 0) throw new ArgumentException("Return product, warehouse and quantity are required.");
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(reference)) throw new ArgumentException("Return user and reference are required.");
        return new StockMovement
        {
            ProductId = productId, WarehouseId = warehouseId, Quantity = quantity, Type = StockMovementType.ReceiptReturnOut,
            Status = StockMovementStatus.Posted, IdempotencyKey = $"GRR:{returnId}:{sourceLineId}", CreatedByUserId = userId.Trim(),
            PostedByUserId = userId.Trim(), Reference = Normalize(reference), SourceDocumentType = "GoodsReceiptReturn",
            SourceDocumentId = returnId, SourceLineId = sourceLineId, StageSequence = 1, CreatedAt = DateTime.UtcNow, PostedAt = DateTime.UtcNow
        };
    }

    public void Post(string userId)
    {
        if (Status != StockMovementStatus.Planned) throw new InvalidOperationException("Only planned movements can be posted.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Movement user is required.");
        Status = StockMovementStatus.Posted; PostedAt = DateTime.UtcNow; PostedByUserId = userId.Trim();
    }

    public void Reverse(string userId)
    {
        if (Status != StockMovementStatus.Posted) throw new InvalidOperationException("Only posted movements can be reversed.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Movement user is required.");
        Status = StockMovementStatus.Reversed; ReversedAt = DateTime.UtcNow; ReversedByUserId = userId.Trim();
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
