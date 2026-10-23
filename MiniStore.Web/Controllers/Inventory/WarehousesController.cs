using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

[AutoValidateAntiforgeryToken]
public class WarehousesController : Controller
{
    private readonly WarehouseService _warehouseService;
    private readonly BranchService _branchService;
    private readonly AccountService _accountService;
    private readonly InventorySettingsService _inventorySettingsService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public WarehousesController(
        WarehouseService warehouseService,
        BranchService branchService,
        AccountService accountService,
        InventorySettingsService inventorySettingsService,
        IStringLocalizer<SharedResource> localizer)
    {
        _warehouseService = warehouseService;
        _branchService = branchService;
        _accountService = accountService;
        _inventorySettingsService = inventorySettingsService;
        _localizer = localizer;
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
        var defaults = await _inventorySettingsService.GetAsync();
        return View(new CreateWarehouseDto
        {
            Type = defaults.DefaultWarehouseType,
            ControlMode = defaults.DefaultControlMode,
            PickingStrategy = defaults.DefaultPickingStrategy,
            AllowPosSales = defaults.DefaultAllowPosSales,
            EnforceLocationCapacity = defaults.DefaultEnforceLocationCapacity,
            RequireSourceLocationForTransfers = defaults.DefaultRequireSourceLocationForTransfers,
            RequireDestinationLocationForTransfers = defaults.DefaultRequireDestinationLocationForTransfers
        });
    }

    [HttpPost]
    [PermissionAuthorize("Warehouses.Create")]
    public async Task<IActionResult> Create(
        CreateWarehouseDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = _localizer["Please check the entered data."].Value;

            await LoadAccountingOptions(); return View(dto);
        }

        try
        {
            await _warehouseService.CreateAsync(dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = _localizer["Warehouse created successfully."].Value;

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType()).LogError(ex, "Request operation failed");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = _localizer["The operation could not be completed. Please try again or contact the administrator."].Value;

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
            TempData["NotificationMessage"] = _localizer["Warehouse not found."].Value;

            return RedirectToAction(nameof(Index));
        }

        var dto = new UpdateWarehouseDto
        {
            Name = warehouse.Name,
            BranchId = warehouse.BranchId ?? 0,
            InventoryAccountId = warehouse.InventoryAccountId ?? 0,
            Type = warehouse.Type,
            ControlMode = warehouse.ControlMode,
            PickingStrategy = warehouse.PickingStrategy,
            PurchaseReceivingRoute = warehouse.PurchaseReceivingRoute,
            AllowPosSales = warehouse.AllowPosSales,
            EnforceLocationCapacity = warehouse.EnforceLocationCapacity,
            RequireSourceLocationForTransfers = warehouse.RequireSourceLocationForTransfers,
            RequireDestinationLocationForTransfers = warehouse.RequireDestinationLocationForTransfers
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
            TempData["NotificationMessage"] = _localizer["Please check the entered data."].Value;

            await LoadAccountingOptions(); return View(dto);
        }

        try
        {
            await _warehouseService.UpdateAsync(id, dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = _localizer["Warehouse updated successfully."].Value;

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType()).LogError(ex, "Request operation failed");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = _localizer["The operation could not be completed. Please try again or contact the administrator."].Value;

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
            TempData["NotificationMessage"] = _localizer["Warehouse deleted successfully."].Value;
        }
        catch (Exception ex)
        {
            HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType()).LogError(ex, "Request operation failed");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = _localizer["The operation could not be completed. Please try again or contact the administrator."].Value;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadAccountingOptions()
    {
        ViewBag.Branches = await _branchService.GetAllAsync();
        ViewBag.Accounts = await _accountService.GetAllAsync();
    }
}
