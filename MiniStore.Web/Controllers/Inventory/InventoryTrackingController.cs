using System.Security.Claims;using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.DTOs.Inventory.Tracking;using MiniStore.Application.Services;using MiniStore.Domain.Entities;using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers;
public sealed class InventoryTrackingController(InventoryTrackingService service,IStringLocalizer<SharedResource> localizer):Controller
{
 [HttpGet,PermissionAuthorize("InventoryTracking.View")]public async Task<IActionResult> Index()=>View(await service.GetPageAsync());
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("InventoryTracking.Open")]
 public async Task<IActionResult> Open(OpenTrackingAllocationDto input){if(!ModelState.IsValid)return View("Index",await service.GetPageAsync(input));try{await service.OpenAsync(input,User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException("Authenticated user was not found."));TempData["NotificationType"]="success";TempData["NotificationMessage"]="Operation completed successfully.";return RedirectToAction(nameof(Index));}catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,ex.Message);return View("Index",await service.GetPageAsync(input));}}
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("InventoryTracking.ManageQuarantine")]
 public async Task<IActionResult> Quarantine(long balanceId,string reason)=>await ChangeStatus(balanceId,reason,true);
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("InventoryTracking.ManageQuarantine")]
 public async Task<IActionResult> Release(long balanceId,string reason)=>await ChangeStatus(balanceId,reason,false);
 private async Task<IActionResult> ChangeStatus(long balanceId,string reason,bool quarantine)
 {
  try{var userId=User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException("Authenticated user was not found.");if(quarantine)await service.QuarantineAsync(balanceId,reason,userId);else await service.ReleaseAsync(balanceId,reason,userId);TempData["NotificationType"]="success";TempData["NotificationMessage"]=localizer["Operation completed successfully."].Value;}
  catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){TempData["NotificationType"]="danger";TempData["NotificationMessage"]=localizer[ex.Message].Value;}
  return RedirectToAction(nameof(Index));
 }
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("InventoryTracking.ManageRecall")]
 public async Task<IActionResult> CreateRecall(string reference,int productId,string identifier,string reason)
 {
  try{await service.CreateRecallAsync(reference,productId,identifier,reason,User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException("Authenticated user was not found."));TempData["NotificationType"]="success";TempData["NotificationMessage"]=localizer["Recall created and available stock quarantined."].Value;}
  catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){TempData["NotificationType"]="danger";TempData["NotificationMessage"]=localizer[ex.Message].Value;}
  return RedirectToAction(nameof(Index));
 }
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("InventoryTracking.ManageRecall")]
 public async Task<IActionResult> CloseRecall(long recallId,string notes)
 {
  try{await service.CloseRecallAsync(recallId,notes,User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException("Authenticated user was not found."));TempData["NotificationType"]="success";TempData["NotificationMessage"]=localizer["Recall closed. Quarantined stock remains blocked until explicitly released."].Value;}
  catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){TempData["NotificationType"]="danger";TempData["NotificationMessage"]=localizer[ex.Message].Value;}
  return RedirectToAction(nameof(Index));
 }
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("InventoryTracking.ManageRecall")]
 public async Task<IActionResult> RecordRecallCommunication(long recallId,string partyName,string? channelAddress,RecallCommunicationChannel channel,RecallCommunicationOutcome outcome,string notes)
 {
  try{await service.RecordRecallCommunicationAsync(recallId,partyName,channelAddress,channel,outcome,notes,User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException("Authenticated user was not found."));TempData["NotificationType"]="success";TempData["NotificationMessage"]=localizer["Recall communication recorded."].Value;}
  catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){TempData["NotificationType"]="danger";TempData["NotificationMessage"]=localizer[ex.Message].Value;}
  return RedirectToAction(nameof(Index));
 }
}
