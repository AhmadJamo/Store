using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class SupplierIntegrationDashboardDto
{
    public List<SupplierIntegrationCredentialDto> Credentials { get; init; }=[];
    public List<SupplierOutboxDto> Outbox { get; init; }=[];
    public List<SupplierExternalSubmissionDto> Submissions { get; init; }=[];
    public List<SupplierExternalAuditDto> AuditEvents { get; init; }=[];
    public List<PurchaseRequestOptionDto> Suppliers { get; init; }=[];
}
public sealed class SupplierIntegrationCredentialDto
{
    public int Id{get;init;} public int SupplierId{get;init;} public string SupplierName{get;init;}=string.Empty; public string Name{get;init;}=string.Empty;
    public string KeyPrefix{get;init;}=string.Empty; public string Scopes{get;init;}=string.Empty; public string? CallbackUrl{get;init;} public bool IsActive{get;init;}
    public DateTime? ExpiresAtUtc{get;init;} public DateTime? LastUsedAtUtc{get;init;} public bool CanQueuePurchaseOrder{get;init;}=true; public byte[] RowVersion{get;init;}=[];
}
public sealed class CreateSupplierIntegrationCredentialDto
{
    public int SupplierId{get;set;} public string Name{get;set;}=string.Empty; public List<string> Scopes{get;set;}=[]; public string? CallbackUrl{get;set;}
    public string? CallbackBearerToken{get;set;} public DateTime? ExpiresAtUtc{get;set;}
}
public sealed record CreatedSupplierIntegrationCredentialDto(int Id,string ApiKey);
public sealed class CreateSupplierPortalAccessDto
{
    public SupplierExternalDocumentType DocumentType{get;set;} public int DocumentId{get;set;} public int SupplierId{get;set;} public int ValidDays{get;set;}=14;
}
public sealed record CreatedSupplierPortalAccessDto(int Id,string Token,DateTime ExpiresAtUtc);
public sealed class SupplierPortalAccessDto
{
    public int Id{get;init;} public int SupplierId{get;init;} public SupplierExternalDocumentType DocumentType{get;init;} public int DocumentId{get;init;}
    public string DocumentNumber{get;init;}=string.Empty; public DateTime ExpiresAtUtc{get;init;} public bool IsRevoked{get;init;} public DateTime? LastAccessedAtUtc{get;init;}
}
public sealed class SupplierPortalPageDto
{
    public string Token{get;init;}=string.Empty; public SupplierExternalDocumentType DocumentType{get;init;} public int DocumentId{get;init;}
    public string DocumentNumber{get;init;}=string.Empty; public string SupplierName{get;init;}=string.Empty; public string CurrencyCode{get;init;}=string.Empty;
    public DateOnly? ResponseDeadline{get;init;} public DateOnly? ExpectedDate{get;init;} public string? PaymentTerms{get;init;}
    public bool CanSubmitQuotation{get;init;} public bool CanConfirmOrder{get;init;} public bool CanSubmitDeliveryNotice{get;init;}
    public List<SupplierPortalLineDto> Lines{get;init;}=[];
}
public sealed class SupplierPortalLineDto
{
    public int Id{get;init;} public string ProductCode{get;init;}=string.Empty; public string ProductName{get;init;}=string.Empty; public string UnitName{get;init;}=string.Empty;
    public decimal Quantity{get;init;} public decimal? UnitPrice{get;init;}
}
public sealed class SupplierPortalQuotationDto
{
    public string Token{get;set;}=string.Empty; public string IdempotencyKey{get;set;}=string.Empty; public string CurrencyCode{get;set;}=string.Empty; public string? SupplierReference{get;set;}
    public DateOnly QuotationDate{get;set;}=DateOnly.FromDateTime(DateTime.Today); public DateOnly ValidUntilDate{get;set;}=DateOnly.FromDateTime(DateTime.Today.AddDays(14));
    public int LeadTimeDays{get;set;} public string? PaymentTerms{get;set;} public string? Notes{get;set;} public List<SupplierPortalQuotationLineDto> Lines{get;set;}=[];
}
public sealed class SupplierPortalQuotationLineDto{public int DocumentLineId{get;set;}public decimal Quantity{get;set;}public decimal UnitPrice{get;set;}public decimal DiscountPercent{get;set;}public decimal TaxPercent{get;set;}}
public sealed class SupplierExternalOrderConfirmationDto{public string Token{get;set;}=string.Empty;public string IdempotencyKey{get;set;}=string.Empty;public DateOnly ConfirmedExpectedDate{get;set;}public string ConfirmationReference{get;set;}=string.Empty;public string? Note{get;set;}}
public sealed class SupplierExternalDeliveryNoticeDto{public string Token{get;set;}=string.Empty;public string IdempotencyKey{get;set;}=string.Empty;public DateOnly DeliveryDate{get;set;}public string Reference{get;set;}=string.Empty;public string? Carrier{get;set;}public string? TrackingNumber{get;set;}public string? Note{get;set;}}
public sealed class SupplierExternalSubmissionDto{public int Id{get;init;}public string SupplierName{get;init;}=string.Empty;public SupplierExternalChannel Channel{get;init;}public SupplierExternalSubmissionType Type{get;init;}public string DocumentReference{get;init;}=string.Empty;public string? ExternalReference{get;init;}public string? ResultType{get;init;}public int? ResultId{get;init;}public DateTime CreatedAtUtc{get;init;}}
public sealed class SupplierExternalSubmissionDetailsDto{public int Id{get;init;}public string SupplierName{get;init;}=string.Empty;public SupplierExternalChannel Channel{get;init;}public SupplierExternalSubmissionType Type{get;init;}public SupplierExternalDocumentType DocumentType{get;init;}public int DocumentId{get;init;}public string IdempotencyKey{get;init;}=string.Empty;public string RequestHash{get;init;}=string.Empty;public string Payload{get;init;}=string.Empty;public string? ExternalReference{get;init;}public string Actor{get;init;}=string.Empty;public DateTime CreatedAtUtc{get;init;}}
public sealed class SupplierOutboxDto{public int Id{get;init;}public string SupplierName{get;init;}=string.Empty;public string MessageType{get;init;}=string.Empty;public string AggregateType{get;init;}=string.Empty;public int AggregateId{get;init;}public SupplierOutboxStatus Status{get;init;}public int AttemptCount{get;init;}public DateTime NextAttemptAtUtc{get;init;}public DateTime? DeliveredAtUtc{get;init;}public string? LastError{get;init;}}
public sealed class SupplierExternalAuditDto{public string SupplierName{get;init;}=string.Empty;public SupplierExternalChannel Channel{get;init;}public string Actor{get;init;}=string.Empty;public string Action{get;init;}=string.Empty;public string Outcome{get;init;}=string.Empty;public string CorrelationId{get;init;}=string.Empty;public string? RemoteAddress{get;init;}public DateTime CreatedAtUtc{get;init;}}
public sealed class QueueSupplierPurchaseOrderDto{public int PurchaseOrderId{get;set;}public int CredentialId{get;set;}}

public sealed class SupplierApiQuotationRequest
{
    public int PurchaseSourcingEventId{get;set;} public string CurrencyCode{get;set;}=string.Empty; public string? SupplierReference{get;set;} public DateOnly QuotationDate{get;set;}
    public DateOnly ValidUntilDate{get;set;} public int LeadTimeDays{get;set;} public string? PaymentTerms{get;set;} public string? Notes{get;set;} public List<SupplierPortalQuotationLineDto> Lines{get;set;}=[];
}
public sealed class SupplierApiOrderConfirmationRequest{public DateOnly ConfirmedExpectedDate{get;set;}public string ConfirmationReference{get;set;}=string.Empty;public string? Note{get;set;}}
public sealed class SupplierApiDeliveryNoticeRequest{public DateOnly DeliveryDate{get;set;}public string Reference{get;set;}=string.Empty;public string? Carrier{get;set;}public string? TrackingNumber{get;set;}public string? Note{get;set;}}
public sealed record SupplierExternalResultDto(bool Replayed,int SubmissionId,string? ResultType,int? ResultId);
