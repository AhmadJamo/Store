using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.DTOs.Purchases;using MiniStore.Application.Services;using MiniStore.Domain.Entities;using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers;
public sealed class PurchaseRequestsController(PurchaseRequestService service,IStringLocalizer<SharedResource> localizer):Controller
{
 [HttpGet][PermissionAuthorize("PurchaseRequests.View")]public async Task<IActionResult> Index(PurchaseRequestStatus? status,string? search){ViewBag.Status=status;ViewBag.Search=search;return View(await service.GetAllAsync(status,search));}
 [HttpGet][PermissionAuthorize("PurchaseRequests.Create")]public async Task<IActionResult>Create()=>View(await service.GetCreatePageAsync());
 [HttpPost][ValidateAntiForgeryToken][PermissionAuthorize("PurchaseRequests.Create")]public async Task<IActionResult>Create(CreatePurchaseRequestDto dto){try{var id=await service.CreateAsync(dto);TempData["NotificationType"]="success";TempData["NotificationMessage"]=localizer["Purchase request created."].Value;return RedirectToAction(nameof(Details),new{id});}catch(Exception e)when(e is ArgumentException or InvalidOperationException){ModelState.AddModelError("",localizer[e.Message]);return View(await service.GetCreatePageAsync(dto));}}
 [HttpGet][PermissionAuthorize("PurchaseRequests.View")]public async Task<IActionResult>Details(int id){var dto=await service.GetAsync(id);return dto is null?NotFound():View(dto);}
 [HttpPost][ValidateAntiForgeryToken][PermissionAuthorize("PurchaseRequests.Submit")]public async Task<IActionResult>Submit(int id,byte[] rowVersion){await service.SubmitAsync(id,rowVersion);return RedirectToAction(nameof(Details),new{id});}
 [HttpPost][ValidateAntiForgeryToken][PermissionAuthorize("PurchaseRequests.Cancel")]public async Task<IActionResult>Cancel(int id,string reason,byte[] rowVersion){await service.CancelAsync(id,reason,rowVersion);return RedirectToAction(nameof(Details),new{id});}
}
