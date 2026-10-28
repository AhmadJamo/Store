using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;
namespace MiniStore.Application.DTOs.Purchases;
public sealed class LandedCostCreatePageDto
{
 public CreateLandedCostDto Form{get;init;}=new();public string ReceiptNumber{get;init;}=string.Empty;public string OrderNumber{get;init;}=string.Empty;public string SupplierName{get;init;}=string.Empty;public string WarehouseName{get;init;}=string.Empty;public List<LandedCostReceiptLineDto>Lines{get;init;}=[];
}
public sealed class LandedCostReceiptLineDto{public string Product{get;init;}=string.Empty;public decimal Quantity{get;init;}public decimal ReceiptValue{get;init;}}
public sealed class CreateLandedCostDto
{
 [Range(1,int.MaxValue)]public int GoodsReceiptId{get;set;}public DateOnly CostDate{get;set;}=DateOnly.FromDateTime(DateTime.Today);[Required,StringLength(100)]public string ExternalReference{get;set;}=string.Empty;[Range(1,2)]public LandedCostAllocationMethod AllocationMethod{get;set;}=LandedCostAllocationMethod.ByReceiptValue;[StringLength(1000)]public string?Notes{get;set;}[MinLength(1)]public List<CreateLandedCostComponentDto>Components{get;set;}=[];
}
public sealed class CreateLandedCostComponentDto{[Range(1,4)]public LandedCostType Type{get;set;}[Range(typeof(decimal),"0","999999999999",ParseLimitsInInvariantCulture=true,ConvertValueInInvariantCulture=true)]public decimal Amount{get;set;}[StringLength(250)]public string?Description{get;set;}}
public sealed class LandedCostDto
{
 public int Id{get;init;}public string CostNumber{get;init;}=string.Empty;public string ReceiptNumber{get;init;}=string.Empty;public string WarehouseName{get;init;}=string.Empty;public DateOnly CostDate{get;init;}public LandedCostStatus Status{get;init;}public string ExternalReference{get;init;}=string.Empty;public LandedCostAllocationMethod AllocationMethod{get;init;}public decimal TotalAmount{get;init;}public decimal CapitalizedAmount{get;init;}public decimal ExpensedAmount{get;init;}public string?Notes{get;init;}public List<LandedCostComponentDto>Components{get;init;}=[];public List<LandedCostAllocationDto>Allocations{get;init;}=[];
}
public sealed class LandedCostComponentDto{public LandedCostType Type{get;init;}public decimal Amount{get;init;}public string?Description{get;init;}}
public sealed class LandedCostAllocationDto{public string Product{get;init;}=string.Empty;public decimal AllocationBasis{get;init;}public decimal AllocatedAmount{get;init;}public decimal CapitalizedAmount{get;init;}public decimal ExpensedAmount{get;init;}public decimal AverageUnitCostBefore{get;init;}public decimal AverageUnitCostAfter{get;init;}}
