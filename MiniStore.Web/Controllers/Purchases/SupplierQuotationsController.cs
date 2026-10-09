using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;
public sealed class SupplierQuotationsController(SupplierQuotationService service,IStringLocalizer<SharedResource> localizer):Controller
{
 [HttpGet][PermissionAuthorize("SupplierQuotations.Create")]public async Task<IActionResult>Create(int sourcingEventId){var page=await service.GetCreatePageAsync(sourcingEventId);return page.SourcingEvent is null?NotFound():View(page);}
 [HttpPost][ValidateAntiForgeryToken][PermissionAuthorize("SupplierQuotations.Create")]public async Task<IActionResult>Create(CreateSupplierQuotationDto dto){try{var id=await service.CreateAndSubmitAsync(dto);TempData["NotificationType"]="success";TempData["NotificationMessage"]=localizer["Supplier quotation submitted."].Value;return RedirectToAction(nameof(Comparison),new{sourcingEventId=dto.PurchaseSourcingEventId});}catch(Exception e)when(e is ArgumentException or InvalidOperationException){ModelState.AddModelError("",localizer[e.Message]);var page=await service.GetCreatePageAsync(dto.PurchaseSourcingEventId,dto);return page.SourcingEvent is null?NotFound():View(page);}}
 [HttpGet][PermissionAuthorize("SupplierQuotations.View")]public async Task<IActionResult>Comparison(int sourcingEventId)=>View(await service.GetComparisonAsync(sourcingEventId));
 [HttpPost][ValidateAntiForgeryToken][PermissionAuthorize("SupplierQuotations.Award")]public async Task<IActionResult>Award(int sourcingEventId,int quotationId,string reason){try{await service.AwardAsync(sourcingEventId,quotationId,reason);TempData["NotificationType"]="success";TempData["NotificationMessage"]=localizer["Supplier quotation awarded."].Value;}catch(Exception e)when(e is ArgumentException or InvalidOperationException){TempData["NotificationType"]="danger";TempData["NotificationMessage"]=localizer[e.Message].Value;}return RedirectToAction(nameof(Comparison),new{sourcingEventId});}
}
