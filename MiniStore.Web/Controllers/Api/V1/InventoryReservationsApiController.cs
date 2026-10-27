using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Inventory.Reservations;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/inventory-reservations")]
public sealed class InventoryReservationsApiController(InventoryReservationService reservations) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("InventoryReservations.View")]
    public async Task<ActionResult<InventoryReservationPagedResultDto>> GetPage(
        InventoryReservationStatus? status, string? search, int page = 1, int pageSize = 50) =>
        Ok(await reservations.GetPagedAsync(status, search, page, pageSize));
}
