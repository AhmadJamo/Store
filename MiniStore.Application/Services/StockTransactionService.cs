using MiniStore.Application.DTOs.ProductStocks;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class StockTransactionService
{
    private readonly IStockTransactionRepository
        _stockTransactionRepository;

    private readonly IProductRepository
        _productRepository;

    private readonly IWarehouseRepository
        _warehouseRepository;

    private readonly IProductStockRepository
        _productStockRepository;

    public StockTransactionService(
        IStockTransactionRepository stockTransactionRepository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        IProductStockRepository productStockRepository)
    {
        _stockTransactionRepository =
            stockTransactionRepository;

        _productRepository =
            productRepository;

        _warehouseRepository =
            warehouseRepository;

        _productStockRepository =
            productStockRepository;
    }

    public async Task CreateAsync(
        CreateStockTransactionDto dto)
    {
        if (dto.Quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        var product =
            await _productRepository.GetByIdAsync(
                dto.ProductId);

        if (product == null)
            throw new InvalidOperationException(
                "Product not found.");

        var warehouse =
            await _warehouseRepository.GetByIdAsync(
                dto.WarehouseId);

        if (warehouse == null)
            throw new InvalidOperationException(
                "Warehouse not found.");

        var stock =
            await _productStockRepository
                .GetByProductAndWarehouseAsync(
                    dto.ProductId,
                    dto.WarehouseId);

        if (stock == null)
        {
            stock = new ProductStock(
                dto.ProductId,
                dto.WarehouseId);

            await _productStockRepository
                .AddAsync(stock);
        }

        var finalQuantity = dto.Type switch
        {
            StockTransactionType.OpeningBalance
                => dto.Quantity,

            StockTransactionType.Purchase
                => dto.Quantity,

            StockTransactionType.Sale
                => -dto.Quantity,

            StockTransactionType.TransferIn
                => dto.Quantity,

            StockTransactionType.TransferOut
                => -dto.Quantity,

            StockTransactionType.AdjustmentIn
                => dto.Quantity,

            StockTransactionType.AdjustmentOut
                => -dto.Quantity,

            _ => throw new ArgumentException(
                "Invalid stock transaction type.")
        };

        if (finalQuantity > 0)
        {
            stock.AddQuantity(finalQuantity);
        }
        else
        {
            stock.RemoveQuantity(-finalQuantity);
        }

        var transaction =
            new StockTransaction(
                dto.ProductId,
                dto.WarehouseId,
                finalQuantity,
                dto.Type,
                dto.Reference);

        await _stockTransactionRepository
            .AddAsync(transaction);

        await _stockTransactionRepository
            .SaveChangesAsync();

        await _productStockRepository
            .SaveChangesAsync();
    }
    public async Task<List<StockTransactionDto>> GetAllAsync()
    {
        var transactions =
            await _stockTransactionRepository.GetAllAsync();

        var products =
            await _productRepository.GetAllAsync(null);

        var warehouses =
            await _warehouseRepository.GetAllAsync();

        return transactions.Select(transaction =>
            new StockTransactionDto
            {
                Id = transaction.Id,

                ProductId = transaction.ProductId,

                ProductName = products
                    .FirstOrDefault(x =>
                        x.Id == transaction.ProductId)
                    ?.Name ?? "Unknown Product",

                WarehouseId = transaction.WarehouseId,

                WarehouseName = warehouses
                    .FirstOrDefault(x =>
                        x.Id == transaction.WarehouseId)
                    ?.Name ?? "Unknown Warehouse",

                Quantity = transaction.Quantity,

                Type = transaction.Type,

                Reference = transaction.Reference,

                CreatedAt = transaction.CreatedAt

            }).ToList();
    }
}