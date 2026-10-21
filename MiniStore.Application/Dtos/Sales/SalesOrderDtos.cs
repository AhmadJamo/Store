using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Sales;

public sealed record SalesOrderSummaryDto(int Id,string OrderNumber,string SourceQuotationNumber,string CustomerCode,string CustomerName,DateOnly OrderDate,string CurrencyCode,decimal TotalAmount,SalesOrderStatus Status);
public sealed record SalesOrderLineDto(int Id,int SourceQuotationLineId,string ProductCode,string ProductName,string UnitCode,string UnitName,decimal Quantity,decimal StockQuantity,decimal UnitPrice,decimal CompanyUnitPrice,decimal NetAmount,string PriceSourceCode,string? TaxName,decimal? TaxRatePercent,bool IsTaxInclusive,string TaxSourceCode,decimal TaxAmount,decimal TotalAmount,string? Notes);
public sealed record SalesOrderHistoryDto(SalesOrderStatus FromStatus,SalesOrderStatus ToStatus,SalesOrderHistoryAction Action,string UserId,string? Reason,DateTime CreatedAtUtc);

public sealed class SalesOrderDetailsDto
{
    public int Id{get;init;} public string OrderNumber{get;init;}=""; public int SourceQuotationId{get;init;} public string SourceQuotationNumber{get;init;}=""; public SalesOrderStatus Status{get;init;}
    public string CustomerCode{get;init;}=""; public string CustomerName{get;init;}=""; public string? CustomerLegalName{get;init;} public string? CustomerTaxNumber{get;init;} public string? Address{get;init;}
    public DateOnly OrderDate{get;init;} public string CurrencyCode{get;init;}=""; public decimal ExchangeRate{get;init;} public DateOnly ExchangeRateDate{get;init;} public string ExchangeRateSource{get;init;}=""; public int PaymentTermDays{get;init;} public string? Notes{get;init;}
    public decimal NetAmount{get;init;} public decimal TaxAmount{get;init;} public decimal TotalAmount{get;init;} public string CreatedByUserId{get;init;}=""; public DateTime CreatedAtUtc{get;init;} public string? ConfirmedByUserId{get;init;} public DateTime? ConfirmedAtUtc{get;init;} public byte[] RowVersion{get;init;}=[];
    public List<SalesOrderLineDto> Lines{get;init;}=[]; public List<SalesOrderHistoryDto> History{get;init;}=[];
    public SalesOrderReservationSummaryDto Reservation{get;set;}=new();
    public SalesDeliveryProgressDto Delivery{get;set;}=new();
    public CustomerInvoiceProgressDto Invoice{get;set;}=new();
    public SalesOrderApprovalDto? Approval{get;set;}
}

public sealed class ConvertSalesQuotationDto
{ [Range(1,int.MaxValue)] public int QuotationId{get;set;} public byte[] RowVersion{get;set;}=[]; }

public sealed class SalesOrderStatusCommandDto
{ [Range(1,int.MaxValue)] public int Id{get;set;} public byte[] RowVersion{get;set;}=[]; [StringLength(500)] public string? Reason{get;set;} }
