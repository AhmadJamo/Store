
using MiniStore.Application.DTOs.Sales;
using MiniStore.Application.Permissions;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Enums;
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
    private readonly IDiscountSettingsRepository _discountSettingsRepository;
    private readonly IPermissionService _permissionService;

    public SaleService(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        IProductStockRepository productStockRepository,
        IStockTransactionRepository stockTransactionRepository,
        IUnitOfWork unitOfWork,
        IInvoiceSettingsRepository invoiceSettingsRepository,
        IDiscountSettingsRepository discountSettingsRepository,
        IPermissionService permissionService)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _productStockRepository = productStockRepository;
        _stockTransactionRepository = stockTransactionRepository;
        _unitOfWork = unitOfWork;
        _invoiceSettingsRepository = invoiceSettingsRepository;
        _discountSettingsRepository = discountSettingsRepository;
        _permissionService = permissionService;
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

            Subtotal = sale.Subtotal,
            InvoiceDiscountType = sale.InvoiceDiscountType,
            InvoiceDiscountValue = sale.InvoiceDiscountValue,
            InvoiceDiscountAmount = sale.InvoiceDiscountAmount,
            TotalAmount = sale.TotalAmount,

            Items = sale.Items
                .Select(item => new SaleItemDetailsDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,

                    GrossTotal = item.GrossTotal,
                    DiscountType = item.DiscountType,
                    DiscountValue = item.DiscountValue,
                    DiscountAmount = item.DiscountAmount,
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
            var discountSettings =
                await _discountSettingsRepository.GetAsync();

            if (discountSettings == null)
            {
                throw new InvalidOperationException(
                    "Discount settings have not been configured.");
            }

            if (!discountSettings.Enabled)
            {
                if (dto.Items.Any(x => x.DiscountValue > 0) ||
                    dto.InvoiceDiscountValue > 0)
                {
                    throw new InvalidOperationException(
                        "Discounts are currently disabled.");
                }
            }

            if (!discountSettings.AllowLineDiscount &&
                dto.Items.Any(x => x.DiscountValue > 0))
            {
                throw new InvalidOperationException(
                    "Line discounts are currently disabled.");
            }

            if (!discountSettings.AllowInvoiceDiscount &&
                dto.InvoiceDiscountValue > 0)
            {
                throw new InvalidOperationException(
                    "Invoice discounts are currently disabled.");
            }

            if (!discountSettings.AllowPercentageDiscount &&
                dto.Items.Any(x =>
                    x.DiscountValue > 0 &&
                    x.DiscountType == DiscountType.Percentage))
            {
                throw new InvalidOperationException(
                    "Percentage line discounts are currently disabled.");
            }

            if (!discountSettings.AllowFixedAmountDiscount &&
                dto.Items.Any(x =>
                    x.DiscountValue > 0 &&
                    x.DiscountType == DiscountType.FixedAmount))
            {
                throw new InvalidOperationException(
                    "Fixed amount line discounts are currently disabled.");
            }

            if (dto.InvoiceDiscountValue > 0 &&
                dto.InvoiceDiscountType == DiscountType.Percentage &&
                !discountSettings.AllowPercentageDiscount)
            {
                throw new InvalidOperationException(
                    "Percentage invoice discounts are currently disabled.");
            }

            if (dto.InvoiceDiscountValue > 0 &&
                dto.InvoiceDiscountType == DiscountType.FixedAmount &&
                !discountSettings.AllowFixedAmountDiscount)
            {
                throw new InvalidOperationException(
                    "Fixed amount invoice discounts are currently disabled.");
            }

            var canOverrideDiscountLimit =
                discountSettings.AllowDiscountAboveLimit &&
                await _permissionService.CanAsync(
                    createdByUserId,
                    discountSettings.DiscountOverridePermission);

            foreach (var item in dto.Items.Where(x => x.DiscountValue > 0))
            {
                if (item.DiscountType == DiscountType.Percentage &&
                    item.DiscountValue >
                        discountSettings.MaxLineDiscountPercent &&
                    !canOverrideDiscountLimit)
                {
                    throw new InvalidOperationException(
                        $"Line discount percentage cannot exceed " +
                        $"{discountSettings.MaxLineDiscountPercent}%.");
                }

                if (item.DiscountType == DiscountType.FixedAmount &&
                    item.DiscountValue >
                        discountSettings.MaxLineDiscountAmount &&
                    !canOverrideDiscountLimit)
                {
                    throw new InvalidOperationException(
                        $"Line discount amount cannot exceed " +
                        $"{discountSettings.MaxLineDiscountAmount}.");
                }
            }

            var invoiceSettings =
                await _invoiceSettingsRepository.GetAsync();

            if (invoiceSettings == null)
            {
                invoiceSettings = new InvoiceSettings();

                await _invoiceSettingsRepository
                    .AddAsync(invoiceSettings);
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

                var salePrice =
                    channel == SaleChannel.Wholesale
                        ? product.WholesalePrice
                        : product.SalePrice;

                sale.AddItem(
                    new SaleItem(
                        itemDto.ProductId,
                        itemDto.Quantity,
                        salePrice,
                        itemDto.DiscountType,
                        itemDto.DiscountValue));

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

            if (dto.InvoiceDiscountValue > 0)
            {
                if (dto.InvoiceDiscountType == DiscountType.Percentage &&
                    dto.InvoiceDiscountValue >
                        discountSettings.MaxInvoiceDiscountPercent &&
                    !canOverrideDiscountLimit)
                {
                    throw new InvalidOperationException(
                        $"Invoice discount percentage cannot exceed " +
                        $"{discountSettings.MaxInvoiceDiscountPercent}%.");
                }

                if (dto.InvoiceDiscountType == DiscountType.FixedAmount &&
                    dto.InvoiceDiscountValue >
                        discountSettings.MaxInvoiceDiscountAmount &&
                    !canOverrideDiscountLimit)
                {
                    throw new InvalidOperationException(
                        $"Invoice discount amount cannot exceed " +
                        $"{discountSettings.MaxInvoiceDiscountAmount}.");
                }
            }

            sale.ApplyInvoiceDiscount(
                dto.InvoiceDiscountType,
                dto.InvoiceDiscountValue);

            await _saleRepository.AddAsync(sale);
        });

        return sale!.Id;
    }
}

