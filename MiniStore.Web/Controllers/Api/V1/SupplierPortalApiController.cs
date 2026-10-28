using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController,AllowAnonymous,EnableRateLimiting("supplier-portal"),Route("api/public/supplier-portal")]
public sealed class SupplierPortalApiController(SupplierIntegrationService service,ISecurityAuditEventWriter securityAudit,IAuditRequestContext auditRequest,IStringLocalizer<SharedResource> localizer,ILogger<SupplierPortalApiController> logger):ControllerBase
{
 [HttpGet("{token}")]public async Task<IActionResult>Get(string token){Private();var page=await service.GetPortalPageAsync(token,Remote(),HttpContext.TraceIdentifier);if(page is null){await Denied();return InternalApiProblemFactory.Create(HttpContext,404,localizer["Supplier portal link unavailable"],InternalApiProblemCodes.NotFound,localizer["This private link is invalid, expired or revoked. Contact the purchasing team for a new link."]);}return Ok(page);}
 [HttpPost("quotation"),ValidateAntiForgeryToken]public Task<IActionResult>Quotation(SupplierPortalQuotationDto dto)=>Run(()=>service.SubmitPortalQuotationAsync(dto,Remote(),HttpContext.TraceIdentifier));
 [HttpPost("order-confirmation"),ValidateAntiForgeryToken]public Task<IActionResult>Confirm(SupplierExternalOrderConfirmationDto dto)=>Run(()=>service.ConfirmPortalOrderAsync(dto,Remote(),HttpContext.TraceIdentifier));
 [HttpPost("delivery-notice"),ValidateAntiForgeryToken]public Task<IActionResult>Delivery(SupplierExternalDeliveryNoticeDto dto)=>Run(()=>service.SubmitPortalDeliveryAsync(dto,Remote(),HttpContext.TraceIdentifier));
 private async Task<IActionResult>Run(Func<Task<SupplierExternalResultDto>> action){Private();try{return Ok(await action());}catch(UnauthorizedAccessException e){logger.LogWarning("Rejected Angular supplier portal authentication. Correlation {CorrelationId}",HttpContext.TraceIdentifier);await Denied();return InternalApiProblemFactory.Create(HttpContext,401,localizer[e.Message],InternalApiProblemCodes.Unauthorized);}catch(Exception e)when(e is ArgumentException or InvalidOperationException){return InternalApiProblemFactory.Create(HttpContext,409,localizer[e.Message],InternalApiProblemCodes.Validation);}}
 private void Private(){Response.Headers.CacheControl="no-store";Response.Headers["Referrer-Policy"]="no-referrer";Response.Headers["X-Robots-Tag"]="noindex, nofollow";}
 private string?Remote()=>HttpContext.Connection.RemoteIpAddress?.ToString();
 private Task<Guid?>Denied()=>securityAudit.RecordAsync(new IndependentAuditEventWriteRequest{Scope=AuditEventScope.System,Category=AuditEventCategory.Integration,Severity=AuditEventSeverity.Warning,Outcome=AuditEventOutcome.Denied,EventType=SemanticAuditEventTypes.SupplierPortalAuthenticationDenied,Action="Authenticate",ActorType=AuditActorType.External,ActorId="external",SourceChannel=AuditSourceChannel.Portal,CorrelationId=auditRequest.CorrelationId,ResourceType="SupplierPortal",ResourceId="request",ReasonCode="ExternalCredentialInvalid",RequestPathTemplate=auditRequest.RequestPathTemplate,SourceIpMasked=auditRequest.SourceIpMasked,UserAgentHash=auditRequest.UserAgentHash},CancellationToken.None);
}
