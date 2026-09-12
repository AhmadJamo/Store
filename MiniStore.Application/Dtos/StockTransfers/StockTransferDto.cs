using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.StockTransfers;

public class StockTransferDto
{
    public int Id { get; set; }

    public string TransferNumber { get; set; } = string.Empty;

    public int FromWarehouseId { get; set; }

    public string FromWarehouseName { get; set; } = string.Empty;

    public int ToWarehouseId { get; set; }

    public string ToWarehouseName { get; set; } = string.Empty;

    public StockTransferStatus Status { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }

    public int ItemsCount { get; set; }
}