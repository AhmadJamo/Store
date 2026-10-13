using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class CreateSupplierQuotationDto
{
    public int PurchaseSourcingEventId { get; set; }
    public int SupplierId { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
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
{ public int Id { get; init; } public string QuotationNumber { get; init; }=string.Empty; public int SupplierId { get; init; } public string SupplierName { get; init; }=string.Empty; public SupplierQuotationStatus Status { get; init; } public string CurrencyCode { get; init; }=string.Empty; public string CompanyCurrencyCode { get; init; }=string.Empty; public decimal ComparisonExchangeRate { get; init; } public decimal CompanyGrossAmount { get; init; } public DateOnly ValidUntilDate { get; init; } public int LeadTimeDays { get; init; } public decimal NetAmount { get; init; } public decimal TaxAmount { get; init; } public decimal GrossAmount { get; init; } public List<SupplierQuotationLineDto> Lines { get; init; }=[]; }
public sealed class SupplierQuotationLineDto
{ public int Id { get; init; } public int PurchaseSourcingLineId { get; init; } public string ProductCode { get; init; }=string.Empty; public string ProductName { get; init; }=string.Empty; public string UnitName { get; init; }=string.Empty; public decimal QuotedQuantity { get; init; } public decimal UnitPrice { get; init; } public decimal DiscountPercent { get; init; } public decimal TaxPercent { get; init; } public decimal GrossAmount { get; init; } public decimal CompanyGrossAmount { get; init; } }
public sealed class SupplierQuotationCreatePageDto
{ public CreateSupplierQuotationDto Form { get; init; }=new(); public PurchaseSourcingDto? SourcingEvent { get; init; } public List<PurchaseRequestOptionDto> Suppliers { get; init; }=[]; public string CompanyCurrencyCode { get; init; }="JOD"; public List<string> AvailableCurrencyCodes { get; init; }=[]; }

public sealed class SupplierQuotationComparisonPageDto
{
    public int SourcingEventId { get; init; }
    public int? AwardedQuotationId { get; init; }
    public int? PurchaseOrderId { get; init; }
    public int? PurchaseAgreementId { get; init; }
    public CreatePurchaseQuotationAwardDto Form { get; init; } = new();
    public List<PurchaseSourcingLineDto> SourcingLines { get; init; } = [];
    public List<PurchaseQuotationAwardAllocationDto> AwardAllocations { get; init; } = [];
    public List<PurchaseRequestOptionDto> PurchaseOrders { get; init; } = [];
    public List<SupplierQuotationDto> Quotations { get; init; } = [];
    public string CompanyCurrencyCode { get; init; } = string.Empty;
    public bool CanAward => !AwardedQuotationId.HasValue;
    public bool CanCreatePurchaseDocument => AwardedQuotationId.HasValue &&
        PurchaseOrders.Count == 0 && !PurchaseAgreementId.HasValue;
    public bool CanCreatePurchaseAgreement { get; init; }
}

public sealed class CreatePurchaseQuotationAwardDto
{
    public int SourcingEventId { get; set; }
    public List<CreatePurchaseQuotationAwardAllocationDto> Allocations { get; set; } = [];
}

public sealed class CreatePurchaseQuotationAwardAllocationDto
{
    public int SupplierQuotationLineId { get; set; }
    public decimal AwardedQuantity { get; set; }
    public string? Reason { get; set; }
}

public sealed class PurchaseQuotationAwardAllocationDto
{
    public int PurchaseSourcingLineId { get; init; }
    public int SupplierQuotationId { get; init; }
    public int SupplierQuotationLineId { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public decimal AwardedQuantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal GrossAmount { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public int LeadTimeDays { get; init; }
    public string Reason { get; init; } = string.Empty;
}
