using System.ComponentModel.DataAnnotations;
namespace MiniStore.Application.DTOs.Warehouses;
public sealed class WarehouseLocationApiPageDto{public WarehouseInventorySearchDto Result{get;set;}=new();public StorageLocationPageOptionsDto Options{get;set;}=new();}
public sealed class WarehouseLocationEditApiPageDto{public EditStorageLocationDto Input{get;set;}=new();public List<StorageLocationOptionDto> ParentLocations{get;set;}=[];}
public sealed record PutawayTargetOptionDto(int Id,string Name);
public sealed record UnassignedLocationOptionDto(int Id,int WarehouseId,string Code);
public sealed class UnassignedStockPageDto
{
 public List<UnassignedStockDto> Rows{get;set;}=[];public List<StorageLocationWarehouseOptionDto> Warehouses{get;set;}=[];public List<UnassignedLocationOptionDto> Locations{get;set;}=[];
 public List<PutawayTargetOptionDto> Products{get;set;}=[];public List<PutawayTargetOptionDto> Categories{get;set;}=[];public List<PutawayRuleDto> Rules{get;set;}=[];
}
public sealed class AssignUnassignedStockDto
{
 [Range(1,int.MaxValue)]public int ProductId{get;set;}[Range(1,int.MaxValue)]public int WarehouseId{get;set;}[Range(1,int.MaxValue)]public int StorageLocationId{get;set;}
 [Range(typeof(decimal),"0.000001","999999999999",ParseLimitsInInvariantCulture=true,ConvertValueInInvariantCulture=true)]public decimal Quantity{get;set;}
 [Required,StringLength(100,MinimumLength=16)]public string IdempotencyKey{get;set;}=Guid.NewGuid().ToString("N");
}
public sealed class SetPutawayRuleActiveDto{public bool Active{get;set;}}
