using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.DTOs.Warehouses;using MiniStore.Application.Services;using MiniStore.Web.Authorization;using MiniStore.Web.Middleware.Api;
namespace MiniStore.Web.Controllers.Api.V1;
[ApiController,Authorize,Route("api/v1/unassigned-stock")]
public sealed class UnassignedStockApiController(UnassignedStockService stock,PutawayRuleService rules,IStringLocalizer<SharedResource> localizer):ControllerBase
{
 [HttpGet,PermissionAuthorize("ProductStock.View")]public async Task<ActionResult<UnassignedStockPageDto>> GetPage(int? warehouseId,string? query,string? sort)=>Ok(await stock.GetPageAsync(warehouseId,query,sort));
 [HttpPost("assign"),ValidateAntiForgeryToken,PermissionAuthorize("ProductStock.Edit")]public Task<IActionResult> Assign(AssignUnassignedStockDto input)=>Execute(()=>stock.AssignAsync(input.ProductId,input.WarehouseId,input.StorageLocationId,input.Quantity,input.IdempotencyKey));
 [HttpPost("rules"),ValidateAntiForgeryToken,PermissionAuthorize("ProductStock.Edit")]public Task<IActionResult> CreateRule(CreatePutawayRuleDto input)=>Execute(()=>rules.CreateAsync(input));
 [HttpPut("rules/{id:int}/active"),ValidateAntiForgeryToken,PermissionAuthorize("ProductStock.Edit")]public Task<IActionResult> SetRuleActive(int id,SetPutawayRuleActiveDto input)=>Execute(()=>rules.SetActiveAsync(id,input.Active));
 private async Task<IActionResult> Execute(Func<Task> action){try{await action();return NoContent();}catch(Exception exception)when(exception is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,localizer[exception.Message]);return InternalApiProblemFactory.Validation(HttpContext,ModelState,localizer["One or more validation errors occurred."],InternalApiProblemCodes.Validation);}}
}
