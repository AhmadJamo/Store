using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.StockTransfers;

public class StockTransferHistoryDto
{
    public int Id { get; set; }

    public StockTransferStatus FromStatus { get; set; }

    public StockTransferStatus ToStatus { get; set; }

    public StockTransferHistoryAction Action { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }

    public string UserName { get; set; } = string.Empty;
}