using MiniStore.Domain.Entities;

using System.ComponentModel.DataAnnotations;

namespace MiniStore.Application.DTOs.ProductStocks;

public class CreateStockTransactionDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int WarehouseId { get; set; }

    [Range(typeof(decimal), "0.001", "999999999999999.999")]
    public decimal Quantity { get; set; }

    public StockTransactionType Type { get; set; }

    [Required]
    [StringLength(100)]
    public string? Reference { get; set; }
}
