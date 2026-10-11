using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.DTOs.Purchases;using MiniStore.Application.Services;using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers;
public sealed class PurchaseTemplatesController(PurchaseTemplateService service,IStringLocalizer<SharedResource> localizer):Controller
{
 [HttpGet,PermissionAuthorize("PurchaseTemplates.View")]public async Task<IActionResult>Index()=>View(await service.GetAllAsync());
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("PurchaseTemplates.Create")]public async Task<IActionResult>CreateFromOrder(CreatePurchaseTemplateDto dto){try{await service.CreateFromOrderAsync(dto);Notify("success","Purchase template created.");return RedirectToAction(nameof(Index));}catch(Exception e)when(e is ArgumentException or InvalidOperationException){Notify("danger",e.Message);return RedirectToAction("Details","PurchaseOrders",new{id=dto.PurchaseOrderId});}}
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("PurchaseTemplates.Generate")]public async Task<IActionResult>Generate(int id){try{var requestId=await service.GenerateRequestAsync(id);Notify("success","Purchase request draft generated from template.");return RedirectToAction("Details","PurchaseRequests",new{id=requestId});}catch(Exception e)when(e is ArgumentException or InvalidOperationException){Notify("danger",e.Message);return RedirectToAction(nameof(Index));}}
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("PurchaseTemplates.Manage")]public async Task<IActionResult>SetActive(int id,bool active){await service.SetActiveAsync(id,active);return RedirectToAction(nameof(Index));}
 private void Notify(string type,string message){TempData["NotificationType"]=type;TempData["NotificationMessage"]=localizer[message].Value;}
}
