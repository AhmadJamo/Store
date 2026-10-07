using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Inventory.Scanning;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using Microsoft.Extensions.Localization;

namespace MiniStore.Web.Controllers;

public sealed class InventoryScanningController(
    InventoryScanningService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet]
    [PermissionAuthorize("InventoryScanning.View")]
    public async Task<IActionResult> Index(string? adjustmentScan, string? productScan, string? sourceLocationScan, string? locationScan) =>
        View(await service.ResolveAsync(new InventoryScanQueryDto
        {
            AdjustmentScan = adjustmentScan ?? string.Empty,
            ProductScan = productScan ?? string.Empty,
            SourceLocationScan = sourceLocationScan ?? string.Empty,
            LocationScan = locationScan ?? string.Empty
        }));

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("InventoryScanning.View")]
    [PermissionAuthorize("ProductStock.Edit")]
    public async Task<IActionResult> Putaway(ScannedPutawayDto input)
    {
        try
        {
            await service.ExecutePutawayAsync(input);
            Notify("success", "Scanned putaway posted.");
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            Notify("danger", exception.Message);
        }

        return RedirectToAction(nameof(Index), new
        {
            productScan = input.ProductScan,
            locationScan = input.LocationScan
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("InventoryScanning.View")]
    [PermissionAuthorize("InventoryAdjustments.Count")]
    public async Task<IActionResult> Count(ScannedInventoryCountDto input)
    {
        try
        {
            await service.RecordCountAsync(input);
            Notify("success", "Scanned count recorded.");
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            Notify("danger", exception.Message);
        }

        return RedirectToAction(nameof(Index), new
        {
            adjustmentScan = input.AdjustmentNumber,
            productScan = input.ProductScan,
            locationScan = input.LocationScan
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("InventoryScanning.View")]
    [PermissionAuthorize("LocationMovements.Create")]
    public async Task<IActionResult> Relocate(ScannedRelocationDto input)
    {
        try
        {
            await service.ExecuteRelocationAsync(input);
            Notify("success", "Scanned relocation posted.");
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            Notify("danger", exception.Message);
        }

        return RedirectToAction(nameof(Index), new
        {
            productScan = input.ProductScan,
            sourceLocationScan = input.SourceLocationScan,
            locationScan = input.DestinationLocationScan
        });
    }

    private void Notify(string type, string message)
    {
        TempData["NotificationType"] = type;
        TempData["NotificationMessage"] = localizer[message].Value;
    }
}
