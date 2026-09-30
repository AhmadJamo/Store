using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;
using Microsoft.Extensions.Localization;

namespace MiniStore.Web.Controllers;

public class WarehouseLocationsController(
    StorageLocationService service,
    IWarehouseRepository warehouseRepository,
    IStringLocalizer<SharedResource> localizer) : Controller
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

    [HttpGet]
    [PermissionAuthorize("Warehouses.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await service.GetForEditAsync(id);
        if (dto is null)
        {
            return NotFound();
        }

        await LoadParentLocationsAsync(dto.WarehouseId, dto.Id);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Warehouses.Edit")]
    public async Task<IActionResult> Edit(EditStorageLocationDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadParentLocationsAsync(dto.WarehouseId, dto.Id);
            return View(dto);
        }

        try
        {
            await service.UpdateAsync(dto);
            TempData["Success"] = localizer["Storage location updated successfully."].Value;
            return RedirectToAction(nameof(Index), new { warehouseId = dto.WarehouseId });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            await LoadParentLocationsAsync(dto.WarehouseId, dto.Id);
            return View(dto);
        }
    }

    private async Task LoadWarehousesAsync()
    {
        ViewBag.Warehouses = (await warehouseRepository.GetAllAsync())
            .Where(warehouse => warehouse.ControlMode != MiniStore.Domain.Entities.InventoryControlMode.Simple)
            .ToList();
        ViewBag.ParentLocations = (await service.SearchAsync(null, null)).Locations;
    }

    private async Task LoadParentLocationsAsync(int warehouseId, int? excludedId = null)
    {
        ViewBag.ParentLocations = (await service.GetParentOptionsAsync(warehouseId))
            .Where(location => location.Id != excludedId)
            .ToList();
    }
}
