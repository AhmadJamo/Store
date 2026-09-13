using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Warehouses;

public class CreateStorageLocationDto
{
    public int WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Zone { get; set; }
    public string? Aisle { get; set; }
    public string? Rack { get; set; }
    public string? Level { get; set; }
    public string? Bin { get; set; }
    public StorageLocationType Type { get; set; } = StorageLocationType.Storage;
    public decimal? MaximumQuantity { get; set; }
}
