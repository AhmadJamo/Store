using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.ProductStocks;

public class StockTransactionDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public StockTransactionType Type { get; set; }

    public string? Reference { get; set; }

    public DateTime CreatedAt { get; set; }
    public decimal QuantityBefore { get; set; }
    public decimal QuantityAfter { get; set; }
    public decimal AverageUnitCostBefore { get; set; }
    public decimal AverageUnitCostAfter { get; set; }
    public decimal InventoryValueBefore { get; set; }
    public decimal InventoryValueAfter { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TransactionValue { get; set; }
    public decimal CostVariance { get; set; }
}
