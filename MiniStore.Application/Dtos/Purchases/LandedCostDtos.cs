using MiniStore.Domain.Entities;
namespace MiniStore.Application.DTOs.Purchases;
public sealed class LandedCostCreatePageDto
{
 public CreateLandedCostDto Form{get;init;}=new();public string ReceiptNumber{get;init;}=string.Empty;public string OrderNumber{get;init;}=string.Empty;public string SupplierName{get;init;}=string.Empty;public string WarehouseName{get;init;}=string.Empty;public List<LandedCostReceiptLineDto>Lines{get;init;}=[];
}
public sealed class LandedCostReceiptLineDto{public string Product{get;init;}=string.Empty;public decimal Quantity{get;init;}public decimal ReceiptValue{get;init;}}
public sealed class CreateLandedCostDto
{
 public int GoodsReceiptId{get;set;}public DateOnly CostDate{get;set;}=DateOnly.FromDateTime(DateTime.Today);public string ExternalReference{get;set;}=string.Empty;public LandedCostAllocationMethod AllocationMethod{get;set;}=LandedCostAllocationMethod.ByReceiptValue;public string?Notes{get;set;}public List<CreateLandedCostComponentDto>Components{get;set;}=[];
}
public sealed class CreateLandedCostComponentDto{public LandedCostType Type{get;set;}public decimal Amount{get;set;}public string?Description{get;set;}}
public sealed class LandedCostDto
{
 public int Id{get;init;}public string CostNumber{get;init;}=string.Empty;public string ReceiptNumber{get;init;}=string.Empty;public string WarehouseName{get;init;}=string.Empty;public DateOnly CostDate{get;init;}public string ExternalReference{get;init;}=string.Empty;public LandedCostAllocationMethod AllocationMethod{get;init;}public decimal TotalAmount{get;init;}public decimal CapitalizedAmount{get;init;}public decimal ExpensedAmount{get;init;}public string?Notes{get;init;}public List<LandedCostComponentDto>Components{get;init;}=[];public List<LandedCostAllocationDto>Allocations{get;init;}=[];
}
public sealed class LandedCostComponentDto{public LandedCostType Type{get;init;}public decimal Amount{get;init;}public string?Description{get;init;}}
public sealed class LandedCostAllocationDto{public string Product{get;init;}=string.Empty;public decimal AllocationBasis{get;init;}public decimal AllocatedAmount{get;init;}public decimal CapitalizedAmount{get;init;}public decimal ExpensedAmount{get;init;}public decimal AverageUnitCostBefore{get;init;}public decimal AverageUnitCostAfter{get;init;}}
