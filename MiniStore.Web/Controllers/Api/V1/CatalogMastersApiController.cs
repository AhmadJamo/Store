using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Settings;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[PermissionAuthorize("Administration.Access")]
[Route("api/v1/catalog-masters")]
public sealed class CatalogMastersApiController(TaxRateService taxes, PaymentMethodService payments, BranchService branches,
    AccountService accounts, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<CatalogMastersPageDto>> GetPage() => Ok(await Page());
    [HttpPost("tax-rates"), ValidateAntiForgeryToken] public async Task<ActionResult<CatalogMastersPageDto>> CreateTaxRate(CreateTaxRateDto form) => await Execute(async()=>await taxes.CreateAsync(form.Name,form.Rate,form.OutputAccountId,form.InputAccountId,form.IsPriceInclusive));
    [HttpPost("payment-methods"), ValidateAntiForgeryToken] public async Task<ActionResult<CatalogMastersPageDto>> CreatePaymentMethod(CreatePaymentMethodDto form) => await Execute(async()=>await payments.CreateAsync(form.Name,form.AccountId));
    [HttpPost("branches"), ValidateAntiForgeryToken] public async Task<ActionResult<CatalogMastersPageDto>> CreateBranch(CreateBranchDto form) => await Execute(async()=>await branches.CreateAsync(form.Code.ToUpperInvariant(),form.Name));
    private async Task<ActionResult<CatalogMastersPageDto>> Execute(Func<Task> operation) { try { await operation(); return Ok(await Page()); } catch(Exception exception) when(exception is ArgumentException or InvalidOperationException) { ModelState.AddModelError(string.Empty,localizer[exception.Message]); return InternalApiProblemFactory.Validation(HttpContext,ModelState,localizer["One or more validation errors occurred."],InternalApiProblemCodes.Validation); } }
    private async Task<CatalogMastersPageDto> Page()=>new(){TaxRates=await taxes.GetRowsAsync(),PaymentMethods=await payments.GetRowsAsync(),Branches=await branches.GetRowsAsync(),Accounts=(await accounts.GetAllAsync()).ToList()};
}
