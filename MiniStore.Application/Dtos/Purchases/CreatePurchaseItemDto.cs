namespace MiniStore.Application.DTOs.Purchases;

public class CreatePurchaseItemDto
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }

    public decimal Quantity { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal DiscountAmount { get; set; }

    public int? TaxRateId { get; set; }
    public string? LotNumber { get; set; }
    public string? SerialNumbers { get; set; }
    public DateOnly? ManufactureDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
}
