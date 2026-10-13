using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.DTOs.Purchases;using MiniStore.Application.Services;using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers;
public sealed class LandedCostsController(LandedCostService service,PurchaseDocumentReversalService reversalService,IStringLocalizer<SharedResource>localizer):Controller
{
 [HttpGet,PermissionAuthorize("LandedCosts.View")]public async Task<IActionResult>Index()=>View(await service.GetAllAsync());
 [HttpGet,PermissionAuthorize("LandedCosts.View")]public async Task<IActionResult>Details(int id){var value=await service.GetAsync(id);ViewBag.Reversal=await reversalService.GetByOriginalAsync(MiniStore.Domain.Entities.PurchaseDocumentType.LandedCost,id);return value is null?NotFound():View(value);}
 [HttpGet,PermissionAuthorize("LandedCosts.Create")]public async Task<IActionResult>Create(int goodsReceiptId){var page=await service.GetCreatePageAsync(goodsReceiptId);return string.IsNullOrEmpty(page.ReceiptNumber)?NotFound():View(page);}
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("LandedCosts.Create")]public async Task<IActionResult>Create(CreateLandedCostDto dto){try{var id=await service.CreateAndPostAsync(dto);TempData["NotificationType"]="success";TempData["NotificationMessage"]=localizer["Landed cost posted."].Value;return RedirectToAction(nameof(Details),new{id});}catch(Exception e)when(e is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,localizer[e.Message]);var page=await service.GetCreatePageAsync(dto.GoodsReceiptId,dto);return string.IsNullOrEmpty(page.ReceiptNumber)?NotFound():View(page);}}
}
