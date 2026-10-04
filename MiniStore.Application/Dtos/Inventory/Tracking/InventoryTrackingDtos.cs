using System.ComponentModel.DataAnnotations;using MiniStore.Domain.Entities;
namespace MiniStore.Application.DTOs.Inventory.Tracking;
public sealed class OpenTrackingAllocationDto
{
 [Range(1,int.MaxValue)] public int ProductId{get;set;} public ProductTrackingPolicy Policy{get;set;}
 [Required,StringLength(100)] public string SourceReference{get;set;}=""; public List<OpenTrackingAllocationLineDto> Lines{get;set;}=[];
}
public sealed class OpenTrackingAllocationLineDto
{
 [Range(1,int.MaxValue)] public int WarehouseId{get;set;} public int? StorageLocationId{get;set;}
 [Required,StringLength(100)] public string Identifier{get;set;}="";
 [Range(typeof(decimal),"0.000001","999999999999")] public decimal Quantity{get;set;}
 public DateOnly? ManufactureDate{get;set;} public DateOnly? ExpirationDate{get;set;}
}
public sealed class InventoryTrackingPageDto
{
 public OpenTrackingAllocationDto Input{get;set;}=new(); public List<(int Id,string Name)> Products{get;set;}=[];
 public List<(int Id,string Name)> Warehouses{get;set;}=[]; public List<(int Id,int WarehouseId,string Name)> Locations{get;set;}=[];
 public List<TrackingBalanceRowDto> Balances{get;set;}=[]; public List<TrackingTransactionRowDto> Transactions{get;set;}=[];
}
public sealed class TrackingBalanceRowDto
{
 public string Product{get;set;}="";public string Warehouse{get;set;}="";public string Location{get;set;}="";public ProductTrackingPolicy Policy{get;set;}
 public string Identifier{get;set;}="";public decimal Quantity{get;set;}public DateOnly? ExpirationDate{get;set;}public InventoryTrackingStatus Status{get;set;}
}
public sealed class TrackingTransactionRowDto
{
 public string Product{get;set;}="";public string Identifier{get;set;}="";public decimal Quantity{get;set;}public InventoryTrackingTransactionType Type{get;set;}
 public string Reference{get;set;}="";public DateTime CreatedAt{get;set;}
}
