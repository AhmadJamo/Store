using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;

namespace MiniStore.Web.Controllers;

[AllowAnonymous,EnableRateLimiting("supplier-portal")]
public sealed class SupplierPortalController(
    SupplierIntegrationService service,
    ISecurityAuditEventWriter securityAudit,
    IAuditRequestContext auditRequest,
    IStringLocalizer<SharedResource> localizer,
    ILogger<SupplierPortalController> logger):Controller
{
    [HttpGet("supplier-portal/{token}")]
    public async Task<IActionResult>Index(string token){PrivateResponse();var page=await service.GetPortalPageAsync(token,Remote(),HttpContext.TraceIdentifier);if(page is null)await RecordDenied();return page is null?View("Unavailable"):View(page);}
    [HttpPost("supplier-portal/quotation"),ValidateAntiForgeryToken]
    public async Task<IActionResult>Quotation(SupplierPortalQuotationDto dto){try{var result=await service.SubmitPortalQuotationAsync(dto,Remote(),HttpContext.TraceIdentifier);Notify(result.Replayed?"This supplier submission was already received safely.":"Supplier quotation submitted successfully.");}catch(UnauthorizedAccessException e){logger.LogWarning("Rejected supplier portal quotation authentication. Correlation {CorrelationId}",HttpContext.TraceIdentifier);await RecordDenied();Notify(e.Message,"danger");}catch(Exception e)when(e is ArgumentException or InvalidOperationException){logger.LogWarning("Rejected supplier portal quotation. Correlation {CorrelationId}: {Reason}",HttpContext.TraceIdentifier,e.Message);Notify(e.Message,"danger");}return RedirectToAction(nameof(Index),new{token=dto.Token});}
    [HttpPost("supplier-portal/order-confirmation"),ValidateAntiForgeryToken]
    public async Task<IActionResult>ConfirmOrder(SupplierExternalOrderConfirmationDto dto){try{var result=await service.ConfirmPortalOrderAsync(dto,Remote(),HttpContext.TraceIdentifier);Notify(result.Replayed?"This supplier submission was already received safely.":"Purchase order confirmation submitted successfully.");}catch(UnauthorizedAccessException e){logger.LogWarning("Rejected supplier portal order confirmation authentication. Correlation {CorrelationId}",HttpContext.TraceIdentifier);await RecordDenied();Notify(e.Message,"danger");}catch(Exception e)when(e is ArgumentException or InvalidOperationException){logger.LogWarning("Rejected supplier portal order confirmation. Correlation {CorrelationId}: {Reason}",HttpContext.TraceIdentifier,e.Message);Notify(e.Message,"danger");}return RedirectToAction(nameof(Index),new{token=dto.Token});}
    [HttpPost("supplier-portal/delivery-notice"),ValidateAntiForgeryToken]
    public async Task<IActionResult>DeliveryNotice(SupplierExternalDeliveryNoticeDto dto){try{var result=await service.SubmitPortalDeliveryAsync(dto,Remote(),HttpContext.TraceIdentifier);Notify(result.Replayed?"This supplier submission was already received safely.":"Delivery notice submitted for internal review.");}catch(UnauthorizedAccessException e){logger.LogWarning("Rejected supplier portal delivery authentication. Correlation {CorrelationId}",HttpContext.TraceIdentifier);await RecordDenied();Notify(e.Message,"danger");}catch(Exception e)when(e is ArgumentException or InvalidOperationException){logger.LogWarning("Rejected supplier portal delivery notice. Correlation {CorrelationId}: {Reason}",HttpContext.TraceIdentifier,e.Message);Notify(e.Message,"danger");}return RedirectToAction(nameof(Index),new{token=dto.Token});}
    private string? Remote()=>HttpContext.Connection.RemoteIpAddress?.ToString();
    private Task<Guid?> RecordDenied()=>securityAudit.RecordAsync(new IndependentAuditEventWriteRequest
    {
        Scope=AuditEventScope.System,Category=AuditEventCategory.Integration,Severity=AuditEventSeverity.Warning,
        Outcome=AuditEventOutcome.Denied,EventType=SemanticAuditEventTypes.SupplierPortalAuthenticationDenied,
        Action="Authenticate",ActorType=AuditActorType.External,ActorId="external",SourceChannel=AuditSourceChannel.Portal,
        CorrelationId=auditRequest.CorrelationId,ResourceType="SupplierPortal",ResourceId="request",
        ReasonCode="ExternalCredentialInvalid",RequestPathTemplate=auditRequest.RequestPathTemplate,
        SourceIpMasked=auditRequest.SourceIpMasked,UserAgentHash=auditRequest.UserAgentHash
    },CancellationToken.None);
    private void PrivateResponse(){Response.Headers.CacheControl="no-store";Response.Headers["Referrer-Policy"]="no-referrer";Response.Headers["X-Robots-Tag"]="noindex, nofollow";}
    private void Notify(string message,string type="success"){TempData["NotificationType"]=type;TempData["NotificationMessage"]=localizer[message].Value;}
}
