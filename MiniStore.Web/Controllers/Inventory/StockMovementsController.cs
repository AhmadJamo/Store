using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class StockMovementsController(StockMovementService service) : Controller
{
    [HttpGet]
    [PermissionAuthorize("StockMovements.View")]
    public async Task<IActionResult> Index() => View(await service.GetPageAsync());
}
