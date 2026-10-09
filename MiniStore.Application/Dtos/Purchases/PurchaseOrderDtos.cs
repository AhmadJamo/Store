using MiniStore.Domain.Entities;
namespace MiniStore.Application.DTOs.Purchases;
public sealed class PurchaseOrderDto
{public int Id{get;init;}public string OrderNumber{get;init;}=string.Empty;public string SupplierName{get;init;}=string.Empty;public string WarehouseName{get;init;}=string.Empty;public PurchaseOrderStatus Status{get;init;}public string CurrencyCode{get;init;}=string.Empty;public DateOnly OrderDate{get;init;}public DateOnly ExpectedDate{get;init;}public byte[] RowVersion{get;init;}=[];public List<PurchaseOrderLineDto> Lines{get;init;}=[];}
public sealed class PurchaseOrderLineDto
{public string ProductCode{get;init;}=string.Empty;public string ProductName{get;init;}=string.Empty;public string UnitName{get;init;}=string.Empty;public decimal OrderedQuantity{get;init;}public decimal UnitPrice{get;init;}public decimal GrossAmount{get;init;}}
