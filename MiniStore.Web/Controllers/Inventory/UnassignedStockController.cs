using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class UnassignedStockController(
    UnassignedStockService service,
    IWarehouseRepository warehouseRepository,
    IStorageLocationRepository storageLocationRepository) : Controller
{
    [HttpGet]
    [PermissionAuthorize("ProductStock.View")]
    public async Task<IActionResult> Index(int? warehouseId, string? query, string? sort)
    {
        ViewBag.Warehouses = await warehouseRepository.GetAllAsync();
        ViewBag.Locations = await storageLocationRepository.SearchAsync(null, null);
        ViewBag.WarehouseId = warehouseId;
        ViewBag.Query = query;
        ViewBag.Sort = sort;

        var model = await service.SearchAsync(warehouseId, query, sort);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("ProductStock.Edit")]
    public async Task<IActionResult> Assign(
        int productId,
        int warehouseId,
        int storageLocationId,
        decimal quantity)
    {
        try
        {
            await service.AssignAsync(
                productId,
                warehouseId,
                storageLocationId,
                quantity);

            SetNotification("success", "Stock location saved.");
        }
        catch (Exception exception)
            when (exception is ArgumentException or InvalidOperationException)
        {
            SetNotification("error", exception.Message);
        }

        return RedirectToAction(nameof(Index), new { warehouseId });
    }

    private void SetNotification(string type, string message)
    {
        TempData["NotificationType"] = type;
        TempData["NotificationMessage"] = message;
    }
}
