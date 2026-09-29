using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Inventory.Reconciliation;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class InventoryReconciliationController(
    InventoryReconciliationService service) : Controller
{
    [HttpGet]
    [PermissionAuthorize("InventoryReconciliation.View")]
    public async Task<IActionResult> Index(
        int? warehouseId,
        string? search,
        bool exceptionsOnly = true,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var model = await service.SearchAsync(
            new InventoryReconciliationQueryDto
            {
                WarehouseId = warehouseId,
                Search = search,
                ExceptionsOnly = exceptionsOnly,
                Page = page,
                PageSize = pageSize
            },
            cancellationToken);

        return View(model);
    }
}
