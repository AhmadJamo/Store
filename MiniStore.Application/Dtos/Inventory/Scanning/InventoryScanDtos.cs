namespace MiniStore.Application.DTOs.Inventory.Scanning;

public enum InventoryScanStatus
{
    Empty,
    Resolved,
    NotFound,
    Ambiguous
}

public sealed class InventoryScanQueryDto
{
    public string ProductScan { get; init; } = string.Empty;
    public string LocationScan { get; init; } = string.Empty;
}

public sealed class InventoryScanResultDto
{
    public string NormalizedValue { get; init; } = string.Empty;
    public InventoryScanStatus Status { get; init; }
    public int? EntityId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string? SecondaryText { get; init; }
}

public sealed class InventoryScanBalanceDto
{
    public string WarehouseName { get; init; } = string.Empty;
    public string LocationName { get; init; } = string.Empty;
    public bool IsUnassigned { get; init; }
    public decimal OnHand { get; init; }
    public decimal Reserved { get; init; }
    public decimal Available { get; init; }
}

public sealed class InventoryScanPageDto
{
    public InventoryScanQueryDto Query { get; init; } = new();
    public InventoryScanResultDto Product { get; init; } = new();
    public InventoryScanResultDto Location { get; init; } = new();
    public List<InventoryScanBalanceDto> Balances { get; init; } = [];
}
