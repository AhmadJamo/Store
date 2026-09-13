using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class UnassignedStockService(
    IProductStockRepository productStockRepository,
    IProductLocationStockRepository locationStockRepository,
    IProductRepository productRepository,
    IWarehouseRepository warehouseRepository,
    IStorageLocationRepository storageLocationRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<List<UnassignedStockDto>> SearchAsync(
        int? warehouseId,
        string? query,
        string? sort)
    {
        var products = await productRepository.GetAllAsync(query);
        var warehouses = await warehouseRepository.GetAllAsync();
        var warehouseStocks = await productStockRepository.GetAllAsync();
        var locationStocks = await locationStockRepository.GetAllAsync();

        var productsById = products.ToDictionary(product => product.Id);
        var warehouseNamesById = warehouses.ToDictionary(
            warehouse => warehouse.Id,
            warehouse => warehouse.Name);
        var assignedQuantities = locationStocks
            .GroupBy(stock => (stock.ProductId, stock.WarehouseId))
            .ToDictionary(group => group.Key, group => group.Sum(stock => stock.Quantity));

        var rows = warehouseStocks
            .Where(stock =>
                productsById.ContainsKey(stock.ProductId) &&
                (!warehouseId.HasValue || stock.WarehouseId == warehouseId))
            .Select(stock => CreateRow(
                stock,
                productsById[stock.ProductId],
                warehouseNamesById.GetValueOrDefault(stock.WarehouseId, "Unknown"),
                assignedQuantities.GetValueOrDefault(
                    (stock.ProductId, stock.WarehouseId))))
            .Where(row => row.UnassignedQuantity > 0)
            .ToList();

        return SortRows(rows, sort);
    }

    public async Task AssignAsync(
        int productId,
        int warehouseId,
        int storageLocationId,
        decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        var location = await storageLocationRepository.GetByIdAsync(storageLocationId)
            ?? throw new InvalidOperationException("Storage location not found.");

        if (location.WarehouseId != warehouseId ||
            location.Status != StorageLocationStatus.Active)
        {
            throw new InvalidOperationException(
                "Select an active location in the same warehouse.");
        }

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var warehouseStock = await productStockRepository
                .GetByProductAndWarehouseAsync(productId, warehouseId)
                ?? throw new InvalidOperationException("Warehouse stock not found.");

            var existingLocationStocks = await locationStockRepository.GetAllAsync();
            var assignedQuantity = existingLocationStocks
                .Where(stock =>
                    stock.ProductId == productId &&
                    stock.WarehouseId == warehouseId)
                .Sum(stock => stock.Quantity);
            var unassignedQuantity = warehouseStock.Quantity - assignedQuantity;

            if (quantity > unassignedQuantity)
            {
                throw new InvalidOperationException(
                    "Quantity exceeds the unassigned balance.");
            }

            var targetStock = await locationStockRepository.GetAsync(
                productId,
                storageLocationId);
            var targetQuantityAfterAssignment =
                (targetStock?.Quantity ?? 0) + quantity;

            if (location.MaximumQuantity.HasValue &&
                targetQuantityAfterAssignment > location.MaximumQuantity.Value)
            {
                throw new InvalidOperationException(
                    "Quantity exceeds the location capacity.");
            }

            if (targetStock is null)
            {
                targetStock = new ProductLocationStock(
                    productId,
                    warehouseId,
                    storageLocationId);

                await locationStockRepository.AddAsync(targetStock);
            }

            targetStock.AddQuantity(quantity);
        });
    }

    private static UnassignedStockDto CreateRow(
        ProductStock warehouseStock,
        Product product,
        string warehouseName,
        decimal assignedQuantity)
    {
        return new UnassignedStockDto
        {
            ProductId = warehouseStock.ProductId,
            ProductName = product.Name,
            Barcode = product.Barcode,
            WarehouseId = warehouseStock.WarehouseId,
            WarehouseName = warehouseName,
            WarehouseQuantity = warehouseStock.Quantity,
            AssignedQuantity = assignedQuantity
        };
    }

    private static List<UnassignedStockDto> SortRows(
        IEnumerable<UnassignedStockDto> rows,
        string? sort)
    {
        return sort switch
        {
            "barcode" => rows
                .OrderBy(row => row.Barcode)
                .ThenBy(row => row.ProductName)
                .ToList(),
            "warehouse" => rows
                .OrderBy(row => row.WarehouseName)
                .ThenBy(row => row.ProductName)
                .ToList(),
            _ => rows
                .OrderBy(row => row.ProductName)
                .ThenBy(row => row.WarehouseName)
                .ToList()
        };
    }
}
