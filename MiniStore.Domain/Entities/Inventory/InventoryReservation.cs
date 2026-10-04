namespace MiniStore.Domain.Entities;

public enum InventoryReservationSourceType { StockTransfer = 1, SaleOrder = 2 }
public enum InventoryReservationStatus { Active = 1, Consumed = 2, Released = 3 }

public sealed class InventoryReservation
{
    private readonly List<InventoryReservationLine> _lines = [];
    private InventoryReservation() { SourceReference = CreatedByUserId = string.Empty; }

    public long Id { get; private set; }
    public InventoryReservationSourceType SourceType { get; private set; }
    public int SourceId { get; private set; }
    public string SourceReference { get; private set; }
    public InventoryReservationStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ClosedByUserId { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public string? ReleaseReason { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public IReadOnlyCollection<InventoryReservationLine> Lines => _lines;

    public InventoryReservation(InventoryReservationSourceType sourceType, int sourceId,
        string sourceReference, string createdByUserId)
    {
        if (!Enum.IsDefined(sourceType)) throw new ArgumentException("Reservation source type is invalid.");
        if (sourceId <= 0) throw new ArgumentException("Reservation source is required.");
        if (string.IsNullOrWhiteSpace(sourceReference) || sourceReference.Trim().Length > 100)
            throw new ArgumentException("Reservation source reference is required and cannot exceed 100 characters.");
        EnsureUser(createdByUserId);
        SourceType = sourceType; SourceId = sourceId; SourceReference = sourceReference.Trim();
        CreatedByUserId = createdByUserId.Trim(); Status = InventoryReservationStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddLine(int productId, int warehouseId, int? storageLocationId, decimal quantity)
    {
        if (Status != InventoryReservationStatus.Active) throw new InvalidOperationException("Only active reservations can be changed.");
        if (_lines.Any(x => x.ProductId == productId && x.WarehouseId == warehouseId && x.StorageLocationId == storageLocationId))
            throw new InvalidOperationException("A reservation cannot contain duplicate inventory dimensions.");
        _lines.Add(new InventoryReservationLine(productId, warehouseId, storageLocationId, quantity));
    }

    public void Consume(string userId)
    {
        EnsureActiveWithLines(); EnsureUser(userId);
        Status = InventoryReservationStatus.Consumed; ClosedByUserId = userId.Trim(); ClosedAt = DateTime.UtcNow;
    }

    public void Release(string userId, string reason)
    {
        EnsureActiveWithLines(); EnsureUser(userId);
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 250)
            throw new ArgumentException("Release reason is required and cannot exceed 250 characters.");
        Status = InventoryReservationStatus.Released; ClosedByUserId = userId.Trim();
        ClosedAt = DateTime.UtcNow; ReleaseReason = reason.Trim();
    }

    private void EnsureActiveWithLines()
    {
        if (Status != InventoryReservationStatus.Active) throw new InvalidOperationException("Only active reservations can be closed.");
        if (_lines.Count == 0) throw new InvalidOperationException("Reservation must contain at least one line.");
    }
    private static void EnsureUser(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > 450) throw new ArgumentException("Reservation user is required.");
    }
}

public sealed class InventoryReservationLine
{
    private InventoryReservationLine() { }
    public long Id { get; private set; }
    public long InventoryReservationId { get; private set; }
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public int? StorageLocationId { get; private set; }
    public decimal Quantity { get; private set; }

    internal InventoryReservationLine(int productId, int warehouseId, int? storageLocationId, decimal quantity)
    {
        if (productId <= 0 || warehouseId <= 0) throw new ArgumentException("Product and warehouse are required.");
        if (storageLocationId <= 0) storageLocationId = null;
        if (quantity <= 0) throw new ArgumentException("Reservation quantity must be greater than zero.");
        ProductId = productId; WarehouseId = warehouseId; StorageLocationId = storageLocationId; Quantity = quantity;
    }
}
