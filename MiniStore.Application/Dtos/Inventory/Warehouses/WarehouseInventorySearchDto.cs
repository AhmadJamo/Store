using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Warehouses;

public class WarehouseInventorySearchDto
{
    public List<StorageLocationRowDto> Locations { get; set; } = [];
    public Dictionary<int, int> LocationDepths { get; set; } = [];
    public List<WarehouseProductSearchRowDto> Products { get; set; } = [];
}

public sealed class StorageLocationRowDto
{
    public int Id { get; init; }
    public int WarehouseId { get; init; }
    public int? ParentLocationId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Zone { get; init; }
    public string? Aisle { get; init; }
    public string? Rack { get; init; }
    public string? Level { get; init; }
    public string? Bin { get; init; }
    public StorageLocationType Type { get; init; }
    public StorageLocationStatus Status { get; init; }
    public bool IsReceivable { get; init; }
    public bool IsPickable { get; init; }
    public bool IsReservable { get; init; }
    public bool IsShippable { get; init; }
    public bool IsCountable { get; init; }
}

public sealed record StorageLocationOptionDto(int Id, int WarehouseId, string Label);

public sealed record StorageLocationWarehouseOptionDto(int Id, string Name);

public sealed class StorageLocationPageOptionsDto
{
    public List<StorageLocationWarehouseOptionDto> Warehouses { get; init; } = [];
    public List<StorageLocationOptionDto> ParentLocations { get; init; } = [];
}

public class WarehouseProductSearchRowDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Location { get; set; } = "Unassigned";
}
