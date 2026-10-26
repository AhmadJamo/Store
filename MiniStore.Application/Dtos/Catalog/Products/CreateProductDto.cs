namespace MiniStore.Application.DTOs.Products;

public class CreateProductDto : ProductLogisticsInputDto
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.StringLength(100)]
    public string? Barcode { get; set; }

    [System.ComponentModel.DataAnnotations.Range(typeof(decimal), "0", "999999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal PurchasePrice { get; set; }

    [System.ComponentModel.DataAnnotations.Range(typeof(decimal), "0", "999999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal SalePrice { get; set; }

    [System.ComponentModel.DataAnnotations.Range(typeof(decimal), "0", "999999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal WholesalePrice { get; set; }

    public MiniStore.Domain.Entities.ProductInventoryBehavior InventoryBehavior { get; set; } =
        MiniStore.Domain.Entities.ProductInventoryBehavior.Stocked;

    public MiniStore.Domain.Entities.UnitOfMeasure StockUnit { get; set; } =
        MiniStore.Domain.Entities.UnitOfMeasure.Piece;

    [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
    public int MeasurementUnitId { get; set; }

    public bool AllowNegativeRecipeConsumption { get; set; }

    public MiniStore.Domain.Entities.ProductType ProductType { get; set; } =
        MiniStore.Domain.Entities.ProductType.DirectSale;

    public bool IsSellableInPos { get; set; } = true;

    public bool IsSellableInSales { get; set; } = true;

    public bool IsActive { get; set; } = true;

    public List<int> SupplierIds { get; set; } = [];

    public MiniStore.Domain.Entities.PurchaseBillingPolicy PurchaseBillingPolicy { get; set; } =
        MiniStore.Domain.Entities.PurchaseBillingPolicy.ReceivedQuantity;

    public int? PurchaseExpenseAccountId { get; set; }

}
