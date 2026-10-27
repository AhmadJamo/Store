using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.DTOs.Warehouses;using MiniStore.Application.Services;using MiniStore.Web.Authorization;using MiniStore.Web.Middleware.Api;
namespace MiniStore.Web.Controllers.Api.V1;
[ApiController,Authorize,Route("api/v1/warehouse-locations")]
public sealed class WarehouseLocationsApiController(StorageLocationService locations,IStringLocalizer<SharedResource> localizer):ControllerBase
{
 [HttpGet,PermissionAuthorize("Warehouses.View")]public async Task<ActionResult<WarehouseLocationApiPageDto>> GetPage(int? warehouseId,string? query)=>Ok(new WarehouseLocationApiPageDto{Result=await locations.SearchAsync(warehouseId,query),Options=await locations.GetPageOptionsAsync()});
 [HttpGet("{id:int}/form"),PermissionAuthorize("Warehouses.Edit")]public async Task<ActionResult<WarehouseLocationEditApiPageDto>> GetEditForm(int id){var input=await locations.GetForEditAsync(id);return input is null?NotFound():Ok(new WarehouseLocationEditApiPageDto{Input=input,ParentLocations=(await locations.GetParentOptionsAsync(input.WarehouseId)).Where(x=>x.Id!=id).ToList()});}
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("Warehouses.Create")]public Task<IActionResult> Create(CreateStorageLocationDto input)=>Execute(()=>locations.CreateAsync(input));
 [HttpPut("{id:int}"),ValidateAntiForgeryToken,PermissionAuthorize("Warehouses.Edit")]public Task<IActionResult> Update(int id,EditStorageLocationDto input){if(id!=input.Id){ModelState.AddModelError(nameof(input.Id),localizer["Route and payload identifiers do not match."]);return Task.FromResult(Validation());}return Execute(()=>locations.UpdateAsync(input));}
 private async Task<IActionResult> Execute(Func<Task> action){try{await action();return NoContent();}catch(Exception exception)when(exception is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,localizer[exception.Message]);return Validation();}}
 private IActionResult Validation()=>InternalApiProblemFactory.Validation(HttpContext,ModelState,localizer["One or more validation errors occurred."],InternalApiProblemCodes.Validation);
}
