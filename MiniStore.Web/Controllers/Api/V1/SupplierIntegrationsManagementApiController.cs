using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/supplier-integrations")]
public sealed class SupplierIntegrationsManagementApiController(SupplierIntegrationService service, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("SupplierIntegrations.View")] public async Task<IActionResult> Get() => Ok(await service.GetDashboardAsync());
    [HttpPost("credentials"), ValidateAntiForgeryToken, PermissionAuthorize("SupplierIntegrations.Manage")]
    public async Task<IActionResult> CreateCredential(CreateSupplierIntegrationCredentialDto dto) { try { var value=await service.CreateCredentialAsync(dto); Response.Headers.CacheControl="no-store"; return Ok(value); } catch(Exception e) when(e is ArgumentException or InvalidOperationException){return Rule(e.Message);} }
    [HttpPost("credentials/{id:int}/deactivate"), ValidateAntiForgeryToken, PermissionAuthorize("SupplierIntegrations.Manage")]
    public async Task<IActionResult> Deactivate(int id,[FromBody]string rowVersion){try{await service.DeactivateCredentialAsync(id,Convert.FromBase64String(rowVersion));return NoContent();}catch(Exception e)when(e is ArgumentException or InvalidOperationException or FormatException){return Rule(e.Message);}}
    [HttpGet("portal-access/{type}/{documentId:int}"), PermissionAuthorize("SupplierIntegrations.View")] public async Task<IActionResult> Accesses(SupplierExternalDocumentType type,int documentId)=>Ok(await service.GetPortalAccessesAsync(type,documentId));
    [HttpPost("portal-access"),ValidateAntiForgeryToken,PermissionAuthorize("SupplierIntegrations.Manage")]
    public async Task<IActionResult>CreateAccess(CreateSupplierPortalAccessDto dto){try{var value=await service.CreatePortalAccessAsync(dto);Response.Headers.CacheControl="no-store";Response.Headers["Referrer-Policy"]="no-referrer";return Ok(new{value.Id,value.Token,value.ExpiresAtUtc,PortalUrl=Url.Action("Index","SupplierPortal",new{token=value.Token},Request.Scheme)});}catch(Exception e)when(e is ArgumentException or InvalidOperationException){return Rule(e.Message);}}
    [HttpPost("portal-access/{id:int}/revoke"),ValidateAntiForgeryToken,PermissionAuthorize("SupplierIntegrations.Manage")] public async Task<IActionResult>Revoke(int id){try{await service.RevokePortalAccessAsync(id);return NoContent();}catch(Exception e)when(e is ArgumentException or InvalidOperationException){return Rule(e.Message);}}
    [HttpGet("credentials/supplier/{supplierId:int}"),PermissionAuthorize("SupplierIntegrations.View")] public async Task<IActionResult>Credentials(int supplierId,int? purchaseOrderId)=>Ok(await service.GetCredentialsForSupplierAsync(supplierId,purchaseOrderId));
    [HttpPost("outbox/purchase-orders"),ValidateAntiForgeryToken,PermissionAuthorize("SupplierIntegrations.Queue")] public async Task<IActionResult>Queue(QueueSupplierPurchaseOrderDto dto){try{await service.QueuePurchaseOrderAsync(dto);return NoContent();}catch(Exception e)when(e is ArgumentException or InvalidOperationException){return Rule(e.Message);}}
    [HttpPost("outbox/{id:int}/retry"),ValidateAntiForgeryToken,PermissionAuthorize("SupplierIntegrations.Retry")] public async Task<IActionResult>Retry(int id){try{await service.RetryOutboxAsync(id);return NoContent();}catch(Exception e)when(e is ArgumentException or InvalidOperationException){return Rule(e.Message);}}
    [HttpGet("purchase-orders/{id:int}/ubl"),PermissionAuthorize("SupplierIntegrations.View")] public async Task<IActionResult>Ubl(int id){try{return File(System.Text.Encoding.UTF8.GetBytes(await service.ExportPurchaseOrderUblAsync(id)),"application/xml",$"purchase-order-{id}.xml");}catch(InvalidOperationException){return NotFound();}}
    [HttpGet("submissions/{id:int}"),PermissionAuthorize("SupplierIntegrations.View")] public async Task<IActionResult>Submission(int id)=>await service.GetSubmissionAsync(id)is{} value?Ok(value):NotFound();
    [HttpGet("submissions/{id:int}/download"),PermissionAuthorize("SupplierIntegrations.View")] public async Task<IActionResult>Download(int id){var value=await service.GetSubmissionAsync(id);return value is null?NotFound():File(System.Text.Encoding.UTF8.GetBytes(value.Payload),value.Type==SupplierExternalSubmissionType.InvoiceUbl?"application/xml":"application/json",$"supplier-submission-{id}.{(value.Type==SupplierExternalSubmissionType.InvoiceUbl?"xml":"json")}");}
    private ObjectResult Rule(string message)=>InternalApiProblemFactory.Create(HttpContext,409,localizer[message],InternalApiProblemCodes.Validation);
}
