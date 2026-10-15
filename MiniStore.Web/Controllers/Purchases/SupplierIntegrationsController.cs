using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

[PermissionAuthorize("SupplierIntegrations.View")]
public sealed class SupplierIntegrationsController(SupplierIntegrationService service,IStringLocalizer<SharedResource> localizer):Controller
{
    [HttpGet]public async Task<IActionResult>Index()=>View(await service.GetDashboardAsync());
    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SupplierIntegrations.Manage")]
    public async Task<IActionResult>CreateCredential(CreateSupplierIntegrationCredentialDto dto){try{var created=await service.CreateCredentialAsync(dto);Response.Headers.CacheControl="no-store";ViewBag.ApiKey=created.ApiKey;return View("CredentialCreated",created);}catch(Exception e)when(e is ArgumentException or InvalidOperationException){Notify(e.Message,"danger");return RedirectToAction(nameof(Index));}}
    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SupplierIntegrations.Manage")]
    public async Task<IActionResult>DeactivateCredential(int id,string rowVersion){try{await service.DeactivateCredentialAsync(id,Convert.FromBase64String(rowVersion));Notify("Supplier integration credential deactivated.");}catch(Exception e)when(e is ArgumentException or InvalidOperationException or FormatException){Notify(e.Message,"danger");}return RedirectToAction(nameof(Index));}
    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SupplierIntegrations.Manage")]
    public async Task<IActionResult>CreatePortalAccess(CreateSupplierPortalAccessDto dto){try{var created=await service.CreatePortalAccessAsync(dto);Response.Headers.CacheControl="no-store";Response.Headers["Referrer-Policy"]="no-referrer";ViewBag.PortalUrl=Url.Action("Index","SupplierPortal",new{token=created.Token},Request.Scheme);return View("PortalAccessCreated",created);}catch(Exception e)when(e is ArgumentException or InvalidOperationException){Notify(e.Message,"danger");return RedirectToDocument(dto.DocumentType,dto.DocumentId);}}
    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SupplierIntegrations.Manage")]
    public async Task<IActionResult>RevokePortalAccess(int id,SupplierExternalDocumentType documentType,int documentId){try{await service.RevokePortalAccessAsync(id);Notify("Supplier portal access revoked.");}catch(Exception e)when(e is ArgumentException or InvalidOperationException){Notify(e.Message,"danger");}return RedirectToDocument(documentType,documentId);}
    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SupplierIntegrations.Queue")]
    public async Task<IActionResult>QueuePurchaseOrder(QueueSupplierPurchaseOrderDto dto){try{await service.QueuePurchaseOrderAsync(dto);Notify("Purchase order queued for reliable external delivery.");}catch(Exception e)when(e is ArgumentException or InvalidOperationException){Notify(e.Message,"danger");}return RedirectToAction("Details","PurchaseOrders",new{id=dto.PurchaseOrderId});}
    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SupplierIntegrations.Retry")]
    public async Task<IActionResult>Retry(int id){try{await service.RetryOutboxAsync(id);Notify("Outbox delivery queued for retry.");}catch(Exception e)when(e is ArgumentException or InvalidOperationException){Notify(e.Message,"danger");}return RedirectToAction(nameof(Index));}
    [HttpGet,PermissionAuthorize("SupplierIntegrations.View")]
    public async Task<IActionResult>PurchaseOrderUbl(int id){try{var xml=await service.ExportPurchaseOrderUblAsync(id);return File(System.Text.Encoding.UTF8.GetBytes(xml),"application/xml",$"purchase-order-{id}.xml");}catch(InvalidOperationException){return NotFound();}}
    [HttpGet,PermissionAuthorize("SupplierIntegrations.View")]public async Task<IActionResult>Submission(int id){var value=await service.GetSubmissionAsync(id);return value is null?NotFound():View(value);}
    [HttpGet,PermissionAuthorize("SupplierIntegrations.View")]public async Task<IActionResult>DownloadSubmission(int id){var value=await service.GetSubmissionAsync(id);return value is null?NotFound():File(System.Text.Encoding.UTF8.GetBytes(value.Payload),value.Type==SupplierExternalSubmissionType.InvoiceUbl?"application/xml":"application/json",$"supplier-submission-{value.Id}.{(value.Type==SupplierExternalSubmissionType.InvoiceUbl?"xml":"json")}");}
    private IActionResult RedirectToDocument(SupplierExternalDocumentType type,int id)=>type==SupplierExternalDocumentType.Rfq?RedirectToAction("Details","PurchaseSourcing",new{id}):RedirectToAction("Details","PurchaseOrders",new{id});
    private void Notify(string message,string type="success"){TempData["NotificationType"]=type;TempData["NotificationMessage"]=localizer[message].Value;}
}
