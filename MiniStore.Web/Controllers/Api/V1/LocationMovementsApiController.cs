using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.DTOs.LocationMovements;using MiniStore.Application.Services;using MiniStore.Web.Authorization;using MiniStore.Web.Middleware.Api;
namespace MiniStore.Web.Controllers.Api.V1;
[ApiController,Authorize,Route("api/v1/location-movements")]
public sealed class LocationMovementsApiController(LocationMovementService movements,IStringLocalizer<SharedResource> localizer):ControllerBase
{
 [HttpGet,PermissionAuthorize("LocationMovements.View")]public async Task<ActionResult<LocationMovementPageDto>> GetPage(int? warehouseId,string? query)=>Ok(await movements.GetPageAsync(warehouseId,query));
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("LocationMovements.Create")]public async Task<IActionResult> Move(CreateLocationMovementDto input){try{await movements.MoveAsync(input);return NoContent();}catch(Exception exception)when(exception is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,localizer[exception.Message]);return InternalApiProblemFactory.Validation(HttpContext,ModelState,localizer["One or more validation errors occurred."],InternalApiProblemCodes.Validation);}}
}
