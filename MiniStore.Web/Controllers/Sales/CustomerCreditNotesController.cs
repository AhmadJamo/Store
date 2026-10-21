using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers;
public sealed class CustomerCreditNotesController(CustomerCreditNoteService service,IStringLocalizer<SharedResource>localizer):Controller
{
 [HttpGet,PermissionAuthorize("CustomerCreditNotes.View")]public async Task<IActionResult>Index()=>View(await service.GetAllAsync());
 [HttpGet,PermissionAuthorize("CustomerCreditNotes.View")]public async Task<IActionResult>Details(int id)=>await service.GetAsync(id)is{}x?View(x):NotFound();
 [HttpGet,PermissionAuthorize("CustomerCreditNotes.Create")]public async Task<IActionResult>Create(int customerInvoiceId){var page=await service.GetCreatePageAsync(customerInvoiceId);return page.InvoiceId==0?NotFound():View(page);}
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("CustomerCreditNotes.Create")]public async Task<IActionResult>Create(CreateCustomerCreditNoteDto dto){try{var id=await service.CreateAndPostAsync(dto);TempData["NotificationType"]="success";TempData["NotificationMessage"]=localizer["Customer credit note posted. No cash refund was created."].Value;return RedirectToAction(nameof(Details),new{id,customerInvoiceId=dto.CustomerInvoiceId});}catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,localizer[ex.Message]);var page=await service.GetCreatePageAsync(dto.CustomerInvoiceId,dto);return page.InvoiceId==0?NotFound():View(page);}}
}
