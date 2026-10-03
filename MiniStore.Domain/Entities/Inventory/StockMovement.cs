namespace MiniStore.Domain.Entities;

public enum StockMovementType { Putaway = 1, Relocation = 2 }
public enum StockMovementStatus { Posted = 1, Reversed = 2 }

public sealed class StockMovement
{
    private StockMovement() { IdempotencyKey = CreatedByUserId = string.Empty; }
    public long Id { get; private set; }
    public long LocationMovementId { get; private set; }
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public int? FromStorageLocationId { get; private set; }
    public int ToStorageLocationId { get; private set; }
    public decimal Quantity { get; private set; }
    public StockMovementType Type { get; private set; }
    public StockMovementStatus Status { get; private set; }
    public string IdempotencyKey { get; private set; }
    public string CreatedByUserId { get; private set; }
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }
    public DateTime PostedAt { get; private set; }

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
        PostedAt = DateTime.UtcNow;
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
