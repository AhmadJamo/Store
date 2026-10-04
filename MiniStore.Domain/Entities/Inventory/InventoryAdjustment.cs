namespace MiniStore.Domain.Entities;

public enum InventoryAdjustmentStatus { Draft = 1, Counted = 2, Approved = 3, Posted = 4, Cancelled = 5 }

public sealed class InventoryAdjustment
{
    private readonly List<InventoryAdjustmentLine> _lines = [];
    private InventoryAdjustment() { AdjustmentNumber = Reason = CreatedByUserId = string.Empty; }
    public long Id { get; private set; }
    public string AdjustmentNumber { get; private set; }
    public int WarehouseId { get; private set; }
    public int? StorageLocationId { get; private set; }
    public bool IsBlindCount { get; private set; }
    public string Reason { get; private set; }
    public InventoryAdjustmentStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? CountedByUserId { get; private set; }
    public DateTime? CountedAt { get; private set; }
    public string? ApprovedByUserId { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? PostedByUserId { get; private set; }
    public DateTime? PostedAt { get; private set; }
    public string? CancelledByUserId { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public IReadOnlyCollection<InventoryAdjustmentLine> Lines => _lines;

    public InventoryAdjustment(string number, int warehouseId, int? locationId, bool blind, string reason, string userId)
    {
        if (string.IsNullOrWhiteSpace(number) || number.Trim().Length > 50) throw new ArgumentException("Adjustment number is required.");
        if (warehouseId <= 0) throw new ArgumentException("Warehouse is required.");
        if (locationId <= 0) locationId = null;
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 250) throw new ArgumentException("Adjustment reason is required.");
        EnsureUser(userId);
        AdjustmentNumber = number.Trim(); WarehouseId = warehouseId; StorageLocationId = locationId;
        IsBlindCount = blind; Reason = reason.Trim(); CreatedByUserId = userId.Trim();
        Status = InventoryAdjustmentStatus.Draft; CreatedAt = DateTime.UtcNow;
    }

    public void AddLine(int productId, decimal expectedQuantity)
    {
        EnsureDraft();
        if (productId <= 0) throw new ArgumentException("Product is required.");
        if (_lines.Any(x => x.ProductId == productId)) throw new InvalidOperationException("Product is already included in this count.");
        _lines.Add(new InventoryAdjustmentLine(productId, expectedQuantity));
    }

    public void RecordCounts(IEnumerable<(int ProductId, decimal CountedQuantity)> counts, string userId)
    {
        EnsureDraft(); EnsureUser(userId);
        var values = counts.ToDictionary(x => x.ProductId, x => x.CountedQuantity);
        if (_lines.Count == 0 || values.Count != _lines.Count) throw new InvalidOperationException("A counted quantity is required for every line.");
        foreach (var line in _lines)
        {
            if (!values.TryGetValue(line.ProductId, out var quantity)) throw new InvalidOperationException("A counted quantity is required for every line.");
            line.RecordCount(quantity);
        }
        Status = InventoryAdjustmentStatus.Counted; CountedByUserId = userId.Trim(); CountedAt = DateTime.UtcNow;
    }

    public void Approve(string userId)
    {
        EnsureUser(userId);
        if (Status != InventoryAdjustmentStatus.Counted) throw new InvalidOperationException("Only a completed count can be approved.");
        if (CreatedByUserId == userId || CountedByUserId == userId) throw new InvalidOperationException("The counter cannot approve their own inventory adjustment.");
        Status = InventoryAdjustmentStatus.Approved; ApprovedByUserId = userId.Trim(); ApprovedAt = DateTime.UtcNow;
    }
    public void Post(string userId)
    {
        EnsureUser(userId);
        if (Status != InventoryAdjustmentStatus.Approved) throw new InvalidOperationException("Only an approved inventory adjustment can be posted.");
        Status = InventoryAdjustmentStatus.Posted; PostedByUserId = userId.Trim(); PostedAt = DateTime.UtcNow;
    }
    public void Cancel(string userId)
    {
        EnsureUser(userId);
        if (Status is InventoryAdjustmentStatus.Posted or InventoryAdjustmentStatus.Cancelled) throw new InvalidOperationException("Posted or cancelled adjustments cannot be cancelled.");
        Status = InventoryAdjustmentStatus.Cancelled; CancelledByUserId = userId.Trim(); CancelledAt = DateTime.UtcNow;
    }
    private void EnsureDraft() { if (Status != InventoryAdjustmentStatus.Draft) throw new InvalidOperationException("Only draft adjustments can be edited."); }
    private static void EnsureUser(string value) { if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > 450) throw new ArgumentException("User is required."); }
}

public sealed class InventoryAdjustmentLine
{
    private InventoryAdjustmentLine() { }
    public long Id { get; private set; }
    public long InventoryAdjustmentId { get; private set; }
    public int ProductId { get; private set; }
    public decimal ExpectedQuantity { get; private set; }
    public decimal? CountedQuantity { get; private set; }
    public decimal VarianceQuantity => (CountedQuantity ?? ExpectedQuantity) - ExpectedQuantity;
    internal InventoryAdjustmentLine(int productId, decimal expected) { ProductId = productId; ExpectedQuantity = expected; }
    internal void RecordCount(decimal quantity)
    {
        if (quantity < 0) throw new ArgumentException("Counted quantity cannot be negative.");
        CountedQuantity = Math.Round(quantity, 6, MidpointRounding.AwayFromZero);
    }
}
