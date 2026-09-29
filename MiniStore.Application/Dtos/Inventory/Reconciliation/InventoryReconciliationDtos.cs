namespace MiniStore.Application.DTOs.Inventory.Reconciliation;

public sealed class InventoryReconciliationQueryDto
{
    public int? WarehouseId { get; init; }
    public string? Search { get; init; }
    public bool ExceptionsOnly { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public sealed class InventoryReconciliationPageDto
{
    public IReadOnlyList<InventoryReconciliationRowDto> Items { get; init; } = [];
    public IReadOnlyList<InventoryReconciliationWarehouseDto> Warehouses { get; init; } = [];
    public int? WarehouseId { get; init; }
    public string Search { get; init; } = string.Empty;
    public bool ExceptionsOnly { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int ExceptionCount { get; init; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
}

public sealed record InventoryReconciliationWarehouseDto(int Id, string Name);

public sealed class InventoryReconciliationRowDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductCode { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public decimal WarehouseQuantity { get; init; }
    public decimal AssignedLocationQuantity { get; init; }
    public decimal UnassignedQuantity => WarehouseQuantity - AssignedLocationQuantity;
    public decimal InventoryValue { get; init; }
    public int? LatestTransactionId { get; init; }
    public DateTime? LatestTransactionAt { get; init; }
    public decimal? LatestQuantityAfter { get; init; }
    public decimal? LatestInventoryValueAfter { get; init; }
    public IReadOnlyList<InventoryReconciliationIssue> Issues { get; init; } = [];
    public bool IsConsistent => Issues.Count == 0;
}

public enum InventoryReconciliationIssue
{
    MissingWarehouseStock,
    OverAllocated,
    NegativeLocationQuantity,
    SimpleWarehouseHasLocationStock,
    MissingTransaction,
    QuantitySnapshotMismatch,
    ValueSnapshotMismatch
}
