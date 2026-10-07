namespace MiniStore.Application.DTOs.Inventory.Insights;

public enum InventoryActivityState
{
    Active,
    Slow,
    Dead,
    NeverIssued,
    NoMovementHistory
}

public sealed class InventoryInsightsQueryDto
{
    public int? WarehouseId { get; init; }
    public string Search { get; init; } = string.Empty;
    public InventoryActivityState? State { get; init; }
    public bool AttentionOnly { get; init; }
    public int SlowDays { get; init; } = 30;
    public int DeadDays { get; init; } = 90;
}

public sealed class InventoryInsightRowDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductCode { get; init; } = string.Empty;
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public decimal OnHand { get; init; }
    public decimal InventoryValue { get; init; }
    public DateTime? LastOutboundAt { get; init; }
    public DateTime? FirstInboundAt { get; init; }
    public DateTime? ActivityAnchorAt { get; init; }
    public int? DaysSinceActivity { get; init; }
    public InventoryActivityState State { get; init; }
}

public sealed class InventoryInsightsPageDto
{
    public InventoryInsightsQueryDto Query { get; init; } = new();
    public List<InventoryInsightRowDto> Rows { get; init; } = [];
    public List<(int Id, string Name)> Warehouses { get; init; } = [];
    public decimal TotalValue { get; init; }
    public decimal AttentionValue { get; init; }
    public int SlowCount { get; init; }
    public int DeadCount { get; init; }
    public int NeverIssuedCount { get; init; }
    public int NoMovementHistoryCount { get; init; }
}
