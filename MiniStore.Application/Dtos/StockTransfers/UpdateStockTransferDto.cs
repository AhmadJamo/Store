namespace MiniStore.Application.DTOs.StockTransfers;

public class UpdateStockTransferDto
{
    public int FromWarehouseId { get; set; }

    public int ToWarehouseId { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }

    public List<UpdateStockTransferItemDto> Items { get; set; } = new();
}

public class UpdateStockTransferItemDto
{
    public int ProductId { get; set; }

    public decimal Quantity { get; set; }
}