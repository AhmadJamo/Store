namespace MiniStore.Domain.Entities;

public class Product
{
    private Product()
    {
        Name = string.Empty;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }

    public string ProductCode { get; private set; } = string.Empty;

    public string? Barcode { get; private set; }

    public decimal PurchasePrice { get; private set; }

    public decimal SalePrice { get; private set; }

    public decimal WholesalePrice { get; private set; }

    public ProductInventoryBehavior InventoryBehavior { get; private set; }

    public ProductType ProductType { get; private set; }

    public UnitOfMeasure StockUnit { get; private set; }

    public int? MeasurementUnitId { get; private set; }

    public int? ProductCategoryId { get; private set; }

    public decimal? NetWeight { get; private set; }

    public decimal? GrossWeight { get; private set; }

    public int? WeightMeasurementUnitId { get; private set; }

    public decimal? Length { get; private set; }

    public decimal? Width { get; private set; }

    public decimal? Height { get; private set; }

    public int? DimensionMeasurementUnitId { get; private set; }

    public ProductTrackingPolicy TrackingPolicy { get; private set; }

    public ProductHandlingRequirements HandlingRequirements { get; private set; }

    public bool AllowNegativeRecipeConsumption { get; private set; }

    public bool IsSellableInPos { get; private set; }

    public bool IsSellableInSales { get; private set; }

    public bool IsActive { get; private set; }

    //State
    public Product(
        string name,
        string? barcode,
        decimal purchasePrice,
        decimal salePrice,
        decimal wholesalePrice,
        ProductInventoryBehavior inventoryBehavior = ProductInventoryBehavior.Stocked,
        UnitOfMeasure stockUnit = UnitOfMeasure.Piece,
        bool allowNegativeRecipeConsumption = false,
        ProductType? productType = null,
        bool isSellableInPos = true,
        bool isSellableInSales = true,
        bool isActive = true,
        int? measurementUnitId = null)
    {
        var resolvedProductType = productType ??
            (inventoryBehavior == ProductInventoryBehavior.PreparedToOrder
                ? ProductType.PreparedToOrder
                : ProductType.DirectSale);

        ValidateName(name);
        ValidateBarcode(barcode);
        ValidatePurchasePrice(purchasePrice);
        ValidateSalePrice(salePrice);
        ValidateWholesalePrice(wholesalePrice);

        ValidateProductConfiguration(
            resolvedProductType,
            inventoryBehavior,
            stockUnit,
            purchasePrice,
            wholesalePrice,
            salePrice,
            isSellableInPos,
            isSellableInSales);

        Name = name;
        Barcode = NormalizeBarcode(barcode);
        PurchasePrice = resolvedProductType == ProductType.PreparedToOrder ? 0 : purchasePrice;
        SalePrice = salePrice;
        WholesalePrice = wholesalePrice;
        InventoryBehavior = MapInventoryBehavior(resolvedProductType);
        ProductType = resolvedProductType;
        StockUnit = stockUnit;
        MeasurementUnitId = measurementUnitId;
        AllowNegativeRecipeConsumption = allowNegativeRecipeConsumption;
        IsSellableInPos = isSellableInPos;
        IsSellableInSales = isSellableInSales;
        IsActive = isActive;
    }

    //Behavior
    public void ChangeName(string name)
    {
        ValidateName(name);

        Name = name;
    }

    public void ChangeBarcode(string? barcode)
    {
        ValidateBarcode(barcode);

        Barcode = NormalizeBarcode(barcode);
    }

    public void ChangePurchasePrice(decimal price)
    {
        ValidatePurchasePrice(price);

        if (SalePrice < price)
            throw new ArgumentException(
                "Purchase price cannot be higher than sale price.");

        PurchasePrice = price;
    }

    public void ChangeSalePrice(decimal price)
    {
        ValidateSalePrice(price);

        ValidatePriceOrder(PurchasePrice, WholesalePrice, price);

        SalePrice = price;
    }

    public void ChangeWholesalePrice(decimal price)
    {
        ValidateWholesalePrice(price);
        ValidatePriceOrder(PurchasePrice, price, SalePrice);

        WholesalePrice = price;
    }

    public void ConfigureInventory(
        ProductInventoryBehavior inventoryBehavior,
        UnitOfMeasure stockUnit,
        bool allowNegativeRecipeConsumption)
    {
        ValidateInventoryConfiguration(inventoryBehavior, stockUnit);

        InventoryBehavior = inventoryBehavior;
        StockUnit = stockUnit;
        AllowNegativeRecipeConsumption = allowNegativeRecipeConsumption;
    }

    public void ConfigureProduct(
        string name,
        string? barcode,
        decimal purchasePrice,
        decimal salePrice,
        decimal wholesalePrice,
        ProductType productType,
        UnitOfMeasure stockUnit,
        bool allowNegativeRecipeConsumption,
        bool isSellableInPos,
        bool isSellableInSales,
        bool isActive,
        int measurementUnitId)
    {
        var inventoryBehavior = MapInventoryBehavior(productType);
        ValidateName(name);
        ValidateBarcode(barcode);
        ValidatePurchasePrice(purchasePrice);
        ValidateSalePrice(salePrice);
        ValidateWholesalePrice(wholesalePrice);
        if (measurementUnitId <= 0)
            throw new ArgumentException("Measurement unit is required.");
        ValidateProductConfiguration(
            productType,
            inventoryBehavior,
            stockUnit,
            purchasePrice,
            wholesalePrice,
            salePrice,
            isSellableInPos,
            isSellableInSales);

        Name = name.Trim();
        Barcode = NormalizeBarcode(barcode);
        PurchasePrice = productType == ProductType.PreparedToOrder ? 0 : purchasePrice;
        SalePrice = salePrice;
        WholesalePrice = wholesalePrice;
        ProductType = productType;
        InventoryBehavior = inventoryBehavior;
        StockUnit = stockUnit;
        MeasurementUnitId = measurementUnitId;
        AllowNegativeRecipeConsumption = allowNegativeRecipeConsumption;
        IsSellableInPos = isSellableInPos;
        IsSellableInSales = isSellableInSales;
        IsActive = isActive;
    }

    public void MarkPreparedToOrder()
    {
        InventoryBehavior = ProductInventoryBehavior.PreparedToOrder;
        ProductType = ProductType.PreparedToOrder;
        PurchasePrice = 0;
    }

    public void ConfigureLogistics(
        int? productCategoryId,
        decimal? netWeight,
        decimal? grossWeight,
        int? weightMeasurementUnitId,
        decimal? length,
        decimal? width,
        decimal? height,
        int? dimensionMeasurementUnitId,
        ProductTrackingPolicy trackingPolicy,
        ProductHandlingRequirements handlingRequirements)
    {
        if (productCategoryId <= 0)
            throw new ArgumentException("Selected product category is invalid.");
        if (netWeight < 0 || grossWeight < 0)
            throw new ArgumentException("Product weights cannot be negative.");
        if (netWeight.HasValue && grossWeight.HasValue && grossWeight < netWeight)
            throw new ArgumentException("Gross weight cannot be less than net weight.");
        if ((netWeight.HasValue || grossWeight.HasValue) != weightMeasurementUnitId.HasValue)
            throw new ArgumentException("Select a mass unit when a product weight is entered.");

        var dimensionValues = new[] { length, width, height };
        var hasAnyDimension = dimensionValues.Any(value => value.HasValue);
        var hasAllDimensions = dimensionValues.All(value => value.HasValue);
        if (hasAnyDimension && !hasAllDimensions)
            throw new ArgumentException("Length, width and height must be entered together.");
        if (hasAllDimensions && dimensionValues.Any(value => value <= 0))
            throw new ArgumentException("Product dimensions must be greater than zero.");
        if (hasAnyDimension != dimensionMeasurementUnitId.HasValue)
            throw new ArgumentException("Select a length unit when product dimensions are entered.");
        if (!Enum.IsDefined(trackingPolicy))
            throw new ArgumentException("Product tracking policy is invalid.");
        const ProductHandlingRequirements allRequirements =
            ProductHandlingRequirements.Fragile |
            ProductHandlingRequirements.KeepDry |
            ProductHandlingRequirements.Refrigerated |
            ProductHandlingRequirements.Frozen |
            ProductHandlingRequirements.Hazardous;
        if ((handlingRequirements & ~allRequirements) != 0)
            throw new ArgumentException("Product handling requirements are invalid.");
        if (handlingRequirements.HasFlag(ProductHandlingRequirements.Refrigerated) &&
            handlingRequirements.HasFlag(ProductHandlingRequirements.Frozen))
        {
            throw new ArgumentException("A product cannot be both refrigerated and frozen.");
        }

        ProductCategoryId = productCategoryId;
        NetWeight = netWeight;
        GrossWeight = grossWeight;
        WeightMeasurementUnitId = weightMeasurementUnitId;
        Length = length;
        Width = width;
        Height = height;
        DimensionMeasurementUnitId = dimensionMeasurementUnitId;
        TrackingPolicy = trackingPolicy;
        HandlingRequirements = handlingRequirements;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Product name is required.");
    }

    private static void ValidateBarcode(string? barcode)
    {
        if (barcode?.Trim().Length > 100)
            throw new ArgumentException("Product barcode cannot exceed 100 characters.");
    }

    private static void ValidatePurchasePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException(
                "Purchase price cannot be negative.");
    }

    private static void ValidateSalePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException(
                "Sale price cannot be negative.");
    }

    private static void ValidateWholesalePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException(
                "Wholesale price cannot be negative.");
    }

    private static void ValidatePriceOrder(
        decimal purchasePrice,
        decimal wholesalePrice,
        decimal salePrice)
    {
        if (wholesalePrice < purchasePrice ||
            salePrice < wholesalePrice)
        {
            throw new ArgumentException(
                "Prices must satisfy purchase price ≤ wholesale price ≤ retail price.");
        }
    }

    private static void ValidateInventoryConfiguration(
        ProductInventoryBehavior inventoryBehavior,
        UnitOfMeasure stockUnit)
    {
        if (!Enum.IsDefined(inventoryBehavior))
            throw new ArgumentException("Inventory behavior is invalid.");
        if (!Enum.IsDefined(stockUnit))
            throw new ArgumentException("Stock unit is invalid.");
    }

    private static void ValidateProductConfiguration(
        ProductType productType,
        ProductInventoryBehavior inventoryBehavior,
        UnitOfMeasure stockUnit,
        decimal purchasePrice,
        decimal wholesalePrice,
        decimal salePrice,
        bool isSellableInPos,
        bool isSellableInSales)
    {
        if (!Enum.IsDefined(productType))
            throw new ArgumentException("Product type is invalid.");

        ValidateInventoryConfiguration(inventoryBehavior, stockUnit);

        if (inventoryBehavior != MapInventoryBehavior(productType))
            throw new ArgumentException("Product type and inventory behavior do not match.");

        if (productType == ProductType.RawMaterial && (isSellableInPos || isSellableInSales))
            throw new ArgumentException("Raw materials cannot be enabled for direct sales.");

        if ((isSellableInPos || isSellableInSales) && wholesalePrice > salePrice)
            throw new ArgumentException("Wholesale price cannot be higher than retail price.");

        if (productType == ProductType.DirectSale && purchasePrice > salePrice)
            throw new ArgumentException("Purchase price cannot be higher than retail price for a direct-sale product.");
    }

    private static ProductInventoryBehavior MapInventoryBehavior(ProductType productType) =>
        productType == ProductType.PreparedToOrder
            ? ProductInventoryBehavior.PreparedToOrder
            : ProductInventoryBehavior.Stocked;

    private static string? NormalizeBarcode(string? barcode) =>
        string.IsNullOrWhiteSpace(barcode) ? null : barcode.Trim();
}
