using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;
namespace MiniStore.Application.DTOs.Inventory.Adjustments;
public sealed class CreateInventoryAdjustmentDto
{
    [Range(1, int.MaxValue)] public int WarehouseId { get; set; }
    public int? StorageLocationId { get; set; }
    public bool IsBlindCount { get; set; }
    [Required, StringLength(250)] public string Reason { get; set; } = string.Empty;
    public List<int> ProductIds { get; set; } = [];
}
public sealed class RecordInventoryCountsDto { public List<InventoryCountInputDto> Lines { get; set; } = []; }
public sealed class InventoryCountInputDto { public int ProductId { get; set; } [Range(typeof(decimal), "0", "999999999999")] public decimal CountedQuantity { get; set; } }
public class InventoryAdjustmentRowDto
{
    public long Id { get; set; } public string Number { get; set; } = string.Empty; public string Warehouse { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty; public InventoryAdjustmentStatus Status { get; set; }
    public bool IsBlindCount { get; set; } public string Reason { get; set; } = string.Empty; public DateTime CreatedAt { get; set; }
    public int LineCount { get; set; }
}
public sealed class InventoryAdjustmentLineDto
{
    public int ProductId { get; set; } public string ProductName { get; set; } = string.Empty;
    public decimal? ExpectedQuantity { get; set; } public decimal? CountedQuantity { get; set; } public decimal? VarianceQuantity { get; set; }
}
public sealed class InventoryAdjustmentDetailsDto : InventoryAdjustmentRowDto { public List<InventoryAdjustmentLineDto> Lines { get; set; } = []; }
public sealed class InventoryAdjustmentCreatePageDto
{
    public CreateInventoryAdjustmentDto Input { get; set; } = new(); public List<(int Id, string Name)> Warehouses { get; set; } = [];
    public List<(int Id, int WarehouseId, string Name)> Locations { get; set; } = []; public List<(int Id, string Name)> Products { get; set; } = [];
}
