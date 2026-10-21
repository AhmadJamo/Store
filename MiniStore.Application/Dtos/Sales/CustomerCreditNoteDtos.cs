using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;
namespace MiniStore.Application.Dtos.Sales;

public sealed record CustomerCreditNoteSummaryDto(int Id,string Number,string InvoiceNumber,string CustomerName,DateOnly CreditDate,string CurrencyCode,decimal TotalAmount,CustomerCreditNoteStatus Status);
public sealed record CustomerCreditNoteLineDto(string ReceiptNumber,string ProductCode,string ProductName,string UnitName,decimal Quantity,decimal StockQuantity,decimal NetAmount,decimal TaxAmount,decimal GrossAmount);
public sealed class CustomerCreditNoteDetailsDto
{
 public int Id{get;init;}public string Number{get;init;}=string.Empty;public int InvoiceId{get;init;}public string InvoiceNumber{get;init;}=string.Empty;public string CustomerName{get;init;}=string.Empty;public DateOnly CreditDate{get;init;}public string CurrencyCode{get;init;}=string.Empty;public string Reason{get;init;}=string.Empty;public CustomerCreditNoteStatus Status{get;init;}public decimal NetAmount{get;init;}public decimal TaxAmount{get;init;}public decimal TotalAmount{get;init;}public decimal CompanyTotalAmount{get;init;}public int? JournalEntryId{get;init;}public string? JournalEntryNumber{get;init;}public List<CustomerCreditNoteLineDto> Lines{get;init;}=[];
}
public sealed class CustomerCreditNoteAvailableLineDto
{
 public int CustomerInvoiceLineId{get;init;}public int CustomerReturnReceiptLineId{get;init;}public string ReceiptNumber{get;init;}=string.Empty;public string ProductCode{get;init;}=string.Empty;public string ProductName{get;init;}=string.Empty;public string UnitName{get;init;}=string.Empty;public decimal ReturnedStockQuantity{get;init;}public decimal AlreadyCreditedStockQuantity{get;init;}public decimal RemainingStockQuantity{get;init;}public decimal RemainingQuantity{get;init;}
}
public sealed class CreateCustomerCreditNoteLineDto
{
 [Range(1,int.MaxValue)]public int CustomerInvoiceLineId{get;set;}[Range(1,int.MaxValue)]public int CustomerReturnReceiptLineId{get;set;}[Range(typeof(decimal),"0","999999999999.999999")]public decimal Quantity{get;set;}
}
public sealed class CreateCustomerCreditNoteDto
{
 [Range(1,int.MaxValue)]public int CustomerInvoiceId{get;set;}[Required,StringLength(100)]public string RequestKey{get;set;}=Guid.NewGuid().ToString("N");public DateOnly CreditDate{get;set;}=DateOnly.FromDateTime(DateTime.Today);[Required,StringLength(1000)]public string Reason{get;set;}=string.Empty;public List<CreateCustomerCreditNoteLineDto> Lines{get;set;}=[];
}
public sealed class CustomerCreditNoteCreatePageDto
{
 public int InvoiceId{get;init;}public string InvoiceNumber{get;init;}=string.Empty;public string CustomerName{get;init;}=string.Empty;public string CurrencyCode{get;init;}=string.Empty;public List<CustomerCreditNoteAvailableLineDto> Lines{get;init;}=[];public CreateCustomerCreditNoteDto Form{get;init;}=new();
}
