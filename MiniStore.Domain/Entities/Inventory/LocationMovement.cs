namespace MiniStore.Domain.Entities;

public class LocationMovement
{
    public long Id { get; private set; }

    public int ProductId { get; private set; }

    public int WarehouseId { get; private set; }

    public int? FromStorageLocationId { get; private set; }

    public int ToStorageLocationId { get; private set; }

    public decimal Quantity { get; private set; }

    public LocationMovementType Type { get; private set; }

    public string? Reference { get; private set; }

    public string? Notes { get; private set; }

    public string CreatedByUserId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private LocationMovement()
    {
        CreatedByUserId = string.Empty;
    }

    public LocationMovement(
        int productId,
        int warehouseId,
        int? fromStorageLocationId,
        int toStorageLocationId,
        decimal quantity,
        LocationMovementType type,
        string createdByUserId,
        string? reference = null,
        string? notes = null)
    {
        if (productId <= 0)
        {
            throw new ArgumentException("Product is required.");
        }

        if (warehouseId <= 0)
        {
            throw new ArgumentException("Warehouse is required.");
        }

        if (toStorageLocationId <= 0)
        {
            throw new ArgumentException("Destination location is required.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(createdByUserId))
        {
            throw new ArgumentException("Movement user is required.");
        }

        if (createdByUserId.Trim().Length > 450)
        {
            throw new ArgumentException("Movement user is too long.");
        }

        if (reference?.Trim().Length > 100)
        {
            throw new ArgumentException("Reference cannot exceed 100 characters.");
        }

        if (notes?.Trim().Length > 500)
        {
            throw new ArgumentException("Notes cannot exceed 500 characters.");
        }

        if (type == LocationMovementType.Putaway && fromStorageLocationId.HasValue)
        {
            throw new ArgumentException("Putaway must start from unassigned stock.");
        }

        if (type == LocationMovementType.Relocation && !fromStorageLocationId.HasValue)
        {
            throw new ArgumentException("Relocation requires a source location.");
        }

        if (fromStorageLocationId == toStorageLocationId)
        {
            throw new ArgumentException(
                "Source and destination locations must be different.");
        }

        ProductId = productId;
        WarehouseId = warehouseId;
        FromStorageLocationId = fromStorageLocationId;
        ToStorageLocationId = toStorageLocationId;
        Quantity = quantity;
        Type = type;
        CreatedByUserId = createdByUserId.Trim();
        Reference = Normalize(reference);
        Notes = Normalize(notes);
        CreatedAt = DateTime.UtcNow;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
