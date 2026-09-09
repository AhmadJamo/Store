namespace MiniStore.Application.DTOs.Purchases;

public class PurchaseItemDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }
        = string.Empty;

    public decimal Quantity { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal Total { get; set; }
}