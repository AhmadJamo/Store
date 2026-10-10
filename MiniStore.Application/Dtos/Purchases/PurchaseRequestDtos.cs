using MiniStore.Domain.Entities;
namespace MiniStore.Application.DTOs.Purchases;
public sealed class CreatePurchaseRequestDto
{ public int WarehouseId{get;set;} public DateOnly NeededByDate{get;set;}=DateOnly.FromDateTime(DateTime.Today.AddDays(7)); public PurchaseRequestPriority Priority{get;set;}=PurchaseRequestPriority.Normal; public string Justification{get;set;}=string.Empty; public string? Notes{get;set;} public List<CreatePurchaseRequestLineDto> Lines{get;set;}=[]; }
public sealed class CreatePurchaseRequestLineDto
{ public int ProductId{get;set;} public int MeasurementUnitId{get;set;} public decimal RequestedQuantity{get;set;} public int? SuggestedSupplierId{get;set;} public string? Notes{get;set;} }
public sealed class PurchaseRequestDto
{ public int Id{get;init;} public string RequestNumber{get;init;}=string.Empty; public int WarehouseId{get;init;} public string WarehouseName{get;init;}=string.Empty; public DateOnly NeededByDate{get;init;} public PurchaseRequestPriority Priority{get;init;} public string Justification{get;init;}=string.Empty; public string? Notes{get;init;} public string? SourceType{get;init;} public string? SourceReference{get;init;} public PurchaseRequestStatus Status{get;init;} public string CreatedByUserId{get;init;}=string.Empty; public DateTime CreatedAtUtc{get;init;} public byte[] RowVersion{get;init;}=[]; public PurchaseApprovalInstanceDto? Approval{get;set;} public List<PurchaseRequestLineDto> Lines{get;init;}=[]; public List<PurchaseRequestHistoryDto> History{get;init;}=[]; }
public sealed class PurchaseRequestLineDto
{ public int ProductId{get;init;} public string ProductName{get;init;}=string.Empty; public string ProductCode{get;init;}=string.Empty; public int MeasurementUnitId{get;init;} public string UnitName{get;init;}=string.Empty; public decimal RequestedQuantity{get;init;} public decimal StockQuantity{get;init;} public string? SuggestedSupplierName{get;init;} public string? Notes{get;init;} }
public sealed class PurchaseRequestHistoryDto
{ public PurchaseRequestStatus FromStatus{get;init;} public PurchaseRequestStatus ToStatus{get;init;} public PurchaseRequestHistoryAction Action{get;init;} public string UserId{get;init;}=string.Empty; public string? Reason{get;init;} public DateTime CreatedAtUtc{get;init;} }
public sealed record PurchaseRequestOptionDto(int Id,string Label);
public sealed class PurchaseRequestCreatePageDto
{ public CreatePurchaseRequestDto Form{get;init;}=new(); public List<PurchaseRequestOptionDto> Warehouses{get;init;}=[]; public List<PurchaseRequestOptionDto> Products{get;init;}=[]; public List<PurchaseRequestOptionDto> Units{get;init;}=[]; public List<PurchaseRequestOptionDto> Suppliers{get;init;}=[]; }
