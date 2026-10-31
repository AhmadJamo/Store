using System.ComponentModel.DataAnnotations;namespace MiniStore.Application.DTOs.Warehouses;

public sealed class CreatePutawayRuleDto
{
    [Range(1,int.MaxValue)]public int WarehouseId { get; set; }
    [Range(1,int.MaxValue)]public int StorageLocationId { get; set; }
    public int? ProductId { get; set; }
    public int? ProductCategoryId { get; set; }
    [Range(1,9999)]public int Priority { get; set; } = 100;
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
