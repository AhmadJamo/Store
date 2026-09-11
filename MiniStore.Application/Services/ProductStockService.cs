using MiniStore.Application.DTOs.ProductStocks;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class ProductStockService
{
    private readonly IProductStockRepository _productStockRepository;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductStockService(
        IProductStockRepository productStockRepository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        IStockTransactionRepository stockTransactionRepository,
        IUnitOfWork unitOfWork)
    {
        _productStockRepository = productStockRepository;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _stockTransactionRepository = stockTransactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ProductStockDto>> GetAllAsync()
    {
        var stocks =
            await _productStockRepository.GetAllAsync();

        var products =
            await _productRepository.GetAllAsync(null);

        var warehouses =
            await _warehouseRepository.GetAllAsync();

        return stocks.Select(stock => new ProductStockDto
        {
            Id = stock.Id,

            ProductId = stock.ProductId,

            ProductName = products
                .FirstOrDefault(x => x.Id == stock.ProductId)
                ?.Name ?? "Unknown Product",

            WarehouseId = stock.WarehouseId,

            WarehouseName = warehouses
                .FirstOrDefault(x => x.Id == stock.WarehouseId)
                ?.Name ?? "Unknown Warehouse",

            Quantity = stock.Quantity

        }).ToList();
    }

    public async Task<ProductStockDto?> GetByIdAsync(int id)
    {
        var stock =
            await _productStockRepository.GetByIdAsync(id);

        if (stock == null)
            return null;

        var product =
            await _productRepository.GetByIdAsync(
                stock.ProductId);

        var warehouse =
            await _warehouseRepository.GetByIdAsync(
                stock.WarehouseId);

        return new ProductStockDto
        {
            Id = stock.Id,

            ProductId = stock.ProductId,

            ProductName =
                product?.Name ?? "Unknown Product",

            WarehouseId = stock.WarehouseId,

            WarehouseName =
                warehouse?.Name ?? "Unknown Warehouse",

            Quantity = stock.Quantity
        };
    }

    public async Task CreateAsync(
        CreateProductStockDto dto)
    {
        // Opening Balance may be zero,
        // but it can never be negative.
        if (dto.Quantity < 0)
            throw new ArgumentException(
                "Opening balance quantity cannot be negative.");

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

        var existingStock =
            await _productStockRepository
                .GetByProductAndWarehouseAsync(
                    dto.ProductId,
                    dto.WarehouseId);

        if (existingStock != null)
            throw new InvalidOperationException(
                "Stock already exists for this product and warehouse.");

        var stock = new ProductStock(
            dto.ProductId,
            dto.WarehouseId);

        // AddQuantity does not allow zero,
        // so only call it when the opening balance is greater than zero.
        if (dto.Quantity > 0)
        {
            stock.AddQuantity(dto.Quantity);
        }

        await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                await _productStockRepository.AddAsync(stock);

                await _stockTransactionRepository.AddAsync(
                    new StockTransaction(
                        dto.ProductId,
                        dto.WarehouseId,
                        dto.Quantity,
                        StockTransactionType.OpeningBalance,
                        "Opening balance"));
            });
    }

    public async Task UpdateAsync(
        int id,
        UpdateProductStockDto dto)
    {
        if (dto.Quantity < 0)
            throw new ArgumentException(
                "Quantity cannot be negative.");

        var stock =
            await _productStockRepository.GetByIdAsync(id);

        if (stock == null)
            throw new InvalidOperationException(
                "Stock record not found.");

        var difference =
            dto.Quantity - stock.Quantity;

        if (difference == 0)
            return;

        await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                if (difference > 0)
                {
                    stock.AddQuantity(difference);
                }
                else
                {
                    stock.RemoveQuantity(-difference);
                }

                await _stockTransactionRepository.AddAsync(
                    new StockTransaction(
                        stock.ProductId,
                        stock.WarehouseId,
                        difference,
                        difference > 0
                            ? StockTransactionType.AdjustmentIn
                            : StockTransactionType.AdjustmentOut,
                        "Manual stock adjustment"));
            });
    }

    public async Task DeleteAsync(int id)
    {
        var stock =
            await _productStockRepository.GetByIdAsync(id);

        if (stock == null)
            throw new InvalidOperationException(
                "Stock record not found.");

        throw new InvalidOperationException(
            "Stock records cannot be deleted. Adjust the quantity to zero instead.");
    }
}