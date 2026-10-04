using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class InventoryBalancesController(InventoryBalanceService service) : Controller
{
    [HttpGet]
    [PermissionAuthorize("InventoryBalances.View")]
    public async Task<IActionResult> Index(int? warehouseId, string? search) =>
        View(await service.GetPageAsync(warehouseId, search));
}
