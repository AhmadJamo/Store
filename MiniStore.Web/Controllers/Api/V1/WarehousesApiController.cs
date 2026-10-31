using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;
[ApiController, Authorize, Route("api/v1/warehouses")]
public sealed class WarehousesApiController(WarehouseService warehouses, BranchService branches, AccountService accounts,
    InventorySettingsService settings, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("Warehouses.View")] public async Task<ActionResult<List<WarehouseDto>>> GetAll()=>Ok(await warehouses.GetAllAsync());
    [HttpGet("form"), PermissionAuthorize("Warehouses.Create")] public async Task<ActionResult<WarehouseFormPageDto>> GetCreateForm(){var defaults=await settings.GetAsync();return Ok(await Form(new WarehouseDto{Type=defaults.DefaultWarehouseType,ControlMode=defaults.DefaultControlMode,PickingStrategy=defaults.DefaultPickingStrategy,AllowPosSales=defaults.DefaultAllowPosSales,EnforceLocationCapacity=defaults.DefaultEnforceLocationCapacity,RequireSourceLocationForTransfers=defaults.DefaultRequireSourceLocationForTransfers,RequireDestinationLocationForTransfers=defaults.DefaultRequireDestinationLocationForTransfers,PurchaseReceivingRoute=Domain.Entities.PurchaseReceivingRoute.OneStep}));}
    [HttpGet("{id:int}/form"), PermissionAuthorize("Warehouses.Edit")] public async Task<ActionResult<WarehouseFormPageDto>> GetEditForm(int id){var form=await warehouses.GetByIdAsync(id);return form is null?InternalApiProblemFactory.Create(HttpContext,404,localizer["Warehouse not found."],InternalApiProblemCodes.NotFound):Ok(await Form(form));}
    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("Warehouses.Create")] public async Task<ActionResult<WarehouseDto>> Create(CreateWarehouseDto form)=>await Execute(async()=>{await warehouses.CreateAsync(form);return (await warehouses.GetAllAsync()).Single(x=>x.Name.Equals(form.Name.Trim(),StringComparison.OrdinalIgnoreCase));});
    [HttpPut("{id:int}"),ValidateAntiForgeryToken,PermissionAuthorize("Warehouses.Edit")] public async Task<ActionResult<WarehouseDto>> Update(int id,UpdateWarehouseDto form)=>await Execute(async()=>{await warehouses.UpdateAsync(id,form);return (await warehouses.GetByIdAsync(id))!;});
    [HttpDelete("{id:int}"),ValidateAntiForgeryToken,PermissionAuthorize("Warehouses.Delete")] public async Task<IActionResult> Delete(int id){try{await warehouses.DeleteAsync(id);return NoContent();}catch(Exception e)when(e is ArgumentException or InvalidOperationException){return Problem(e);}}
    private async Task<WarehouseFormPageDto> Form(WarehouseDto form)=>new(){Form=form,Branches=await branches.GetRowsAsync(),Accounts=(await accounts.GetAllAsync()).ToList()};
    private async Task<ActionResult<T>> Execute<T>(Func<Task<T>> action){try{return Ok(await action());}catch(Exception e)when(e is ArgumentException or InvalidOperationException){return Problem(e);}}
    private ActionResult Problem(Exception exception){ModelState.AddModelError(string.Empty,localizer[exception.Message]);return InternalApiProblemFactory.Validation(HttpContext,ModelState,localizer["One or more validation errors occurred."],InternalApiProblemCodes.Validation);}
}
