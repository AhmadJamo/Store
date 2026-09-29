using MiniStore.Application.DTOs.Inventory.Reconciliation;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class InventoryReconciliationService(
    IInventoryReconciliationRepository repository,
    IWarehouseRepository warehouseRepository)
{
    private const decimal QuantityTolerance = 0.000001m;
    private const decimal ValueTolerance = 0.00000001m;

    public async Task<InventoryReconciliationPageDto> SearchAsync(
        InventoryReconciliationQueryDto query,
        CancellationToken cancellationToken = default)
    {
        var pageSize = Math.Clamp(query.PageSize, 10, 200);
        var page = Math.Max(1, query.Page);
        var snapshots = await repository.GetSnapshotsAsync(
            query.WarehouseId,
            query.Search,
            cancellationToken);
        var allRows = snapshots.Select(Map).ToList();
        var exceptionCount = allRows.Count(row => !row.IsConsistent);
        var filteredRows = query.ExceptionsOnly
            ? allRows.Where(row => !row.IsConsistent).ToList()
            : allRows;
        var totalCount = filteredRows.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        page = Math.Min(page, totalPages);

        var warehouses = (await warehouseRepository.GetAllAsync())
            .OrderBy(warehouse => warehouse.Name)
            .Select(warehouse => new InventoryReconciliationWarehouseDto(
                warehouse.Id,
                warehouse.Name))
            .ToList();

        return new InventoryReconciliationPageDto
        {
            Items = filteredRows
                .OrderByDescending(row => !row.IsConsistent)
                .ThenBy(row => row.WarehouseName)
                .ThenBy(row => row.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList(),
            Warehouses = warehouses,
            WarehouseId = query.WarehouseId,
            Search = query.Search?.Trim() ?? string.Empty,
            ExceptionsOnly = query.ExceptionsOnly,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            ExceptionCount = exceptionCount
        };
    }

    private static InventoryReconciliationRowDto Map(
        InventoryReconciliationSnapshot snapshot)
    {
        var issues = new List<InventoryReconciliationIssue>();
        if (!snapshot.HasWarehouseStock)
            issues.Add(InventoryReconciliationIssue.MissingWarehouseStock);
        if (snapshot.AssignedLocationQuantity - snapshot.WarehouseQuantity > QuantityTolerance)
            issues.Add(InventoryReconciliationIssue.OverAllocated);
        if (snapshot.HasNegativeLocationQuantity)
            issues.Add(InventoryReconciliationIssue.NegativeLocationQuantity);
        if (snapshot.ControlMode == InventoryControlMode.Simple &&
            Math.Abs(snapshot.AssignedLocationQuantity) > QuantityTolerance)
        {
            issues.Add(InventoryReconciliationIssue.SimpleWarehouseHasLocationStock);
        }

        if (snapshot.HasWarehouseStock && !snapshot.LatestTransactionId.HasValue)
        {
            issues.Add(InventoryReconciliationIssue.MissingTransaction);
        }
        else if (snapshot.HasWarehouseStock)
        {
            if (!snapshot.LatestQuantityAfter.HasValue ||
                Math.Abs(snapshot.LatestQuantityAfter.Value - snapshot.WarehouseQuantity) > QuantityTolerance)
            {
                issues.Add(InventoryReconciliationIssue.QuantitySnapshotMismatch);
            }

            if (!snapshot.LatestInventoryValueAfter.HasValue ||
                Math.Abs(snapshot.LatestInventoryValueAfter.Value - snapshot.InventoryValue) > ValueTolerance)
            {
                issues.Add(InventoryReconciliationIssue.ValueSnapshotMismatch);
            }
        }

        return new InventoryReconciliationRowDto
        {
            ProductId = snapshot.ProductId,
            ProductName = snapshot.ProductName,
            ProductCode = snapshot.ProductCode,
            Barcode = snapshot.Barcode ?? string.Empty,
            WarehouseId = snapshot.WarehouseId,
            WarehouseName = snapshot.WarehouseName,
            WarehouseQuantity = snapshot.WarehouseQuantity,
            AssignedLocationQuantity = snapshot.AssignedLocationQuantity,
            InventoryValue = snapshot.InventoryValue,
            LatestTransactionId = snapshot.LatestTransactionId,
            LatestTransactionAt = snapshot.LatestTransactionAt,
            LatestQuantityAfter = snapshot.LatestQuantityAfter,
            LatestInventoryValueAfter = snapshot.LatestInventoryValueAfter,
            Issues = issues
        };
    }
}
