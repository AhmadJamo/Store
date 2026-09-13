namespace MiniStore.Application.DTOs.Purchases;

public class CreatePurchaseItemDto
{
    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal DiscountAmount { get; set; }

    public int? TaxRateId { get; set; }
}
