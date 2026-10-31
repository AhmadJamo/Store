using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.DTOs.Purchases;using MiniStore.Application.Services;using MiniStore.Web.Authorization;using MiniStore.Web.Middleware.Api;
namespace MiniStore.Web.Controllers.Api.V1;
[ApiController,Authorize,Route("api/v1/goods-receipt-returns")]
public sealed class GoodsReceiptReturnsApiController(GoodsReceiptReturnService service,IStringLocalizer<SharedResource> localizer):ControllerBase{
[HttpGet,PermissionAuthorize("GoodsReceiptReturns.View")]public async Task<ActionResult<List<GoodsReceiptReturnDto>>>GetAll()=>Ok(await service.GetAllAsync());
[HttpGet("{id:int}"),PermissionAuthorize("GoodsReceiptReturns.View")]public async Task<ActionResult<GoodsReceiptReturnDto>>Get(int id){var x=await service.GetAsync(id);return x is null?NotFound():Ok(x);}
[HttpGet("create-form"),PermissionAuthorize("GoodsReceiptReturns.Create")]public async Task<IActionResult>GetCreateForm(int goodsReceiptId){var x=await service.GetCreatePageAsync(goodsReceiptId);return string.IsNullOrWhiteSpace(x.ReceiptNumber)?NotFound():Ok(x);}
[HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("GoodsReceiptReturns.Create")]public async Task<IActionResult>Create(CreateGoodsReceiptReturnDto input){try{return Ok(new{id=await service.CreateAndPostAsync(input)});}catch(Exception e)when(e is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,localizer[e.Message]);return InternalApiProblemFactory.Validation(HttpContext,ModelState,localizer["One or more validation errors occurred."],InternalApiProblemCodes.Validation);}}
}
