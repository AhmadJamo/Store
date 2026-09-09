using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class PurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;

    private readonly IProductRepository _productRepository;

    private readonly ISupplierRepository _supplierRepository;

    private readonly IWarehouseRepository _warehouseRepository;
  
    private readonly IProductStockRepository _productStockRepository;

    private readonly IStockTransactionRepository _stockTransactionRepository;

    private readonly IUnitOfWork _unitOfWork;

    public PurchaseService(
        IPurchaseRepository purchaseRepository,
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        IWarehouseRepository warehouseRepository,
        IProductStockRepository productStockRepository,
        IStockTransactionRepository stockTransactionRepository,
        IUnitOfWork unitOfWork)
    {
        _purchaseRepository = purchaseRepository;

        _productRepository = productRepository;

        _supplierRepository = supplierRepository;

        _warehouseRepository = warehouseRepository;

        _productStockRepository = productStockRepository;

        _stockTransactionRepository = stockTransactionRepository;

        _unitOfWork = unitOfWork;
    }

    public async Task<List<PurchaseDto>> GetAllAsync()
    {
        var purchases =
            await _purchaseRepository.GetAllAsync();

        var suppliers =
            await _supplierRepository.GetAllAsync();

        var warehouses =
            await _warehouseRepository.GetAllAsync();

        var products =
            await _productRepository.GetAllAsync(null);

        return purchases.Select(purchase =>
            new PurchaseDto
            {
                Id = purchase.Id,

                SupplierId = purchase.SupplierId,

                SupplierName = suppliers
                    .FirstOrDefault(x =>
                        x.Id == purchase.SupplierId)
                    ?.Name ?? "Unknown Supplier",

                WarehouseId = purchase.WarehouseId,

                WarehouseName = warehouses
                    .FirstOrDefault(x =>
                        x.Id == purchase.WarehouseId)
                    ?.Name ?? "Unknown Warehouse",

                InvoiceNumber =
                    purchase.InvoiceNumber,

                Date = purchase.Date,

                Notes = purchase.Notes,

                TotalAmount =
                    purchase.TotalAmount,

                Items = purchase.Items
                    .Select(item =>
                        new PurchaseItemDto
                        {
                            Id = item.Id,

                            ProductId =
                                item.ProductId,

                            ProductName = products
                                .FirstOrDefault(x =>
                                    x.Id == item.ProductId)
                                ?.Name
                                ?? "Unknown Product",

                            Quantity =
                                item.Quantity,

                            PurchasePrice =
                                item.PurchasePrice,

                            Total =
                                item.Total

                        })
                    .ToList()
            })
            .ToList();
    }

    public async Task<PurchaseDto?> GetByIdAsync(int id)
    {
        var purchase =
            await _purchaseRepository.GetByIdAsync(id);

        if (purchase == null)
            return null;

        var supplier =
            await _supplierRepository
                .GetByIdAsync(purchase.SupplierId);

        var warehouse =
            await _warehouseRepository
                .GetByIdAsync(purchase.WarehouseId);

        var products =
            await _productRepository.GetAllAsync(null);

        return new PurchaseDto
        {
            Id = purchase.Id,

            SupplierId =
                purchase.SupplierId,

            SupplierName =
                supplier?.Name ?? "Unknown Supplier",

            WarehouseId =
                purchase.WarehouseId,

            WarehouseName =
                warehouse?.Name ?? "Unknown Warehouse",

            InvoiceNumber =
                purchase.InvoiceNumber,

            Date =
                purchase.Date,

            Notes =
                purchase.Notes,

            TotalAmount =
                purchase.TotalAmount,

            Items = purchase.Items
                .Select(item =>
                    new PurchaseItemDto
                    {
                        Id = item.Id,

                        ProductId =
                            item.ProductId,

                        ProductName = products
                            .FirstOrDefault(x =>
                                x.Id == item.ProductId)
                            ?.Name
                            ?? "Unknown Product",

                        Quantity =
                            item.Quantity,

                        PurchasePrice =
                            item.PurchasePrice,

                        Total =
                            item.Total
                    })
                .ToList()
        };
    }





    public async Task CreateAsync(
     CreatePurchaseDto dto)
    {
        if (dto.SupplierId <= 0)
            throw new ArgumentException(
                "Supplier is required.");

        if (dto.WarehouseId <= 0)
            throw new ArgumentException(
                "Warehouse is required.");

        if (string.IsNullOrWhiteSpace(dto.InvoiceNumber))
            throw new ArgumentException(
                "Invoice number is required.");

        if (dto.Items == null ||
            dto.Items.Count == 0)
        {
            throw new ArgumentException(
                "Purchase must contain at least one item.");
        }

        var supplier =
            await _supplierRepository
                .GetByIdAsync(dto.SupplierId);

        if (supplier == null)
            throw new InvalidOperationException(
                "Supplier not found.");

        var warehouse =
            await _warehouseRepository
                .GetByIdAsync(dto.WarehouseId);

        if (warehouse == null)
            throw new InvalidOperationException(
                "Warehouse not found.");

        await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                var purchase =
                    new Purchase(
                        dto.SupplierId,
                        dto.WarehouseId,
                        dto.InvoiceNumber,
                        dto.Date,
                        dto.Notes);

                foreach (var itemDto in dto.Items)
                {
                    if (itemDto.ProductId <= 0)
                        throw new ArgumentException(
                            "Product is required.");

                    if (itemDto.Quantity <= 0)
                        throw new ArgumentException(
                            "Quantity must be greater than zero.");

                    if (itemDto.PurchasePrice < 0)
                        throw new ArgumentException(
                            "Purchase price cannot be negative.");

                    var product =
                        await _productRepository
                            .GetByIdAsync(itemDto.ProductId);

                    if (product == null)
                    {
                        throw new InvalidOperationException(
                            $"Product with ID {itemDto.ProductId} not found.");
                    }

                    var purchaseItem =
                        new PurchaseItem(
                            itemDto.ProductId,
                            itemDto.Quantity,
                            itemDto.PurchasePrice);

                    purchase.AddItem(purchaseItem);

                    var stock =
                        await _productStockRepository
                            .GetByProductAndWarehouseAsync(
                                itemDto.ProductId,
                                dto.WarehouseId);

                    if (stock == null)
                    {
                        stock = new ProductStock(
                            itemDto.ProductId,
                            dto.WarehouseId);

                        await _productStockRepository
                            .AddAsync(stock);
                    }

                    stock.AddQuantity(
                        itemDto.Quantity);

                    var transaction =
                        new StockTransaction(
                            itemDto.ProductId,
                            dto.WarehouseId,
                            itemDto.Quantity,
                            StockTransactionType.Purchase,
                            dto.InvoiceNumber);

                    await _stockTransactionRepository
                        .AddAsync(transaction);
                }

                await _purchaseRepository
                    .AddAsync(purchase);
            });
    }
}
