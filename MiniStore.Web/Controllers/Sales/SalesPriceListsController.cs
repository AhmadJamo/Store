using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers;
public sealed class SalesPriceListsController(SalesPriceListService service,IStringLocalizer<SharedResource> localizer):Controller
{
 [HttpGet,PermissionAuthorize("SalesPricing.View")]
 public async Task<IActionResult> Index(int? id=null)=>View(await service.GetPageAsync(id));

 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SalesPricing.Manage")]
 public async Task<IActionResult> Create([Bind(Prefix="ListForm")]SaveSalesPriceListDto dto)
 {if(!ModelState.IsValid)return View(nameof(Index),await service.GetPageAsync(null,dto));try{var id=await service.CreateListAsync(dto);Success("Price list created successfully.");return RedirectToAction(nameof(Index),new{id});}catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,localizer[ex.Message]);return View(nameof(Index),await service.GetPageAsync(null,dto));}}

 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SalesPricing.Manage")]
 public async Task<IActionResult> SetListActive(SalesPricingStatusDto dto){try{await service.SetListActiveAsync(dto);Success(dto.Active?"Price list activated.":"Price list archived.");}catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){TempData["NotificationType"]="danger";TempData["NotificationMessage"]=localizer[ex.Message].Value;}return RedirectToAction(nameof(Index),new{id=dto.Id});}

 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SalesPricing.Manage")]
 public async Task<IActionResult> AddRule([Bind(Prefix="RuleForm")]SaveSalesPriceRuleDto dto)
 {if(!ModelState.IsValid)return View(nameof(Index),await service.GetPageAsync(dto.SalesPriceListId,ruleForm:dto));try{await service.AddRuleAsync(dto);Success("Price rule added successfully.");return RedirectToAction(nameof(Index),new{id=dto.SalesPriceListId});}catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,localizer[ex.Message]);return View(nameof(Index),await service.GetPageAsync(dto.SalesPriceListId,ruleForm:dto));}}

 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SalesPricing.Manage")]
 public async Task<IActionResult> SetRuleActive(SalesPricingStatusDto dto){try{await service.SetRuleActiveAsync(dto);Success(dto.Active?"Price rule activated.":"Price rule archived.");}catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){TempData["NotificationType"]="danger";TempData["NotificationMessage"]=localizer[ex.Message].Value;}return RedirectToAction(nameof(Index),new{id=dto.PriceListId});}

 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SalesPricing.View")]
 public async Task<IActionResult> Evaluate([Bind(Prefix="EvaluationForm")]EvaluateSalesPriceDto dto)
 {if(!ModelState.IsValid)return View(nameof(Index),await service.GetPageAsync(dto.PriceListId,evaluationForm:dto));try{var result=await service.EvaluateAsync(dto);return View(nameof(Index),await service.GetPageAsync(dto.PriceListId,evaluationForm:dto,result:result));}catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,localizer[ex.Message]);return View(nameof(Index),await service.GetPageAsync(dto.PriceListId,evaluationForm:dto));}}
 private void Success(string key){TempData["NotificationType"]="success";TempData["NotificationMessage"]=localizer[key].Value;}
}
