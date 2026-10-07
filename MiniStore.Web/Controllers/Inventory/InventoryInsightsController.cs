using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Inventory.Insights;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class InventoryInsightsController(InventoryInsightsService service) : Controller
{
    [HttpGet]
    [PermissionAuthorize("InventoryInsights.View")]
    public async Task<IActionResult> Index(
        int? warehouseId,
        string? search,
        InventoryActivityState? state,
        bool attentionOnly = false,
        int slowDays = 30,
        int deadDays = 90)
    {
        try
        {
            return View(await service.GetPageAsync(new InventoryInsightsQueryDto
            {
                WarehouseId = warehouseId,
                Search = search ?? string.Empty,
                State = state,
                AttentionOnly = attentionOnly,
                SlowDays = slowDays,
                DeadDays = deadDays
            }));
        }
        catch (ArgumentException)
        {
            return View(await service.GetPageAsync(new InventoryInsightsQueryDto
            {
                WarehouseId = warehouseId,
                Search = search ?? string.Empty,
                State = state,
                AttentionOnly = attentionOnly
            }));
        }
    }
}
