namespace MiniStore.Application.DTOs.ProductStocks;

public class ProductStockDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
}