using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;
namespace MiniStore.Application.Dtos.Sales;

public sealed class SalesPriceListPageDto
{
 public List<SalesPriceListSummaryDto> PriceLists {get;init;}=[]; public SalesPriceListDetailsDto? Selected {get;init;}
 public SaveSalesPriceListDto ListForm {get;init;}=new(); public SaveSalesPriceRuleDto RuleForm {get;init;}=new();
 public EvaluateSalesPriceDto EvaluationForm {get;init;}=new(){PricingDate=DateOnly.FromDateTime(DateTime.Today)};
 public SalesPriceEvaluationDto? Evaluation {get;init;}
 public List<SalesPricingOptionDto> Products {get;init;}=[]; public List<SalesPricingOptionDto> Categories {get;init;}=[];
 public List<SalesPricingUnitOptionDto> Units {get;init;}=[]; public List<SalesPricingOptionDto> Customers {get;init;}=[];
 public List<SalesPricingTaxOptionDto> TaxRates {get;init;}=[]; public List<string> Currencies {get;init;}=[];
}
public sealed record SalesPricingOptionDto(int Id,string Name);
public sealed record SalesPricingUnitOptionDto(int Id,string Name,string Symbol,MeasurementDimension Dimension,decimal Factor);
public sealed record SalesPricingTaxOptionDto(int Id,string Name,decimal Rate,bool Inclusive);
public sealed record SalesPriceListSummaryDto(int Id,string Code,string Name,string CurrencyCode,bool IsActive,DateOnly? ValidFrom,DateOnly? ValidTo,byte[] RowVersion);
public sealed class SalesPriceListDetailsDto
{ public int Id{get;init;} public string Code{get;init;}="";public string Name{get;init;}="";public string CurrencyCode{get;init;}="";public decimal RoundingIncrement{get;init;}public bool IsActive{get;init;}public byte[] RowVersion{get;init;}=[];public List<SalesPriceRuleDto> Rules{get;init;}=[]; }
public sealed record SalesPriceRuleDto(int Id,SalesPriceRuleScope Scope,int? ProductId,int? ProductCategoryId,int? MeasurementUnitId,decimal MinimumQuantity,int Priority,DateOnly? EffectiveFrom,DateOnly? EffectiveTo,SalesPriceComputationType ComputationType,decimal Value,bool IsActive,byte[] RowVersion);
public sealed class SaveSalesPriceListDto
{
 [Required,StringLength(30,MinimumLength=2)] public string Code{get;set;}="";[Required,StringLength(150)]public string Name{get;set;}="";
 [Required,StringLength(3,MinimumLength=3)]public string CurrencyCode{get;set;}="JOD";public DateOnly? ValidFrom{get;set;}public DateOnly? ValidTo{get;set;}
 [Range(typeof(decimal),"0.000001","1000")]public decimal RoundingIncrement{get;set;}=0.01m;
}
public sealed class SaveSalesPriceRuleDto
{
 [Range(1,int.MaxValue)]public int SalesPriceListId{get;set;}public SalesPriceRuleScope Scope{get;set;}=SalesPriceRuleScope.AllProducts;
 public int? ProductId{get;set;}public int? ProductCategoryId{get;set;}public int? MeasurementUnitId{get;set;}
 [Range(typeof(decimal),"0.000001","999999999999")]public decimal MinimumQuantity{get;set;}=1m;[Range(1,9999)]public int Priority{get;set;}=100;
 public DateOnly? EffectiveFrom{get;set;}public DateOnly? EffectiveTo{get;set;}public SalesPriceComputationType ComputationType{get;set;}=SalesPriceComputationType.PercentageAdjustment;
 [Range(typeof(decimal),"-100","10000")]public decimal Value{get;set;}
}
public sealed class SalesPricingStatusDto { [Range(1,int.MaxValue)]public int Id{get;set;} public bool Active{get;set;} public byte[] RowVersion{get;set;}=[]; public int? PriceListId{get;set;} }
public sealed class EvaluateSalesPriceDto
{
 public int? PriceListId{get;set;}public int? CustomerId{get;set;}[Range(1,int.MaxValue)]public int ProductId{get;set;}[Range(1,int.MaxValue)]public int MeasurementUnitId{get;set;}
 [Range(typeof(decimal),"0.000001","999999999999")]public decimal Quantity{get;set;}=1m;public DateOnly PricingDate{get;set;}=DateOnly.FromDateTime(DateTime.Today);
 public SaleChannel Channel{get;set;}=SaleChannel.Wholesale;public int? TaxRateId{get;set;}
}
public sealed class SalesPriceEvaluationDto
{
 public string ProductName{get;init;}="";public string UnitName{get;init;}="";public decimal Quantity{get;init;}public decimal NormalizedStockQuantity{get;init;}
 public decimal ChannelFallbackCompanyUnitPrice{get;init;}public decimal UnroundedTransactionUnitPrice{get;init;}public decimal UnitPrice{get;init;}public decimal CompanyUnitPrice{get;init;}
 public string TransactionCurrency{get;init;}="";public string CompanyCurrency{get;init;}="";public decimal ExchangeRate{get;init;}public DateOnly ExchangeRateDate{get;init;}public string ExchangeRateSource{get;init;}="";
 public int? PriceListId{get;init;}public string? PriceListName{get;init;}public int? RuleId{get;init;}public string SourceCode{get;init;}="";
 public decimal LineNetAmount{get;init;}public decimal TaxAmount{get;init;}public decimal LineTotal{get;init;}public string TaxSource{get;init;}="";public int? TaxRateId{get;init;}public string? TaxName{get;init;}public decimal? TaxRate{get;init;}public bool TaxInclusive{get;init;}public string? TaxExemptionReason{get;init;}
}
