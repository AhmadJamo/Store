using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.DTOs.Purchases;using MiniStore.Application.Services;using MiniStore.Web.Authorization;using MiniStore.Web.Middleware.Api;
namespace MiniStore.Web.Controllers.Api.V1;
[ApiController,Authorize,Route("api/v1/landed-costs")]
public sealed class LandedCostsApiController(LandedCostService service,IStringLocalizer<SharedResource> localizer):ControllerBase{
[HttpGet,PermissionAuthorize("LandedCosts.View")]public async Task<ActionResult<List<LandedCostDto>>>GetAll()=>Ok(await service.GetAllAsync());
[HttpGet("{id:int}"),PermissionAuthorize("LandedCosts.View")]public async Task<ActionResult<LandedCostDto>>Get(int id){var x=await service.GetAsync(id);return x is null?NotFound():Ok(x);}
[HttpGet("create-form"),PermissionAuthorize("LandedCosts.Create")]public async Task<IActionResult>GetCreateForm(int goodsReceiptId){var x=await service.GetCreatePageAsync(goodsReceiptId);return string.IsNullOrWhiteSpace(x.ReceiptNumber)?NotFound():Ok(x);}
[HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("LandedCosts.Create")]public async Task<IActionResult>Create(CreateLandedCostDto input){try{return Ok(new{id=await service.CreateAndPostAsync(input)});}catch(Exception e)when(e is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,localizer[e.Message]);return InternalApiProblemFactory.Validation(HttpContext,ModelState,localizer["One or more validation errors occurred."],InternalApiProblemCodes.Validation);}}
}
