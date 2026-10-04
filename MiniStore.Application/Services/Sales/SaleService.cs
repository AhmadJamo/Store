
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
    private readonly IInventoryBalanceRepository _inventoryBalanceRepository;
    private readonly InventoryTrackingService _inventoryTrackingService;
    private readonly DocumentNumberService _documentNumbers;
    private readonly IDiscountSettingsRepository _discountSettingsRepository;
    private readonly IPermissionService _permissionService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly InventoryAccessService _inventoryAccessService;
    private readonly IPosTerminalSettingsRepository _posTerminalSettingsRepository;
    private readonly IProductRecipeRepository _productRecipeRepository;
    private readonly ITaxRateRepository _taxRateRepository;

    public SaleService(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        IProductStockRepository productStockRepository,
        IStockTransactionRepository stockTransactionRepository,
        IUnitOfWork unitOfWork,
        IInventoryBalanceRepository inventoryBalanceRepository,
        InventoryTrackingService inventoryTrackingService,
        DocumentNumberService documentNumbers,
        IDiscountSettingsRepository discountSettingsRepository,
        IPermissionService permissionService,
        ICustomerRepository customerRepository,
        IPaymentMethodRepository paymentMethodRepository,
        IWarehouseRepository warehouseRepository,
        InventoryAccessService inventoryAccessService,
        IPosTerminalSettingsRepository posTerminalSettingsRepository,
        IProductRecipeRepository productRecipeRepository,
        ITaxRateRepository taxRateRepository)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _productStockRepository = productStockRepository;
        _stockTransactionRepository = stockTransactionRepository;
        _unitOfWork = unitOfWork;
        _inventoryBalanceRepository = inventoryBalanceRepository;
        _inventoryTrackingService = inventoryTrackingService;
        _documentNumbers = documentNumbers;
        _discountSettingsRepository = discountSettingsRepository;
        _permissionService = permissionService;
        _customerRepository = customerRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _warehouseRepository = warehouseRepository;
        _inventoryAccessService = inventoryAccessService;
        _posTerminalSettingsRepository = posTerminalSettingsRepository;
        _productRecipeRepository = productRecipeRepository;
        _taxRateRepository = taxRateRepository;
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

        var productNames = (await _productRepository.GetAllAsync(null))
            .ToDictionary(x => x.Id, x => x.Name);

        return new SaleDetailsDto
        {
            Id = sale.Id,
            InvoiceNumber = sale.InvoiceNumber,
            WarehouseId = sale.WarehouseId,
            PosTerminalId = sale.PosTerminalId,
            PosOrderType = sale.PosOrderType,
            ServiceReference = sale.ServiceReference,
            GuestCount = sale.GuestCount,
            Date = sale.Date,
            Notes = sale.Notes,

            Subtotal = sale.Subtotal,
            InvoiceDiscountType = sale.InvoiceDiscountType,
            InvoiceDiscountValue = sale.InvoiceDiscountValue,
            InvoiceDiscountAmount = sale.InvoiceDiscountAmount,
            TaxRateId = sale.TaxRateId,
            TaxRatePercent = sale.TaxRatePercent,
            IsTaxInclusive = sale.IsTaxInclusive,
            TaxAmount = sale.TaxAmount,
            TotalAmount = sale.TotalAmount,

            Items = sale.Items
                .Select(item => new SaleItemDetailsDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = productNames.GetValueOrDefault(item.ProductId, $"Product #{item.ProductId}"),
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,

                    GrossTotal = item.GrossTotal,
                    DiscountType = item.DiscountType,
                    DiscountValue = item.DiscountValue,
                    DiscountAmount = item.DiscountAmount,
                    Total = item.Total,
                    Notes = item.Notes,
                    UnitCost = item.UnitCost,
                    CostOfGoodsSold = item.CostOfGoodsSold
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
        if (dto.CustomerId.HasValue && await _customerRepository.GetByIdAsync(dto.CustomerId.Value) is null) throw new ArgumentException("Selected customer was not found.");
        if (dto.PaymentMethodId <= 0 || await _paymentMethodRepository.GetByIdAsync(dto.PaymentMethodId) is null) throw new ArgumentException("Select an active payment method.");
        var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId)
            ?? throw new ArgumentException("Selected warehouse was not found.");
        if (channel == SaleChannel.RetailPos && !warehouse.AllowPosSales)
        {
            throw new InvalidOperationException(
                "The selected warehouse is not enabled for POS sales.");
        }
        PosTerminalSettings? terminalSettings = null;
        if (channel == SaleChannel.RetailPos)
        {
            if (!dto.PosTerminalId.HasValue)
            {
                if (await _inventoryAccessService.HasPosTerminalsAsync())
                    throw new ArgumentException("Select a POS terminal.");
            }
            else if (!await _inventoryAccessService.CanPosSellFromWarehouseAsync(
                dto.PosTerminalId.Value,
                dto.WarehouseId))
            {
                throw new InvalidOperationException(
                    "This POS terminal is not allowed to sell from the selected warehouse.");
            }

            if (dto.PosTerminalId.HasValue)
            {
                terminalSettings = await _posTerminalSettingsRepository.GetAsync(dto.PosTerminalId.Value)
                    ?? new PosTerminalSettings(dto.PosTerminalId.Value, PosExperienceProfile.Retail);
            }

            var effectiveOrderType = dto.PosOrderType
                ?? terminalSettings?.DefaultOrderType
                ?? PosOrderType.WalkIn;
            if (terminalSettings is not null && !terminalSettings.Supports(effectiveOrderType))
                throw new ArgumentException("The selected order type is not enabled for this POS terminal.");
            if (terminalSettings is null && effectiveOrderType != PosOrderType.WalkIn)
                throw new ArgumentException("Legacy POS supports walk-in orders only.");
            if (terminalSettings?.RequireServiceReference == true &&
                effectiveOrderType == PosOrderType.DineIn &&
                string.IsNullOrWhiteSpace(dto.ServiceReference))
                throw new ArgumentException("Enter a table or service reference for dine-in orders.");
            if (dto.GuestCount.HasValue && terminalSettings?.EnableGuestCount != true)
                throw new ArgumentException("Guest count is not enabled for this POS terminal.");
            if (dto.GuestCount.HasValue && effectiveOrderType != PosOrderType.DineIn)
                throw new ArgumentException("Guest count is only valid for dine-in orders.");
            if (dto.Items.Any(x => !string.IsNullOrWhiteSpace(x.Notes)) &&
                terminalSettings?.EnableItemNotes != true)
                throw new ArgumentException("Item notes are not enabled for this POS terminal.");

            dto.PosOrderType = effectiveOrderType;
        }

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

            sale = new Sale(
                await _documentNumbers.GenerateAsync(
                    channel == SaleChannel.Wholesale
                        ? DocumentNumberType.WholesaleSale
                        : DocumentNumberType.PosSale,
                    dto.Date),
                dto.WarehouseId,
                dto.Date,
                channel,
                createdByUserId,
                dto.CustomerId,
                dto.PaymentMethodId,
                dto.Notes,
                dto.PosTerminalId,
                channel == SaleChannel.RetailPos ? dto.PosOrderType : null,
                channel == SaleChannel.RetailPos ? dto.ServiceReference : null,
                channel == SaleChannel.RetailPos ? dto.GuestCount : null);

            if (dto.TaxRateId.HasValue)
            {
                var taxRate = (await _taxRateRepository.GetAllAsync())
                    .SingleOrDefault(x => x.Id == dto.TaxRateId.Value)
                    ?? throw new ArgumentException("The selected sales tax was not found.");
                sale.ApplyTax(
                    taxRate.Id,
                    taxRate.Rate,
                    taxRate.OutputAccountId,
                    taxRate.IsPriceInclusive);
            }

            var directRequirements = new List<(Product Product, decimal Quantity)>();
            var recipeRequirements = new Dictionary<int, (Product Product, decimal Quantity)>();
            var preparedUnitCosts = new Dictionary<int, decimal>();

            foreach (var itemDto in dto.Items)
            {
                var product = await _productRepository
                    .GetByIdAsync(itemDto.ProductId);

                if (product == null)
                {
                    throw new ArgumentException(
                        $"Product with ID {itemDto.ProductId} was not found.");
                }

                if (!product.IsActive)
                    throw new InvalidOperationException("The selected product is inactive.");
                if (channel == SaleChannel.RetailPos && !product.IsSellableInPos)
                    throw new InvalidOperationException("The selected product is not enabled for POS sales.");
                if (channel == SaleChannel.Wholesale && !product.IsSellableInSales)
                    throw new InvalidOperationException("The selected product is not enabled for sales invoices.");

                var salePrice =
                    channel == SaleChannel.Wholesale
                        ? product.WholesalePrice
                        : product.SalePrice;

                ProductRecipe? recipe = null;
                if (product.InventoryBehavior == ProductInventoryBehavior.PreparedToOrder)
                {
                    recipe = await _productRecipeRepository
                        .GetActiveByProductIdAsync(product.Id)
                        ?? throw new InvalidOperationException(
                            "Prepared product has no active recipe.");

                    decimal recipeUnitCost = 0;
                    foreach (var ingredientLine in recipe.Ingredients)
                    {
                        var ingredient = await _productRepository
                            .GetByIdAsync(ingredientLine.IngredientProductId)
                            ?? throw new InvalidOperationException(
                                "A recipe ingredient was not found.");
                        var stockUnitChanged = ingredientLine.StockMeasurementUnitId.HasValue
                            ? ingredient.MeasurementUnitId != ingredientLine.StockMeasurementUnitId
                            : ingredient.StockUnit != ingredientLine.StockUnitSnapshot;
                        if (stockUnitChanged)
                            throw new InvalidOperationException(
                                "An ingredient stock unit changed after this recipe version was created. Create a new recipe version after reconciling stock.");
                        var requestedRecipeQuantity =
                            ingredientLine.StockQuantity * itemDto.Quantity / recipe.YieldQuantity;
                        var stockQuantity = Math.Round(
                            requestedRecipeQuantity,
                            6,
                            MidpointRounding.AwayFromZero);
                        if (stockQuantity <= 0)
                            throw new InvalidOperationException(
                                "Recipe consumption is below the supported stock precision.");

                        var ingredientStock = await _productStockRepository
                            .GetByProductAndWarehouseAsync(ingredient.Id, dto.WarehouseId);
                        var ingredientUnitCost = ingredientStock?.AverageUnitCost > 0
                            ? ingredientStock.AverageUnitCost
                            : ingredientStock?.LastReferenceUnitCost > 0
                                ? ingredientStock.LastReferenceUnitCost
                                : ingredient.PurchasePrice;
                        recipeUnitCost += ingredientLine.StockQuantity /
                            recipe.YieldQuantity * ingredientUnitCost;

                        if (recipeRequirements.TryGetValue(ingredient.Id, out var existing))
                            recipeRequirements[ingredient.Id] = (ingredient, existing.Quantity + stockQuantity);
                        else
                            recipeRequirements[ingredient.Id] = (ingredient, stockQuantity);
                    }
                    preparedUnitCosts[product.Id] = Math.Round(
                        recipeUnitCost, 8, MidpointRounding.AwayFromZero);
                }
                else
                {
                    directRequirements.Add((product, itemDto.Quantity));
                }

                sale.AddItem(
                    new SaleItem(
                        itemDto.ProductId,
                        itemDto.Quantity,
                        salePrice,
                        itemDto.DiscountType,
                        itemDto.DiscountValue,
                        itemDto.Notes,
                        recipe?.Id));

                if (preparedUnitCosts.TryGetValue(product.Id, out var preparedUnitCost))
                {
                    sale.Items.Single(x => x.ProductId == product.Id)
                        .SetCostSnapshot(
                            preparedUnitCost,
                            preparedUnitCost * itemDto.Quantity);
                }
            }

            foreach (var requirement in directRequirements)
            {
                var stock = await _productStockRepository
                    .GetByProductAndWarehouseAsync(requirement.Product.Id, dto.WarehouseId)
                    ?? throw new InvalidOperationException("Stock record was not found.");

                var available = await _inventoryBalanceRepository.GetWarehouseAvailableAsync(
                    requirement.Product.Id, dto.WarehouseId);
                if (available < requirement.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient available stock for product '{requirement.Product.Name}'. " +
                        $"Available after reservations: {available}, Requested: {requirement.Quantity}.");

                if (stock.Quantity < requirement.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock for product '{requirement.Product.Name}'. " +
                        $"Available: {stock.Quantity}, Requested: {requirement.Quantity}.");

                await _inventoryTrackingService.IssueAsync(
                    requirement.Product, dto.WarehouseId, requirement.Quantity,
                    sale!.InvoiceNumber, createdByUserId);

                var costMovement = stock.RemoveQuantity(requirement.Quantity);
                sale.Items.Single(x => x.ProductId == requirement.Product.Id)
                    .SetCostSnapshot(
                        costMovement.UnitCost,
                        Math.Abs(costMovement.TransactionValue));

                await _stockTransactionRepository.AddAsync(new StockTransaction(
                    requirement.Product.Id,
                    dto.WarehouseId,
                    -requirement.Quantity,
                    StockTransactionType.Sale,
                    sale.InvoiceNumber,
                    costMovement));
            }

            foreach (var requirement in recipeRequirements.Values)
            {
                var stock = await _productStockRepository
                    .GetByProductAndWarehouseAsync(requirement.Product.Id, dto.WarehouseId);
                if (stock is null)
                {
                    if (!requirement.Product.AllowNegativeRecipeConsumption)
                        throw new InvalidOperationException(
                            "Stock record was not found for a recipe ingredient.");

                    stock = new ProductStock(requirement.Product.Id, dto.WarehouseId);
                    await _productStockRepository.AddAsync(stock);
                }

                stock.EnsureReferenceUnitCost(requirement.Product.PurchasePrice);

                if (requirement.Product.TrackingPolicy != ProductTrackingPolicy.None)
                {
                    await _inventoryTrackingService.IssueAsync(
                        requirement.Product, dto.WarehouseId, requirement.Quantity,
                        sale!.InvoiceNumber, createdByUserId);
                }

                var costMovement = stock.ConsumeRecipeQuantity(
                    requirement.Quantity,
                    requirement.Product.AllowNegativeRecipeConsumption);

                await _stockTransactionRepository.AddAsync(new StockTransaction(
                    requirement.Product.Id,
                    dto.WarehouseId,
                    -requirement.Quantity,
                    StockTransactionType.RecipeConsumption,
                    sale.InvoiceNumber,
                    costMovement));
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

