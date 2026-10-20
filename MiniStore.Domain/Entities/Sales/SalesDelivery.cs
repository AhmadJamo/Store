namespace MiniStore.Domain.Entities;

public enum SalesDeliveryStatus { Draft = 1, Posted = 2, Cancelled = 3 }
public enum SalesDeliveryHistoryAction { Created = 1, Posted = 2, Cancelled = 3 }

public sealed class SalesDelivery
{
    private SalesDelivery() { DeliveryNumber = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string DeliveryNumber { get; private set; }
    public int SalesOrderId { get; private set; }
    public long InventoryReservationId { get; private set; }
    public int WarehouseId { get; private set; }
    public DateOnly DeliveryDate { get; private set; }
    public SalesDeliveryStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? PostedByUserId { get; private set; }
    public DateTime? PostedAtUtc { get; private set; }
    public string? CancelledByUserId { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? CancellationReason { get; private set; }
    public string? Notes { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<SalesDeliveryLine> Lines { get; private set; } = [];
    public List<SalesDeliveryHistory> History { get; private set; } = [];
    public decimal TotalCostOfGoodsSold => Lines.Sum(x => x.CostOfGoodsSold);

    public SalesDelivery(string deliveryNumber, int salesOrderId, long inventoryReservationId,
        int warehouseId, DateOnly deliveryDate, string? notes, string userId)
    {
        if (string.IsNullOrWhiteSpace(deliveryNumber) || deliveryNumber.Trim().Length > 50)
            throw new ArgumentException("Sales delivery number is required and cannot exceed 50 characters.");
        if (salesOrderId <= 0 || inventoryReservationId <= 0 || warehouseId <= 0)
            throw new ArgumentException("Sales order, reservation and warehouse are required.");
        if (notes?.Trim().Length > 1000) throw new ArgumentException("Delivery notes cannot exceed 1000 characters.");
        EnsureUser(userId);
        DeliveryNumber = deliveryNumber.Trim(); SalesOrderId = salesOrderId;
        InventoryReservationId = inventoryReservationId; WarehouseId = warehouseId;
        DeliveryDate = deliveryDate; Notes = Normalize(notes); Status = SalesDeliveryStatus.Draft;
        CreatedByUserId = userId.Trim(); CreatedAtUtc = DateTime.UtcNow;
        History.Add(new SalesDeliveryHistory(Status, Status, SalesDeliveryHistoryAction.Created, userId));
    }

    public void AddLine(SalesDeliveryLine line)
    {
        EnsureDraft();
        if (line.WarehouseId != WarehouseId) throw new InvalidOperationException("Every delivery line must use the delivery warehouse.");
        if (Lines.Any(x => x.InventoryReservationLineId == line.InventoryReservationLineId))
            throw new InvalidOperationException("Each reservation line can appear only once per delivery.");
        Lines.Add(line);
    }

    public void Post(string userId)
    {
        EnsureDraft(); EnsureUser(userId);
        if (Lines.Count == 0) throw new InvalidOperationException("A sales delivery must contain at least one line.");
        if (Lines.Any(x => !x.IsCostPosted)) throw new InvalidOperationException("Every delivery line must have a posted inventory cost.");
        var from = Status; Status = SalesDeliveryStatus.Posted; PostedByUserId = userId.Trim(); PostedAtUtc = DateTime.UtcNow;
        History.Add(new SalesDeliveryHistory(from, Status, SalesDeliveryHistoryAction.Posted, userId));
    }

    public void Cancel(string userId, string reason)
    {
        EnsureDraft(); EnsureUser(userId);
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 500)
            throw new ArgumentException("Cancellation reason is required and cannot exceed 500 characters.");
        var from = Status; Status = SalesDeliveryStatus.Cancelled; CancelledByUserId = userId.Trim();
        CancelledAtUtc = DateTime.UtcNow; CancellationReason = reason.Trim();
        History.Add(new SalesDeliveryHistory(from, Status, SalesDeliveryHistoryAction.Cancelled, userId, reason));
    }

    private void EnsureDraft()
    {
        if (Status != SalesDeliveryStatus.Draft) throw new InvalidOperationException("Only a draft sales delivery can be changed.");
    }
    private static void EnsureUser(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId) || userId.Trim().Length > 450) throw new ArgumentException("User is required.");
    }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class SalesDeliveryLine
{
    private SalesDeliveryLine() { ProductCodeSnapshot = ProductNameSnapshot = string.Empty; }
    public int Id { get; private set; }
    public int SalesDeliveryId { get; private set; }
    public int SalesOrderLineId { get; private set; }
    public long InventoryReservationLineId { get; private set; }
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public int? StorageLocationId { get; private set; }
    public decimal StockQuantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal CostOfGoodsSold { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string? TrackingAllocations { get; private set; }
    public bool IsCostPosted { get; private set; }

    public SalesDeliveryLine(int salesOrderLineId, long inventoryReservationLineId, int productId,
        int warehouseId, int? storageLocationId, decimal stockQuantity, string productCode,
        string productName, string? trackingAllocations)
    {
        if (salesOrderLineId <= 0 || inventoryReservationLineId <= 0 || productId <= 0 || warehouseId <= 0)
            throw new ArgumentException("Delivery line references are required.");
        if (storageLocationId <= 0) storageLocationId = null;
        if (stockQuantity <= 0) throw new ArgumentException("Delivery quantity must be greater than zero.");
        if (string.IsNullOrWhiteSpace(productCode) || string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Delivery product snapshots are required.");
        if (trackingAllocations?.Trim().Length > 4000) throw new ArgumentException("Tracking allocations cannot exceed 4000 characters.");
        SalesOrderLineId = salesOrderLineId; InventoryReservationLineId = inventoryReservationLineId;
        ProductId = productId; WarehouseId = warehouseId; StorageLocationId = storageLocationId;
        StockQuantity = stockQuantity; ProductCodeSnapshot = productCode.Trim(); ProductNameSnapshot = productName.Trim();
        TrackingAllocations = string.IsNullOrWhiteSpace(trackingAllocations) ? null : trackingAllocations.Trim();
    }

    public void RecordPostedCost(decimal unitCost, decimal totalCost)
    {
        if (IsCostPosted) throw new InvalidOperationException("Delivery line inventory cost has already been posted.");
        if (unitCost < 0 || totalCost < 0) throw new ArgumentException("Delivery inventory cost cannot be negative.");
        UnitCost = unitCost; CostOfGoodsSold = totalCost; IsCostPosted = true;
    }
}

public sealed class SalesDeliveryHistory
{
    private SalesDeliveryHistory() { UserId = string.Empty; }
    public int Id { get; private set; }
    public int SalesDeliveryId { get; private set; }
    public SalesDeliveryStatus FromStatus { get; private set; }
    public SalesDeliveryStatus ToStatus { get; private set; }
    public SalesDeliveryHistoryAction Action { get; private set; }
    public string UserId { get; private set; }
    public string? Reason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public SalesDeliveryHistory(SalesDeliveryStatus from, SalesDeliveryStatus to,
        SalesDeliveryHistoryAction action, string userId, string? reason = null)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        FromStatus = from; ToStatus = to; Action = action; UserId = userId.Trim();
        Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(); CreatedAtUtc = DateTime.UtcNow;
    }
}
