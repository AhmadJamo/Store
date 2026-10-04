using System.Security.Claims;using Microsoft.AspNetCore.Mvc;using MiniStore.Application.DTOs.Inventory.Tracking;using MiniStore.Application.Services;using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers;
public sealed class InventoryTrackingController(InventoryTrackingService service):Controller
{
 [HttpGet,PermissionAuthorize("InventoryTracking.View")]public async Task<IActionResult> Index()=>View(await service.GetPageAsync());
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("InventoryTracking.Open")]
 public async Task<IActionResult> Open(OpenTrackingAllocationDto input){if(!ModelState.IsValid)return View("Index",await service.GetPageAsync(input));try{await service.OpenAsync(input,User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException("Authenticated user was not found."));TempData["NotificationType"]="success";TempData["NotificationMessage"]="Operation completed successfully.";return RedirectToAction(nameof(Index));}catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,ex.Message);return View("Index",await service.GetPageAsync(input));}}
}
