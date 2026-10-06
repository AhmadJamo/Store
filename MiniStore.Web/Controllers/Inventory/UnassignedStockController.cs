using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class UnassignedStockController(
    UnassignedStockService service,
    PutawayRuleService putawayRuleService,
    IWarehouseRepository warehouseRepository,
    IStorageLocationRepository storageLocationRepository,
    IProductRepository productRepository,
    IProductCategoryRepository productCategoryRepository,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet]
    [PermissionAuthorize("ProductStock.View")]
    public async Task<IActionResult> Index(int? warehouseId, string? query, string? sort)
    {
        ViewBag.Warehouses = (await warehouseRepository.GetAllAsync())
            .Where(warehouse => warehouse.ControlMode != MiniStore.Domain.Entities.InventoryControlMode.Simple)
            .ToList();
        ViewBag.Locations = await storageLocationRepository.SearchAsync(null, null);
        ViewBag.Products = await productRepository.GetAllAsync(null);
        ViewBag.Categories = await productCategoryRepository.GetAllAsync();
        ViewBag.PutawayRules = await putawayRuleService.GetAllAsync();
        ViewBag.WarehouseId = warehouseId;
        ViewBag.Query = query;
        ViewBag.Sort = sort;

        var model = await service.SearchAsync(warehouseId, query, sort);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("ProductStock.Edit")]
    public async Task<IActionResult> CreateRule(CreatePutawayRuleDto input)
    {
        try { await putawayRuleService.CreateAsync(input); SetNotification("success", localizer["Putaway rule saved."].Value); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { SetNotification("danger", localizer[exception.Message].Value); }
        return RedirectToAction(nameof(Index), new { warehouseId = input.WarehouseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("ProductStock.Edit")]
    public async Task<IActionResult> SetRuleActive(int id, bool active, int? warehouseId)
    {
        try { await putawayRuleService.SetActiveAsync(id, active); SetNotification("success", localizer["Putaway rule updated."].Value); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { SetNotification("danger", localizer[exception.Message].Value); }
        return RedirectToAction(nameof(Index), new { warehouseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("ProductStock.Edit")]
    public async Task<IActionResult> Assign(
        int productId,
        int warehouseId,
        int storageLocationId,
        decimal quantity,
        string idempotencyKey)
    {
        try
        {
            await service.AssignAsync(
                productId,
                warehouseId,
                storageLocationId,
                quantity,
                idempotencyKey);

            SetNotification("success", localizer["Stock location saved."].Value);
        }
        catch (Exception exception)
            when (exception is ArgumentException or InvalidOperationException)
        {
            SetNotification("danger", localizer[exception.Message].Value);
        }

        return RedirectToAction(nameof(Index), new { warehouseId });
    }

    private void SetNotification(string type, string message)
    {
        TempData["NotificationType"] = type;
        TempData["NotificationMessage"] = message;
    }
}
