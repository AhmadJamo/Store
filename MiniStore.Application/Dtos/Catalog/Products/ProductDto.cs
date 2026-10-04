namespace MiniStore.Application.DTOs.Products;

public class ProductDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ProductCode { get; set; } = string.Empty;

    public string? Barcode { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal SalePrice { get; set; }

    public decimal WholesalePrice { get; set; }

    public MiniStore.Domain.Entities.ProductInventoryBehavior InventoryBehavior { get; set; }

    public MiniStore.Domain.Entities.UnitOfMeasure StockUnit { get; set; }

    public int? MeasurementUnitId { get; set; }

    public bool AllowNegativeRecipeConsumption { get; set; }

    public MiniStore.Domain.Entities.ProductType ProductType { get; set; }

    public bool IsSellableInPos { get; set; }

    public bool IsSellableInSales { get; set; }

    public bool IsActive { get; set; }

    public int? ProductCategoryId { get; set; }
    public string? ProductCategoryName { get; set; }
    public int? ProductTemplateId { get; set; }
    public string? VariantLabel { get; set; }
    public decimal? NetWeight { get; set; }
    public decimal? GrossWeight { get; set; }
    public int? WeightMeasurementUnitId { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public int? DimensionMeasurementUnitId { get; set; }
    public MiniStore.Domain.Entities.ProductTrackingPolicy TrackingPolicy { get; set; }
    public int? DefaultShelfLifeDays { get; set; }
    public bool RequireExpirationDate { get; set; }
    public int ExpirationWarningDays { get; set; }
    public MiniStore.Domain.Entities.ProductHandlingRequirements HandlingRequirements { get; set; }
}
