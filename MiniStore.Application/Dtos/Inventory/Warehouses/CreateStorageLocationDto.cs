using System.ComponentModel.DataAnnotations;using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Warehouses;

public class CreateStorageLocationDto
{
    [Range(1,int.MaxValue)]public int WarehouseId { get; set; }
    [Required,StringLength(100)]public string Code { get; set; } = string.Empty;
    [Required,StringLength(200)]public string Name { get; set; } = string.Empty;
    [StringLength(100)]public string? Barcode { get; set; }
    public int? ParentLocationId { get; set; }
    [Range(0,int.MaxValue)]public int Sequence { get; set; }
    [StringLength(100)]public string? Zone { get; set; }
    [StringLength(100)]public string? Aisle { get; set; }
    [StringLength(100)]public string? Rack { get; set; }
    [StringLength(100)]public string? Level { get; set; }
    [StringLength(100)]public string? Bin { get; set; }
    public StorageLocationType Type { get; set; } = StorageLocationType.Storage;
    [Range(typeof(decimal),"0","999999999999",ParseLimitsInInvariantCulture=true,ConvertValueInInvariantCulture=true)]public decimal? MaximumQuantity { get; set; }
    public bool IsReceivable { get; set; }
    public bool IsPickable { get; set; } = true;
    public bool IsReservable { get; set; } = true;
    public bool IsShippable { get; set; }
    public bool IsCountable { get; set; } = true;
}


public class EditStorageLocationDto
{
    [Range(1,int.MaxValue)]public int Id { get; set; }
    [Range(1,int.MaxValue)]public int WarehouseId { get; set; }
    [Required,StringLength(100)]public string Code { get; set; } = string.Empty;
    [Required,StringLength(200)]public string Name { get; set; } = string.Empty;
    [StringLength(100)]public string? Barcode { get; set; }
    public int? ParentLocationId { get; set; }
    [Range(0,int.MaxValue)]public int Sequence { get; set; }
    public bool IsReceivable { get; set; }
    public bool IsPickable { get; set; }
    public bool IsReservable { get; set; }
    public bool IsShippable { get; set; }
    public bool IsCountable { get; set; }
}
