using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;

namespace MiniStore.Web.Controllers;

[ApiController,AllowAnonymous,IgnoreAntiforgeryToken,EnableRateLimiting("supplier-api"),Route("api/supplier/v1")]
public sealed class SupplierIntegrationApiController(
    SupplierIntegrationService service,
    ISecurityAuditEventWriter securityAudit,
    IAuditRequestContext auditRequest,
    ILogger<SupplierIntegrationApiController> logger):ControllerBase
{
    [HttpGet("rfqs/{id:int}")]public Task<IActionResult>GetRfq(int id)=>Run(async()=>Ok(await service.GetApiRfqAsync(Key(),id,Remote(),HttpContext.TraceIdentifier)));
    [HttpGet("purchase-orders/{id:int}")]public Task<IActionResult>GetOrder(int id)=>Run(async()=>Ok(await service.GetApiOrderAsync(Key(),id,Remote(),HttpContext.TraceIdentifier)));
    [HttpGet("purchase-orders/{id:int}/ubl")]public Task<IActionResult>GetOrderUbl(int id)=>Run(async()=>Content(await service.GetApiOrderUblAsync(Key(),id,Remote(),HttpContext.TraceIdentifier),"application/xml",System.Text.Encoding.UTF8));
    [HttpPost("quotations")]public Task<IActionResult>Quotation(SupplierApiQuotationRequest dto)=>Run(async()=>Ok(await service.SubmitApiQuotationAsync(Key(),Idempotency(),dto,Remote(),HttpContext.TraceIdentifier)));
    [HttpPost("purchase-orders/{id:int}/confirmations")]public Task<IActionResult>Confirm(int id,SupplierApiOrderConfirmationRequest dto)=>Run(async()=>Ok(await service.ConfirmApiOrderAsync(Key(),Idempotency(),id,dto,Remote(),HttpContext.TraceIdentifier)));
    [HttpPost("purchase-orders/{id:int}/confirmations/ubl"),RequestSizeLimit(2_097_152)]public Task<IActionResult>ConfirmUbl(int id)=>Run(async()=>{using var reader=new StreamReader(Request.Body);var xml=await reader.ReadToEndAsync();return Ok(await service.ConfirmApiOrderUblAsync(Key(),Idempotency(),id,xml,Remote(),HttpContext.TraceIdentifier));});
    [HttpPost("purchase-orders/{id:int}/delivery-notices")]public Task<IActionResult>Delivery(int id,SupplierApiDeliveryNoticeRequest dto)=>Run(async()=>Ok(await service.SubmitApiDeliveryAsync(Key(),Idempotency(),id,dto,Remote(),HttpContext.TraceIdentifier)));
    [HttpPost("purchase-orders/{id:int}/invoices/ubl"),RequestSizeLimit(2_097_152)]public Task<IActionResult>Invoice(int id)=>Run(async()=>{using var reader=new StreamReader(Request.Body);var xml=await reader.ReadToEndAsync();return Ok(await service.SubmitApiInvoiceUblAsync(Key(),Idempotency(),id,xml,Remote(),HttpContext.TraceIdentifier));});
    private string Key()=>Request.Headers["X-Supplier-Api-Key"].ToString();private string Idempotency()=>Request.Headers["Idempotency-Key"].ToString();private string? Remote()=>HttpContext.Connection.RemoteIpAddress?.ToString();
    private async Task<IActionResult>Run(Func<Task<IActionResult>> action)
    {
        try{return await action();}
        catch(UnauthorizedAccessException e)
        {
            logger.LogWarning("Rejected supplier API authentication. Correlation {CorrelationId}",HttpContext.TraceIdentifier);
            await RecordDenied();
            return Unauthorized(new{error=e.Message});
        }
        catch(InvalidOperationException e)when(e.Message.StartsWith("Idempotency key",StringComparison.Ordinal)){logger.LogWarning("Rejected supplier API idempotency conflict. Correlation {CorrelationId}",HttpContext.TraceIdentifier);return Conflict(new{error=e.Message});}
        catch(Exception e)when(e is ArgumentException or InvalidOperationException or System.Xml.XmlException){logger.LogWarning("Rejected supplier API request. Correlation {CorrelationId}: {Reason}",HttpContext.TraceIdentifier,e.Message);return BadRequest(new{error=e.Message});}
    }

    private Task<Guid?> RecordDenied() => securityAudit.RecordAsync(new IndependentAuditEventWriteRequest
    {
        Scope = AuditEventScope.System,
        Category = AuditEventCategory.Integration,
        Severity = AuditEventSeverity.Warning,
        Outcome = AuditEventOutcome.Denied,
        EventType = SemanticAuditEventTypes.SupplierApiAuthenticationDenied,
        Action = "Authenticate",
        ActorType = AuditActorType.External,
        ActorId = "external",
        SourceChannel = AuditSourceChannel.Api,
        CorrelationId = auditRequest.CorrelationId,
        ResourceType = "SupplierApi",
        ResourceId = "request",
        ReasonCode = "ExternalCredentialInvalid",
        RequestPathTemplate = auditRequest.RequestPathTemplate,
        SourceIpMasked = auditRequest.SourceIpMasked,
        UserAgentHash = auditRequest.UserAgentHash
    }, CancellationToken.None);
}
