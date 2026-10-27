namespace MiniStore.Application.DTOs.Inventory.Balances;

public sealed class InventoryBalanceRowDto
{
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public bool IsUnassigned { get; set; }
    public decimal OnHand { get; set; }
    public decimal Reserved { get; set; }
    public decimal Available { get; set; }
}

public sealed class InventoryBalancePageDto
{
    public List<InventoryBalanceRowDto> Rows { get; set; } = [];
    public List<(int Id, string Name)> Warehouses { get; set; } = [];
    public int? WarehouseId { get; set; }
    public string Search { get; set; } = string.Empty;
}

public sealed record InventoryWarehouseOptionDto(int Id, string Name);

public sealed class InventoryBalancePagedResultDto
{
    public List<InventoryBalanceRowDto> Items { get; set; } = [];
    public List<InventoryWarehouseOptionDto> Warehouses { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
