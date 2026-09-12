namespace MiniStore.Application.DTOs.StockTransfers;

public class StockTransferItemDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string? Barcode { get; set; }

    public decimal Quantity { get; set; }
}