using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController, Authorize, Route("api/v1/sales-price-lists")]
public sealed class SalesPriceListsApiController(
    SalesPriceListService service,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("SalesPricing.View")]
    public async Task<ActionResult<SalesPriceListPageDto>> GetPage(int? id = null) =>
        Ok(await service.GetPageAsync(id));

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesPricing.Manage")]
    public Task<IActionResult> Create(SaveSalesPriceListDto input) =>
        Run(async () => Ok(new { id = await service.CreateListAsync(input) }));

    [HttpPost("status"), ValidateAntiForgeryToken, PermissionAuthorize("SalesPricing.Manage")]
    public Task<IActionResult> SetListActive(SalesPricingStatusDto input) =>
        Run(async () => { await service.SetListActiveAsync(input); return NoContent(); });

    [HttpPost("rules"), ValidateAntiForgeryToken, PermissionAuthorize("SalesPricing.Manage")]
    public Task<IActionResult> AddRule(SaveSalesPriceRuleDto input) =>
        Run(async () => { await service.AddRuleAsync(input); return NoContent(); });

    [HttpPost("rules/status"), ValidateAntiForgeryToken, PermissionAuthorize("SalesPricing.Manage")]
    public Task<IActionResult> SetRuleActive(SalesPricingStatusDto input) =>
        Run(async () => { await service.SetRuleActiveAsync(input); return NoContent(); });

    [HttpPost("evaluate"), ValidateAntiForgeryToken, PermissionAuthorize("SalesPricing.View")]
    public Task<IActionResult> Evaluate(EvaluateSalesPriceDto input) =>
        Run(async () => Ok(await service.EvaluateAsync(input)));

    private async Task<IActionResult> Run(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(
                HttpContext,
                ModelState,
                localizer["One or more validation errors occurred."],
                InternalApiProblemCodes.Validation);
        }
    }
}
