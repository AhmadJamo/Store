using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;
namespace MiniStore.Web.Controllers.Api.V1;
[ApiController,Authorize,Route("api/v1/purchase-orders")]
public sealed class PurchaseOrdersApiController(PurchaseOrderService service,IStringLocalizer<SharedResource> localizer):ControllerBase
{
 [HttpGet,PermissionAuthorize("PurchaseOrders.View")]public async Task<ActionResult<List<PurchaseOrderDto>>>GetAll(PurchaseOrderStatus?status,string?search)=>Ok(await service.GetAllAsync(status,search));
 [HttpGet("{id:int}"),PermissionAuthorize("PurchaseOrders.View")]public async Task<ActionResult<PurchaseOrderDto>>Get(int id){var x=await service.GetAsync(id);return x is null?NotFound():Ok(x);}
 [HttpPost("from-award"),ValidateAntiForgeryToken,PermissionAuthorize("PurchaseOrders.Create")]public async Task<IActionResult>CreateFromAward(CreatePurchaseOrdersFromAwardDto input)=>await Run(async()=>Ok(new{ids=await service.CreateOrdersFromAwardAsync(input.SourcingEventId)}));
 [HttpPost("{id:int}/approve"),ValidateAntiForgeryToken,PermissionAuthorize("PurchaseOrders.Approve")]public Task<IActionResult>Approve(int id,PurchaseOrderCommandDto input)=>Command(id,input,()=>service.ApproveAsync(id,input.RowVersion));
 [HttpPost("{id:int}/confirm"),ValidateAntiForgeryToken,PermissionAuthorize("PurchaseOrders.Confirm")]public Task<IActionResult>Confirm(int id,PurchaseOrderCommandDto input)=>Command(id,input,async()=>{await service.ConfirmAsync(id,input.RowVersion);});
 [HttpPost("{id:int}/cancel"),ValidateAntiForgeryToken,PermissionAuthorize("PurchaseOrders.Cancel")]public Task<IActionResult>Cancel(int id,PurchaseOrderCommandDto input)=>Command(id,input,()=>service.CancelAsync(id,input.Reason??string.Empty,input.RowVersion));
 [HttpPost("{id:int}/close-remaining"),ValidateAntiForgeryToken,PermissionAuthorize("PurchaseOrders.Close")]public Task<IActionResult>CloseRemaining(int id,PurchaseOrderCommandDto input)=>Command(id,input,()=>service.CloseRemainingAsync(id,input.Reason??string.Empty,input.RowVersion));
 [HttpPost("{id:int}/reopen"),ValidateAntiForgeryToken,PermissionAuthorize("PurchaseOrders.Reopen")]public Task<IActionResult>Reopen(int id,PurchaseOrderCommandDto input)=>Command(id,input,()=>service.ReopenAsync(id,input.Reason??string.Empty,input.RowVersion));
 [HttpPost("{id:int}/dropship"),ValidateAntiForgeryToken,PermissionAuthorize("PurchaseOrders.ConfigureFulfillment")]public Task<IActionResult>ConfigureDropship(int id,ConfigurePurchaseDropshipDto input)=>id==input.PurchaseOrderId?Run(async()=>{await service.ConfigureDropshipAsync(input);return NoContent();}):Task.FromResult(Validation(new ArgumentException("Route and payload identifiers do not match.")));
 [HttpPost("{id:int}/direct-delivery"),ValidateAntiForgeryToken,PermissionAuthorize("PurchaseOrders.RecordDirectDelivery")]public Task<IActionResult>RecordDirectDelivery(int id,RecordPurchaseDirectDeliveryDto input)=>id==input.PurchaseOrderId?Run(async()=>{await service.RecordDirectDeliveryAsync(input);return NoContent();}):Task.FromResult(Validation(new ArgumentException("Route and payload identifiers do not match.")));
 [HttpPost("{id:int}/supplier-confirmation"),ValidateAntiForgeryToken,PermissionAuthorize("PurchaseOrders.RecordSupplierConfirmation")]public Task<IActionResult>RecordSupplierConfirmation(int id,RecordPurchaseOrderSupplierConfirmationDto input)=>id==input.PurchaseOrderId?Run(async()=>{await service.RecordSupplierConfirmationAsync(input);return NoContent();}):Task.FromResult(Validation(new ArgumentException("Route and payload identifiers do not match.")));
 private Task<IActionResult>Command(int id,PurchaseOrderCommandDto input,Func<Task>action)=>id==input.Id?Run(async()=>{await action();return NoContent();}):Task.FromResult(Validation(new ArgumentException("Route and payload identifiers do not match.")));
 private async Task<IActionResult>Run(Func<Task<IActionResult>>action){try{return await action();}catch(Exception e)when(e is ArgumentException or InvalidOperationException){return Validation(e);}}
 private IActionResult Validation(Exception e){ModelState.AddModelError(string.Empty,localizer[e.Message]);return InternalApiProblemFactory.Validation(HttpContext,ModelState,localizer["One or more validation errors occurred."],InternalApiProblemCodes.Validation);}
}
