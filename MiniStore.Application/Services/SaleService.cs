using MiniStore.Application.DTOs.Sale;
using MiniStore.Application.DTOs.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProductStockRepository _productStockRepository;
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceSettingsRepository _invoiceSettingsRepository;

    public SaleService(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        IProductStockRepository productStockRepository,
        IStockTransactionRepository stockTransactionRepository,
        IUnitOfWork unitOfWork,
        IInvoiceSettingsRepository invoiceSettingsRepository)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _productStockRepository = productStockRepository;
        _stockTransactionRepository = stockTransactionRepository;
        _unitOfWork = unitOfWork;
        _invoiceSettingsRepository = invoiceSettingsRepository;
    }

    public async Task<List<SaleListDto>> GetAllAsync()
    {
        var sales = await _saleRepository.GetAllAsync();

        return sales.Select(s => new SaleListDto
        {
            Id = s.Id,
            InvoiceNumber = s.InvoiceNumber,
            WarehouseId = s.WarehouseId,
            Date = s.Date,
            TotalAmount = s.TotalAmount
        }).ToList();
    }

    public async Task<SaleDetailsDto?> GetByIdAsync(int id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);

        if (sale == null)
            return null;

        return new SaleDetailsDto
        {
            Id = sale.Id,
            InvoiceNumber = sale.InvoiceNumber,
            WarehouseId = sale.WarehouseId,
            Date = sale.Date,
            Notes = sale.Notes,
            TotalAmount = sale.TotalAmount,

            Items = sale.Items
                .Select(item => new SaleItemDetailsDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,
                    Total = item.Total
                })
                .ToList()
        };
    }

    public async Task<int> CreateAsync(
        CreateSaleDto dto,
        SaleChannel channel,
        string createdByUserId)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.Items == null || dto.Items.Count == 0)
            throw new ArgumentException(
                "Sale must contain at least one item.");

        Sale? sale = null;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var invoiceSettings =
                await _invoiceSettingsRepository.GetAsync();

            if (invoiceSettings == null)
            {
                invoiceSettings = new InvoiceSettings();
                await _invoiceSettingsRepository.AddAsync(invoiceSettings);
            }

            sale = new Sale(
                invoiceSettings.GenerateNextNumber(channel),
                dto.WarehouseId,
                dto.Date,
                channel,
                createdByUserId,
                dto.Notes);

            foreach (var itemDto in dto.Items)
            {
                var product = await _productRepository
                    .GetByIdAsync(itemDto.ProductId);

                if (product == null)
                {
                    throw new ArgumentException(
                        $"Product with ID {itemDto.ProductId} was not found.");
                }

                var stock = await _productStockRepository
                    .GetByProductAndWarehouseAsync(
                        itemDto.ProductId,
                        dto.WarehouseId);

                if (stock == null)
                {
                    throw new InvalidOperationException(
                        "Stock record was not found.");
                }

                if (stock.Quantity < itemDto.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for product '{product.Name}'. " +
                        $"Available: {stock.Quantity}, " +
                        $"Requested: {itemDto.Quantity}.");
                }

                sale.AddItem(new SaleItem(
                    itemDto.ProductId,
                    itemDto.Quantity,
                    channel == SaleChannel.Wholesale
                        ? product.WholesalePrice
                        : product.SalePrice));

                stock.RemoveQuantity(itemDto.Quantity);

                var transaction = new StockTransaction(
                    itemDto.ProductId,
                    dto.WarehouseId,
                    -itemDto.Quantity,
                    StockTransactionType.Sale,
                    dto.InvoiceNumber);

                await _stockTransactionRepository
                    .AddAsync(transaction);
            }

            await _saleRepository.AddAsync(sale);
        });

        return sale!.Id;
    }
}
