using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class CreateSupplierQuotationDto
{
    public int PurchaseSourcingEventId { get; set; }
    public int SupplierId { get; set; }
    public string? SupplierReference { get; set; }
    public DateOnly QuotationDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly ValidUntilDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(14));
    public int LeadTimeDays { get; set; }
    public string? PaymentTerms { get; set; }
    public string? Notes { get; set; }
    public List<CreateSupplierQuotationLineDto> Lines { get; set; } = [];
}
public sealed class CreateSupplierQuotationLineDto
{ public int PurchaseSourcingLineId { get; set; } public decimal QuotedQuantity { get; set; } public decimal UnitPrice { get; set; } public decimal DiscountPercent { get; set; } public decimal TaxPercent { get; set; } }
public sealed class SupplierQuotationDto
{ public int Id { get; init; } public string QuotationNumber { get; init; }=string.Empty; public int SupplierId { get; init; } public string SupplierName { get; init; }=string.Empty; public SupplierQuotationStatus Status { get; init; } public string CurrencyCode { get; init; }=string.Empty; public DateOnly ValidUntilDate { get; init; } public int LeadTimeDays { get; init; } public decimal NetAmount { get; init; } public decimal TaxAmount { get; init; } public decimal GrossAmount { get; init; } public List<SupplierQuotationLineDto> Lines { get; init; }=[]; }
public sealed class SupplierQuotationLineDto
{ public int PurchaseSourcingLineId { get; init; } public string ProductCode { get; init; }=string.Empty; public string ProductName { get; init; }=string.Empty; public string UnitName { get; init; }=string.Empty; public decimal QuotedQuantity { get; init; } public decimal UnitPrice { get; init; } public decimal DiscountPercent { get; init; } public decimal TaxPercent { get; init; } public decimal GrossAmount { get; init; } }
public sealed class SupplierQuotationCreatePageDto
{ public CreateSupplierQuotationDto Form { get; init; }=new(); public PurchaseSourcingDto? SourcingEvent { get; init; } public List<PurchaseRequestOptionDto> Suppliers { get; init; }=[]; public string CurrencyCode { get; init; }="JOD"; }
