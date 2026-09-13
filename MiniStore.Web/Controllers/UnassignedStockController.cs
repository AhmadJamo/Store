using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers;
public class UnassignedStockController(UnassignedStockService service, IWarehouseRepository warehouses, IStorageLocationRepository locations) : Controller
{
    [HttpGet, PermissionAuthorize("ProductStock.View")]
    public async Task<IActionResult> Index(int? warehouseId, string? query, string? sort)
    {
        ViewBag.Warehouses=await warehouses.GetAllAsync(); ViewBag.Locations=await locations.SearchAsync(null,null); ViewBag.WarehouseId=warehouseId; ViewBag.Query=query; ViewBag.Sort=sort;
        return View(await service.SearchAsync(warehouseId,query,sort));
    }
    [HttpPost, PermissionAuthorize("ProductStock.Edit")]
    public async Task<IActionResult> Assign(int productId,int warehouseId,int storageLocationId,decimal quantity)
    {
        try { await service.AssignAsync(productId,warehouseId,storageLocationId,quantity); TempData["NotificationType"]="success"; TempData["NotificationMessage"]="Stock location saved."; }
        catch(Exception ex) when(ex is ArgumentException or InvalidOperationException){TempData["NotificationType"]="error";TempData["NotificationMessage"]=ex.Message;}
        return RedirectToAction(nameof(Index),new{warehouseId});
    }
}
