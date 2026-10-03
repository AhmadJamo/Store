using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.StockMovements;

public sealed class StockMovementRowDto
{
    public long Id { get; set; }
    public long LocationMovementId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public StockMovementType Type { get; set; }
    public StockMovementStatus Status { get; set; }
    public string? Reference { get; set; }
    public DateTime PostedAt { get; set; }
}

public sealed class StockMovementPageDto
{
    public List<StockMovementRowDto> Rows { get; set; } = [];
    public int LegacyMovementCount { get; set; }
    public int LinkedMovementCount { get; set; }
    public bool IsReconciled { get; set; }
}
