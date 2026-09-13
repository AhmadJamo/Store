namespace MiniStore.Application.DTOs.ProductStocks;

public class CreateProductStockDto
{
    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public decimal Quantity { get; set; }
}