using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class WarehousesController : Controller
{
    private readonly WarehouseService _warehouseService;

    public WarehousesController(
        WarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpGet]
    [PermissionAuthorize("Warehouses.View")]
    public async Task<IActionResult> Index()
    {
        var warehouses =
            await _warehouseService.GetAllAsync();

        return View(warehouses);
    }

    [HttpGet]
    [PermissionAuthorize("Warehouses.Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [PermissionAuthorize("Warehouses.Create")]
    public async Task<IActionResult> Create(
        CreateWarehouseDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "Please check the entered data.";

            return View(dto);
        }

        try
        {
            await _warehouseService.CreateAsync(dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Warehouse created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                ex.Message;

            return View(dto);
        }
    }

    [HttpGet]
    [PermissionAuthorize("Warehouses.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var warehouse =
            await _warehouseService.GetByIdAsync(id);

        if (warehouse == null)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "Warehouse not found.";

            return RedirectToAction(nameof(Index));
        }

        var dto = new UpdateWarehouseDto
        {
            Name = warehouse.Name
        };

        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize("Warehouses.Edit")]
    public async Task<IActionResult> Edit(
        int id,
        UpdateWarehouseDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "Please check the entered data.";

            return View(dto);
        }

        try
        {
            await _warehouseService.UpdateAsync(id, dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Warehouse updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                ex.Message;

            return View(dto);
        }
    }

    [HttpPost]
    [PermissionAuthorize("Warehouses.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _warehouseService.DeleteAsync(id);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Warehouse deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}