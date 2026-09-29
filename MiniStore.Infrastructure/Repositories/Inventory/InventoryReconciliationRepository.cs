using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class InventoryReconciliationRepository(AppDbContext context)
    : IInventoryReconciliationRepository
{
    public async Task<IReadOnlyList<InventoryReconciliationSnapshot>> GetSnapshotsAsync(
        int? warehouseId,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var normalizedSearch = search?.Trim();
        var warehouseStocks =
            from stock in context.ProductStocks.AsNoTracking()
            join product in context.Products.AsNoTracking()
                on stock.ProductId equals product.Id
            join warehouse in context.Warehouses.AsNoTracking()
                on stock.WarehouseId equals warehouse.Id
            where (!warehouseId.HasValue || stock.WarehouseId == warehouseId.Value) &&
                  (string.IsNullOrEmpty(normalizedSearch) ||
                   product.Name.Contains(normalizedSearch) ||
                   product.ProductCode.Contains(normalizedSearch) ||
                   (product.Barcode != null && product.Barcode.Contains(normalizedSearch)))
            select new InventoryReconciliationSnapshot(
                stock.ProductId,
                product.Name,
                product.ProductCode,
                product.Barcode,
                stock.WarehouseId,
                warehouse.Name,
                warehouse.ControlMode,
                true,
                stock.Quantity,
                context.ProductLocationStocks
                    .Where(locationStock =>
                        locationStock.ProductId == stock.ProductId &&
                        locationStock.WarehouseId == stock.WarehouseId)
                    .Sum(row => (decimal?)row.Quantity) ?? 0m,
                context.ProductLocationStocks.Any(locationStock =>
                    locationStock.ProductId == stock.ProductId &&
                    locationStock.WarehouseId == stock.WarehouseId &&
                    locationStock.Quantity < 0),
                context.StockTransactions
                    .Where(transaction =>
                        transaction.ProductId == stock.ProductId &&
                        transaction.WarehouseId == stock.WarehouseId)
                    .OrderByDescending(transaction => transaction.Id)
                    .Select(transaction => (int?)transaction.Id)
                    .FirstOrDefault(),
                context.StockTransactions
                    .Where(transaction =>
                        transaction.ProductId == stock.ProductId &&
                        transaction.WarehouseId == stock.WarehouseId)
                    .OrderByDescending(transaction => transaction.Id)
                    .Select(transaction => (DateTime?)transaction.CreatedAt)
                    .FirstOrDefault(),
                context.StockTransactions
                    .Where(transaction =>
                        transaction.ProductId == stock.ProductId &&
                        transaction.WarehouseId == stock.WarehouseId)
                    .OrderByDescending(transaction => transaction.Id)
                    .Select(transaction => (decimal?)transaction.QuantityAfter)
                    .FirstOrDefault(),
                context.StockTransactions
                    .Where(transaction =>
                        transaction.ProductId == stock.ProductId &&
                        transaction.WarehouseId == stock.WarehouseId)
                    .OrderByDescending(transaction => transaction.Id)
                    .Select(transaction => (decimal?)transaction.InventoryValueAfter)
                    .FirstOrDefault(),
                stock.InventoryValue);

        var rows = await warehouseStocks.ToListAsync(cancellationToken);

        var orphanLocationStocks = await (
            from locationStock in context.ProductLocationStocks.AsNoTracking()
            join product in context.Products.AsNoTracking()
                on locationStock.ProductId equals product.Id
            join warehouse in context.Warehouses.AsNoTracking()
                on locationStock.WarehouseId equals warehouse.Id
            where (!warehouseId.HasValue || locationStock.WarehouseId == warehouseId.Value) &&
                  (string.IsNullOrEmpty(normalizedSearch) ||
                   product.Name.Contains(normalizedSearch) ||
                   product.ProductCode.Contains(normalizedSearch) ||
                   (product.Barcode != null && product.Barcode.Contains(normalizedSearch))) &&
                  !context.ProductStocks.Any(stock =>
                      stock.ProductId == locationStock.ProductId &&
                      stock.WarehouseId == locationStock.WarehouseId)
            group locationStock by new
            {
                locationStock.ProductId,
                ProductName = product.Name,
                product.ProductCode,
                product.Barcode,
                locationStock.WarehouseId,
                WarehouseName = warehouse.Name,
                warehouse.ControlMode
            }
            into orphanGroup
            select new InventoryReconciliationSnapshot(
                orphanGroup.Key.ProductId,
                orphanGroup.Key.ProductName,
                orphanGroup.Key.ProductCode,
                orphanGroup.Key.Barcode,
                orphanGroup.Key.WarehouseId,
                orphanGroup.Key.WarehouseName,
                orphanGroup.Key.ControlMode,
                false,
                0m,
                orphanGroup.Sum(row => row.Quantity),
                orphanGroup.Any(row => row.Quantity < 0),
                null,
                null,
                null,
                null,
                0m))
            .ToListAsync(cancellationToken);

        rows.AddRange(orphanLocationStocks);
        return rows;
    }
}
