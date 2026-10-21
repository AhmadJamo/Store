using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers;
public sealed class CustomerReturnReceiptsController(CustomerReturnReceiptService service,IStringLocalizer<SharedResource> localizer):Controller
{
 [HttpGet,PermissionAuthorize("CustomerReturnReceipts.View")]public async Task<IActionResult>Index(string?search){ViewBag.Search=search;return View(await service.GetAllAsync(search));}
 [HttpGet,PermissionAuthorize("CustomerReturnReceipts.View")]public async Task<IActionResult>Details(int id){var page=await service.GetAsync(id);return page is null?NotFound():View(page);}
 [HttpGet,PermissionAuthorize("CustomerReturnReceipts.Create")]public async Task<IActionResult>Create(int customerReturnRequestId){var page=await service.GetCreatePageAsync(customerReturnRequestId);if(page is null){Notify("danger","Select an approved customer return request before receiving returned goods.");return RedirectToAction("Index","CustomerReturnRequests");}return View(page);}
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("CustomerReturnReceipts.Create")]public async Task<IActionResult>Create(CreateCustomerReturnReceiptDto dto){try{var id=await service.CreateAsync(dto);Notify("success","Customer return receipt draft created. Review the inspection decisions, then complete inspection.");return RedirectToAction(nameof(Details),new{id});}catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){Notify("danger",ex.Message);var page=await service.GetCreatePageAsync(dto.CustomerReturnRequestId,dto);return page is null?RedirectToAction("Index","CustomerReturnRequests"):View(page);}}
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("CustomerReturnReceipts.Inspect")]public Task<IActionResult>Inspect(CustomerReturnReceiptCommandDto dto)=>Run(dto,service.InspectAsync,"Customer return inspection completed. The next step is posting the approved inventory dispositions.");
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("CustomerReturnReceipts.PostDisposition")]public Task<IActionResult>Post(CustomerReturnReceiptCommandDto dto)=>Run(dto,service.PostAsync,"Customer return dispositions posted. Restocked goods and their cost reversal are now recorded.");
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("CustomerReturnReceipts.Cancel")]public Task<IActionResult>Cancel(CustomerReturnReceiptCommandDto dto)=>Run(dto,service.CancelAsync,"Customer return receipt draft cancelled.");
 private async Task<IActionResult>Run(CustomerReturnReceiptCommandDto dto,Func<CustomerReturnReceiptCommandDto,Task> action,string message){try{await action(dto);Notify("success",message);}catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){Notify("danger",ex.Message);}return RedirectToAction(nameof(Details),new{id=dto.Id});}
 private void Notify(string type,string key){TempData["NotificationType"]=type;TempData["NotificationMessage"]=localizer[key].Value;}
}
