using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.DTOs.Purchases;using MiniStore.Application.Services;using MiniStore.Web.Authorization;using MiniStore.Web.Middleware.Api;
namespace MiniStore.Web.Controllers.Api.V1;
[ApiController,Authorize,Route("api/v1/purchase-approval-rules")]public sealed class PurchaseApprovalRulesApiController(PurchaseApprovalService approvals,IStringLocalizer<SharedResource> localizer):ControllerBase
{
 [HttpGet,PermissionAuthorize("PurchaseApprovalRules.View")]public async Task<ActionResult<PurchaseApprovalRulesPageDto>> GetPage()=>Ok(await approvals.GetRulesPageAsync());
 [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("PurchaseApprovalRules.Manage")]public async Task<IActionResult>Create(CreatePurchaseApprovalRuleDto input){try{await approvals.CreateRuleAsync(input);return NoContent();}catch(Exception e)when(e is ArgumentException or InvalidOperationException){return Validation(e);}}
 [HttpPut("{id:int}/active"),ValidateAntiForgeryToken,PermissionAuthorize("PurchaseApprovalRules.Manage")]public async Task<IActionResult>SetActive(int id,SetPurchaseApprovalRuleActiveDto input){if(id!=input.Id)return Validation(new ArgumentException("Route and payload identifiers do not match."));try{await approvals.SetActiveAsync(id,input.Active,input.RowVersion);return NoContent();}catch(Exception e)when(e is ArgumentException or InvalidOperationException){return Validation(e);}}
 private IActionResult Validation(Exception e){ModelState.AddModelError(string.Empty,localizer[e.Message]);return InternalApiProblemFactory.Validation(HttpContext,ModelState,localizer["One or more validation errors occurred."],InternalApiProblemCodes.Validation);}
}
