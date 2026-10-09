using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.Services;using MiniStore.Domain.Entities;using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers;
public sealed class PurchaseOrdersController(PurchaseOrderService service,IStringLocalizer<SharedResource> localizer):Controller
{
 [HttpGet][PermissionAuthorize("PurchaseOrders.View")]public async Task<IActionResult>Index(PurchaseOrderStatus? status,string? search){ViewBag.Status=status;ViewBag.Search=search;return View(await service.GetAllAsync(status,search));}
 [HttpPost][ValidateAntiForgeryToken][PermissionAuthorize("PurchaseOrders.Create")]public async Task<IActionResult>CreateFromAward(int sourcingEventId){try{var id=await service.CreateFromAwardAsync(sourcingEventId);TempData["NotificationType"]="success";TempData["NotificationMessage"]=localizer["Purchase order created."].Value;return RedirectToAction(nameof(Details),new{id});}catch(Exception e)when(e is ArgumentException or InvalidOperationException){TempData["NotificationType"]="danger";TempData["NotificationMessage"]=localizer[e.Message].Value;return RedirectToAction("Comparison","SupplierQuotations",new{sourcingEventId});}}
 [HttpGet][PermissionAuthorize("PurchaseOrders.View")]public async Task<IActionResult> Details(int id){var dto=await service.GetAsync(id);return dto is null?NotFound():View(dto);}
 [HttpPost][ValidateAntiForgeryToken][PermissionAuthorize("PurchaseOrders.Approve")]public async Task<IActionResult>Approve(int id,byte[] rowVersion){await service.ApproveAsync(id,rowVersion);return RedirectToAction(nameof(Details),new{id});}
 [HttpPost][ValidateAntiForgeryToken][PermissionAuthorize("PurchaseOrders.Confirm")]public async Task<IActionResult>Confirm(int id,byte[] rowVersion){await service.ConfirmAsync(id,rowVersion);return RedirectToAction(nameof(Details),new{id});}
 [HttpPost][ValidateAntiForgeryToken][PermissionAuthorize("PurchaseOrders.Cancel")]public async Task<IActionResult>Cancel(int id,string reason,byte[] rowVersion){await service.CancelAsync(id,reason,rowVersion);return RedirectToAction(nameof(Details),new{id});}
}
