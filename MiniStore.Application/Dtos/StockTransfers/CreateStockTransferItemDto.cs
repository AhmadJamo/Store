namespace MiniStore.Application.DTOs.StockTransfers;

public class CreateStockTransferItemDto
{
    public int ProductId { get; set; }

    public decimal Quantity { get; set; }
}