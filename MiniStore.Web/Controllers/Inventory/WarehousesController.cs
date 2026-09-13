using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class WarehousesController : Controller
{
    private readonly WarehouseService _warehouseService;
    private readonly BranchService _branchService;
    private readonly AccountService _accountService;

    public WarehousesController(
        WarehouseService warehouseService, BranchService branchService, AccountService accountService)
    {
        _warehouseService = warehouseService;
        _branchService = branchService;
        _accountService = accountService;
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
    public async Task<IActionResult> Create()
    {
        await LoadAccountingOptions();
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

            await LoadAccountingOptions(); return View(dto);
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
            HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType()).LogError(ex, "Request operation failed");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "The operation could not be completed. Please try again or contact the administrator.";

            await LoadAccountingOptions(); return View(dto);
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
            , BranchId = warehouse.BranchId ?? 0, InventoryAccountId = warehouse.InventoryAccountId ?? 0
        };

        await LoadAccountingOptions(); return View(dto);
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

            await LoadAccountingOptions(); return View(dto);
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
            HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType()).LogError(ex, "Request operation failed");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "The operation could not be completed. Please try again or contact the administrator.";

            await LoadAccountingOptions(); return View(dto);
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
            HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType()).LogError(ex, "Request operation failed");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "The operation could not be completed. Please try again or contact the administrator.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadAccountingOptions()
    {
        ViewBag.Branches = await _branchService.GetAllAsync();
        ViewBag.Accounts = await _accountService.GetAllAsync();
    }
}
