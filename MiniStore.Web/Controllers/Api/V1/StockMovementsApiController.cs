using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using MiniStore.Application.DTOs.StockMovements;using MiniStore.Application.Services;using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers.Api.V1;
[ApiController,Authorize,Route("api/v1/stock-movements")]
public sealed class StockMovementsApiController(StockMovementService movements):ControllerBase
{
 [HttpGet,PermissionAuthorize("StockMovements.View")]public async Task<ActionResult<StockMovementPageDto>> GetPage()=>Ok(await movements.GetPageAsync());
}
