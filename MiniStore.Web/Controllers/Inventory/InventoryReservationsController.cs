using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class InventoryReservationsController(InventoryReservationService service) : Controller
{
    [HttpGet]
    [PermissionAuthorize("InventoryReservations.View")]
    public async Task<IActionResult> Index(InventoryReservationStatus? status, string? search) =>
        View(await service.GetPageAsync(status, search));
}
