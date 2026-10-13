namespace MiniStore.Application.DTOs.Products;

public class UpdateProductDto : ProductLogisticsInputDto
{
    public string Name { get; set; } = string.Empty;

    public string? Barcode { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal SalePrice { get; set; }

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
