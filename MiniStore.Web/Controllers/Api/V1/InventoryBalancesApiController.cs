using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Inventory.Balances;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/inventory-balances")]
public sealed class InventoryBalancesApiController(InventoryBalanceService balances) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("InventoryBalances.View")]
    public async Task<ActionResult<InventoryBalancePagedResultDto>> GetPage(
        int? warehouseId, string? search, int page = 1, int pageSize = 50) =>
        Ok(await balances.GetPagedAsync(warehouseId, search, page, pageSize));
}
