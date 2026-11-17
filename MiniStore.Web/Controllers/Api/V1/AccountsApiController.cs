using Microsoft.AspNetCore.Authorization;
using MiniStore.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.Dtos.Accounting;using MiniStore.Application.Services;using MiniStore.Web.Authorization;using MiniStore.Web.Middleware.Api;
namespace MiniStore.Web.Controllers.Api.V1;
[ApiController,Authorize,Route("api/v1/accounts"),PermissionAuthorize("Administration.Access")]
public sealed class AccountsApiController(AccountService service,IStringLocalizer<SharedResource> localizer):ControllerBase{
[HttpGet]public async Task<ActionResult<AccountPageDto>>Get()=>Ok(await service.GetPageAsync());
[HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Create(CreateAccountDto input){try{await service.CreateAsync(input);return NoContent();}catch(InvalidOperationException e)when(e is ResourceNotFoundException or BusinessConflictException or DuplicateResourceException or OptimisticConcurrencyException){return InternalApiProblemFactory.ApplicationProblem(HttpContext,e,localizer[e.Message]);}catch(Exception e)when(e is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,localizer[e.Message]);return InternalApiProblemFactory.Validation(HttpContext,ModelState,localizer["One or more validation errors occurred."],InternalApiProblemCodes.Validation);}}
}
