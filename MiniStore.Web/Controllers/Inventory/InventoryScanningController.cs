using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Inventory.Scanning;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class InventoryScanningController(InventoryScanningService service) : Controller
{
    [HttpGet]
    [PermissionAuthorize("InventoryScanning.View")]
    public async Task<IActionResult> Index(string? productScan, string? locationScan) =>
        View(await service.ResolveAsync(new InventoryScanQueryDto
        {
            ProductScan = productScan ?? string.Empty,
            LocationScan = locationScan ?? string.Empty
        }));
}
