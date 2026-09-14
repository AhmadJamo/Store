using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class WarehouseLocationsController(
    StorageLocationService service,
    IWarehouseRepository warehouseRepository) : Controller
{
    [HttpGet]
    [PermissionAuthorize("Warehouses.View")]
    public async Task<IActionResult> Index(int? warehouseId, string? query)
    {
        await LoadWarehousesAsync();

        ViewBag.WarehouseId = warehouseId;
        ViewBag.Query = query;

        var model = await service.SearchAsync(warehouseId, query);
        return View(model);
    }

    [HttpGet]
    [PermissionAuthorize("Warehouses.Create")]
    public async Task<IActionResult> Create()
    {
        await LoadWarehousesAsync();
        return View(new CreateStorageLocationDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Warehouses.Create")]
    public async Task<IActionResult> Create(CreateStorageLocationDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadWarehousesAsync();
            return View(dto);
        }

        try
        {
            await service.CreateAsync(dto);
            return RedirectToAction(nameof(Index), new { warehouseId = dto.WarehouseId });
        }
        catch (Exception exception)
            when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadWarehousesAsync();
            return View(dto);
        }
    }

    private async Task LoadWarehousesAsync()
    {
        ViewBag.Warehouses = (await warehouseRepository.GetAllAsync())
            .Where(warehouse => warehouse.ControlMode != MiniStore.Domain.Entities.InventoryControlMode.Simple)
            .ToList();
    }
}
