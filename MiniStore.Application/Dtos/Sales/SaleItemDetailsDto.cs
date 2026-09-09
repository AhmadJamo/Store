namespace MiniStore.Application.DTOs.Sale;

public class SaleItemDetailsDto
{
    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal SalePrice { get; set; }

    public decimal Total { get; set; }
}