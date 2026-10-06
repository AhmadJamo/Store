namespace MiniStore.Application.DTOs.Warehouses;

public sealed class CreatePutawayRuleDto
{
    public int WarehouseId { get; set; }
    public int StorageLocationId { get; set; }
    public int? ProductId { get; set; }
    public int? ProductCategoryId { get; set; }
    public int Priority { get; set; } = 100;
}

public sealed class PutawayRuleDto
{
    public int Id { get; init; }
    public int WarehouseId { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public int StorageLocationId { get; init; }
    public string StorageLocationCode { get; init; } = string.Empty;
    public string Target { get; init; } = string.Empty;
    public int Priority { get; init; }
    public bool IsActive { get; init; }
}
