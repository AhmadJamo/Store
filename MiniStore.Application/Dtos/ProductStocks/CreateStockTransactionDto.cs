using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.ProductStocks;

public class CreateStockTransactionDto
{
    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public decimal Quantity { get; set; }

    public StockTransactionType Type { get; set; }

    public string? Reference { get; set; }
}