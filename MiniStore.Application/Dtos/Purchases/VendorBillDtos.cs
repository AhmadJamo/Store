using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class VendorBillCreatePageDto
{
    public int PurchaseOrderId { get; init; }
    public string PurchaseOrderNumber { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public string CurrencyCode { get; init; } = string.Empty;
    public CreateVendorBillDto Form { get; init; } = new();
    public List<VendorBillAvailableLineDto> Lines { get; init; } = [];
    public List<VendorBillTaxRateDto> TaxRates { get; init; } = [];
}

public sealed class VendorBillAvailableLineDto
{
    public int PurchaseOrderLineId { get; init; }
    public PurchaseBillingPolicy BillingPolicy { get; init; }
    public int? GoodsReceiptLineId { get; init; }
    public int? PurchaseServiceAcceptanceLineId { get; init; }
    public string SourceNumber { get; init; } = string.Empty;
    public DateOnly SourceDate { get; init; }
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty;
    public decimal RemainingQuantity { get; init; }
    public decimal SuggestedUnitPrice { get; init; }
    public int? SuggestedTaxRateId { get; init; }
}

public sealed class VendorBillTaxRateDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Rate { get; init; }
    public bool IsPriceInclusive { get; init; }
}

public sealed class CreateVendorBillDto
{
    public int PurchaseOrderId { get; set; }
    public string SupplierInvoiceNumber { get; set; } = string.Empty;
    public DateOnly BillDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public string? Notes { get; set; }
    public string? MatchOverrideReason { get; set; }
    public List<CreateVendorBillLineDto> Lines { get; set; } = [];
}

public sealed class VendorBillAttachmentInputDto
{
    public string FileName { get; init; } = string.Empty;
    public string? ContentType { get; init; }
    public byte[] Content { get; init; } = [];
}

public sealed class CreateVendorBillLineDto
{
    public int PurchaseOrderLineId { get; set; }
    public PurchaseBillingPolicy BillingPolicy { get; set; }
    public int? GoodsReceiptLineId { get; set; }
    public int? PurchaseServiceAcceptanceLineId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int? TaxRateId { get; set; }
}

public sealed class VendorBillDto
{
    public int Id { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public string PurchaseOrderNumber { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public string SupplierInvoiceNumber { get; init; } = string.Empty;
    public DateOnly BillDate { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public string CompanyCurrencyCode { get; init; } = string.Empty;
    public decimal ExchangeRate { get; init; }
    public DateOnly ExchangeRateDate { get; init; }
    public string ExchangeRateSource { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public VendorBillStatus Status { get; init; }
    public string? ApprovedByUserId { get; init; }
    public DateTime? ApprovedAtUtc { get; init; }
    public string? PostedByUserId { get; init; }
    public DateTime? PostedAtUtc { get; init; }
    public string? CancelledByUserId { get; init; }
    public DateTime? CancelledAtUtc { get; init; }
    public string? CancellationReason { get; init; }
    public byte[] RowVersion { get; init; } = [];
    public decimal NetAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal CreditedAmount { get; init; }
    public decimal OutstandingAmount { get; init; }
    public decimal ReceiptClearingAmount { get; init; }
    public decimal CompanyNetAmount { get; init; }
    public decimal CompanyTaxAmount { get; init; }
    public decimal CompanyTotalAmount { get; init; }
    public bool IsPaidInFull => Status == VendorBillStatus.Posted && OutstandingAmount <= 0 && CreditedAmount == 0;
    public bool IsSettledInFull => Status == VendorBillStatus.Posted && OutstandingAmount <= 0;
    public bool HasReceiptBasedLines { get; init; }
    public bool CanCreateCreditNote { get; init; }
    public PurchaseMatchRunDto? Match { get; init; }
    public List<VendorBillLineDto> Lines { get; init; } = [];
    public List<VendorBillAttachmentDto> Attachments { get; init; } = [];
    public List<VendorBillExtractionSuggestionDto> ExtractionSuggestions { get; init; } = [];
}

public sealed class VendorBillAttachmentDto
{
    public int Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long Size { get; init; }
    public DateTime UploadedAtUtc { get; init; }
}
public sealed class VendorBillExtractionSuggestionDto{public int Id{get;init;}public int VendorBillAttachmentId{get;init;}public string AttachmentName{get;init;}=string.Empty;public string ExtractionMethod{get;init;}=string.Empty;public string? SuggestedInvoiceNumber{get;init;}public DateOnly? SuggestedBillDate{get;init;}public decimal? SuggestedTotalAmount{get;init;}public string? SuggestedCurrencyCode{get;init;}public decimal ConfidencePercent{get;init;}public VendorBillExtractionStatus Status{get;init;}public string? ReviewNote{get;init;}public DateTime CreatedAtUtc{get;init;}}
public sealed class ReviewVendorBillExtractionDto{public int VendorBillId{get;init;}public int SuggestionId{get;init;}public bool Apply{get;init;}public string? SupplierInvoiceNumber{get;init;}public DateOnly? BillDate{get;init;}public decimal? TotalAmount{get;init;}public string? CurrencyCode{get;init;}public string? ReviewNote{get;init;}public byte[] RowVersion{get;init;}=[];}

public sealed class VendorBillLineDto
{
    public int Id { get; init; }
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty;
    public PurchaseBillingPolicy BillingPolicy { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TaxPercent { get; init; }
    public bool IsTaxInclusive { get; init; }
    public decimal NetAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal GrossAmount { get; init; }
    public decimal ReceiptClearingAmount { get; init; }
    public decimal CompanyNetAmount { get; init; }
    public decimal CompanyTaxAmount { get; init; }
    public decimal CompanyGrossAmount { get; init; }
}

public sealed class PurchaseMatchRunDto
{
    public decimal QuantityTolerancePercent { get; init; }
    public decimal PriceTolerancePercent { get; init; }
    public bool WasOverridden { get; init; }
    public string? OverrideReason { get; init; }
    public List<PurchaseMatchExceptionDto> Exceptions { get; init; } = [];
}
public sealed class PurchaseMatchExceptionDto
{
    public PurchaseMatchExceptionType Type { get; init; }
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public decimal ExpectedValue { get; init; }
    public decimal ActualValue { get; init; }
    public decimal VariancePercent { get; init; }
    public decimal TolerancePercent { get; init; }
}
