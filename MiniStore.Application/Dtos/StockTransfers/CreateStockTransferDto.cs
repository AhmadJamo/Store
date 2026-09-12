namespace MiniStore.Application.DTOs.StockTransfers;

public class CreateStockTransferDto
{
    public int FromWarehouseId { get; set; }

    public int ToWarehouseId { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }

    public List<CreateStockTransferItemDto> Items { get; set; } = new();
}