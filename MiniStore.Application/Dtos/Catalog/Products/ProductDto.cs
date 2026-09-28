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
}
