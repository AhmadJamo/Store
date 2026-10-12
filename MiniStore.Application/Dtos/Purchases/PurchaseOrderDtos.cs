using MiniStore.Domain.Entities;
namespace MiniStore.Application.DTOs.Purchases;
public sealed class PurchaseOrderDto
{public int Id{get;init;}public string OrderNumber{get;init;}=string.Empty;public int? PurchaseAgreementId{get;init;}public string SupplierName{get;init;}=string.Empty;public string WarehouseName{get;init;}=string.Empty;public PurchaseOrderStatus Status{get;init;}public string CurrencyCode{get;init;}=string.Empty;public DateOnly OrderDate{get;init;}public DateOnly ExpectedDate{get;init;}public PurchaseOrderClosureType? ClosureType{get;init;}public string? ClosureReason{get;init;}public string? ClosedByUserId{get;init;}public DateTime? ClosedAtUtc{get;init;}public byte[] RowVersion{get;init;}=[];public PurchaseOrderProgressDto Progress{get;init;}=new();public List<PurchaseOrderLineDto> Lines{get;init;}=[];public List<PurchaseOrderLifecycleHistoryDto> History{get;init;}=[];}
public sealed class PurchaseOrderLineDto
{public int Id{get;init;}public string ProductCode{get;init;}=string.Empty;public string ProductName{get;init;}=string.Empty;public string UnitName{get;init;}=string.Empty;public decimal OrderedQuantity{get;init;}public decimal UnitPrice{get;init;}public decimal GrossAmount{get;init;}public decimal ReceivedQuantity{get;init;}public decimal ReturnedQuantity{get;init;}public decimal BilledQuantity{get;init;}public decimal RemainingToReceive{get;init;}public decimal UnsettledReceivedQuantity{get;init;}}
public sealed class PurchaseOrderProgressDto
{public List<PurchaseOrderLineProgressDto> Lines{get;init;}=[];public bool IsFullyReceived{get;init;}public bool IsFullySettled{get;init;}public bool CanCloseRemaining{get;init;}public bool CanReopen{get;init;}}
public sealed class PurchaseOrderLineProgressDto
{public int PurchaseOrderLineId{get;init;}public decimal ReceivedQuantity{get;init;}public decimal ReturnedQuantity{get;init;}public decimal BilledQuantity{get;init;}public decimal RemainingToReceive{get;init;}public decimal UnsettledReceivedQuantity{get;init;}}
public sealed class PurchaseOrderLifecycleHistoryDto
{public PurchaseOrderStatus FromStatus{get;init;}public PurchaseOrderStatus ToStatus{get;init;}public PurchaseOrderLifecycleAction Action{get;init;}public string UserId{get;init;}=string.Empty;public string? Reason{get;init;}public DateTime CreatedAtUtc{get;init;}}
