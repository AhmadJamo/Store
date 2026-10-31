using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Inventory.Tracking;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/inventory-tracking")]
public sealed class InventoryTrackingApiController(InventoryTrackingService tracking,IStringLocalizer<SharedResource> localizer):ControllerBase
{
 [HttpGet,PermissionAuthorize("InventoryTracking.View")]
 public async Task<ActionResult<InventoryTrackingApiPageDto>> GetPage()
 {
  var page=await tracking.GetPageAsync();
  return Ok(new InventoryTrackingApiPageDto{Input=page.Input,Products=page.Products.Select(x=>new InventoryTrackingOptionDto(x.Id,x.Name)).ToList(),TrackedProducts=page.TrackedProducts.Select(x=>new InventoryTrackingOptionDto(x.Id,x.Name)).ToList(),Warehouses=page.Warehouses.Select(x=>new InventoryTrackingOptionDto(x.Id,x.Name)).ToList(),Locations=page.Locations.Select(x=>new InventoryTrackingLocationOptionDto(x.Id,x.WarehouseId,x.Name)).ToList(),Balances=page.Balances,Transactions=page.Transactions,ExpiredCount=page.ExpiredCount,ExpiringSoonCount=page.ExpiringSoonCount,ExpirationAlerts=page.ExpirationAlerts,Recalls=page.Recalls});
 }
 [HttpPost("open"),ValidateAntiForgeryToken,PermissionAuthorize("InventoryTracking.Open")]
 public Task<IActionResult> Open(OpenTrackingAllocationDto input)=>Execute(()=>tracking.OpenAsync(input,UserId()));
 [HttpPost("balances/{id:long}/quarantine"),ValidateAntiForgeryToken,PermissionAuthorize("InventoryTracking.ManageQuarantine")]
 public Task<IActionResult> Quarantine(long id,TrackingReasonDto input)=>Execute(()=>tracking.QuarantineAsync(id,input.Reason,UserId()));
 [HttpPost("balances/{id:long}/release"),ValidateAntiForgeryToken,PermissionAuthorize("InventoryTracking.ManageQuarantine")]
 public Task<IActionResult> Release(long id,TrackingReasonDto input)=>Execute(()=>tracking.ReleaseAsync(id,input.Reason,UserId()));
 [HttpPost("recalls"),ValidateAntiForgeryToken,PermissionAuthorize("InventoryTracking.ManageRecall")]
 public Task<IActionResult> CreateRecall(CreateInventoryRecallDto input)=>Execute(()=>tracking.CreateRecallAsync(input.Reference,input.ProductId,input.Identifier,input.Reason,UserId()));
 [HttpPost("recalls/{id:long}/close"),ValidateAntiForgeryToken,PermissionAuthorize("InventoryTracking.ManageRecall")]
 public Task<IActionResult> CloseRecall(long id,CloseInventoryRecallDto input)=>Execute(()=>tracking.CloseRecallAsync(id,input.Notes,UserId()));
 [HttpPost("recalls/{id:long}/communications"),ValidateAntiForgeryToken,PermissionAuthorize("InventoryTracking.ManageRecall")]
 public Task<IActionResult> RecordCommunication(long id,RecordRecallCommunicationDto input)=>Execute(()=>tracking.RecordRecallCommunicationAsync(id,input.PartyName,input.ChannelAddress,input.Channel,input.Outcome,input.Notes,UserId()));
 private async Task<IActionResult> Execute(Func<Task> action){try{await action();return NoContent();}catch(Exception exception)when(exception is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,localizer[exception.Message]);return InternalApiProblemFactory.Validation(HttpContext,ModelState,localizer["One or more validation errors occurred."],InternalApiProblemCodes.Validation);}}
 private string UserId()=>User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException("Authenticated user was not found.");
}
