namespace MiniStore.Application.DTOs.ProductStocks;

public class CreateStockTransferDto
{
    public int ProductId { get; set; }

    public int FromWarehouseId { get; set; }

    public int ToWarehouseId { get; set; }

    public decimal Quantity { get; set; }

    public string? Reference { get; set; }
}