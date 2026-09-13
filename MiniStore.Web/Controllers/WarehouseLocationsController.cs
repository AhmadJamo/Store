using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class WarehouseLocationsController(StorageLocationService service, IWarehouseRepository warehouses) : Controller
{
    [HttpGet, PermissionAuthorize("Warehouses.View")]
    public async Task<IActionResult> Index(int? warehouseId, string? query)
    {
        ViewBag.Warehouses = await warehouses.GetAllAsync(); ViewBag.WarehouseId = warehouseId; ViewBag.Query = query;
        return View(await service.SearchAsync(warehouseId, query));
    }

    [HttpGet, PermissionAuthorize("Warehouses.Create")]
    public async Task<IActionResult> Create() { ViewBag.Warehouses = await warehouses.GetAllAsync(); return View(new CreateStorageLocationDto()); }

    [HttpPost, PermissionAuthorize("Warehouses.Create")]
    public async Task<IActionResult> Create(CreateStorageLocationDto dto)
    {
        if (!ModelState.IsValid) { ViewBag.Warehouses = await warehouses.GetAllAsync(); return View(dto); }
        try { await service.CreateAsync(dto); return RedirectToAction(nameof(Index), new { warehouseId = dto.WarehouseId }); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { ModelState.AddModelError(string.Empty, ex.Message); ViewBag.Warehouses = await warehouses.GetAllAsync(); return View(dto); }
    }
}
