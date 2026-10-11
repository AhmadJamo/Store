using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.DTOs.Purchases;using MiniStore.Application.Services;using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers;
public sealed class PurchaseAnalyticsController(PurchaseAnalyticsService service,IStringLocalizer<SharedResource>localizer):Controller
{
 [HttpGet,PermissionAuthorize("PurchaseAnalytics.View")]public async Task<IActionResult>Index([FromQuery]PurchaseAnalyticsQueryDto query){try{return View(await service.GetAsync(query));}catch(ArgumentException e){TempData["NotificationType"]="danger";TempData["NotificationMessage"]=localizer[e.Message].Value;return View(await service.GetAsync(new()));}}
}
