using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Warehouses;

public class CreateStorageLocationDto
{
    public int WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public int? ParentLocationId { get; set; }
    public int Sequence { get; set; }
    public string? Zone { get; set; }
    public string? Aisle { get; set; }
    public string? Rack { get; set; }
    public string? Level { get; set; }
    public string? Bin { get; set; }
    public StorageLocationType Type { get; set; } = StorageLocationType.Storage;
    public decimal? MaximumQuantity { get; set; }
    public bool IsReceivable { get; set; }
    public bool IsPickable { get; set; } = true;
    public bool IsReservable { get; set; } = true;
    public bool IsShippable { get; set; }
    public bool IsCountable { get; set; } = true;
}


public class EditStorageLocationDto
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public int? ParentLocationId { get; set; }
    public int Sequence { get; set; }
    public bool IsReceivable { get; set; }
    public bool IsPickable { get; set; }
    public bool IsReservable { get; set; }
    public bool IsShippable { get; set; }
    public bool IsCountable { get; set; }
}
