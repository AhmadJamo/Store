namespace MiniStore.Application.DTOs.Purchases;

public class PurchaseItemDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public int? WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;

    public string ProductName { get; set; }
        = string.Empty;

    public decimal Quantity { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal DiscountAmount { get; set; }

    public int? TaxRateId { get; set; }

    public decimal Total { get; set; }
}
