using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IInventoryReconciliationRepository
{
    Task<IReadOnlyList<InventoryReconciliationSnapshot>> GetSnapshotsAsync(
        int? warehouseId,
        string? search,
        CancellationToken cancellationToken = default);
}

public sealed record InventoryReconciliationSnapshot(
    int ProductId,
    string ProductName,
    string ProductCode,
    string? Barcode,
    int WarehouseId,
    string WarehouseName,
    InventoryControlMode ControlMode,
    bool HasWarehouseStock,
    decimal WarehouseQuantity,
    decimal AssignedLocationQuantity,
    bool HasNegativeLocationQuantity,
    int? LatestTransactionId,
    DateTime? LatestTransactionAt,
    decimal? LatestQuantityAfter,
    decimal? LatestInventoryValueAfter,
    decimal InventoryValue);
