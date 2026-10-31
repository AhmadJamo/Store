using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/purchase-analytics")]
public sealed class PurchaseAnalyticsApiController(
    PurchaseAnalyticsService service,
    IStringLocalizer<SharedResource> localizer,
    UserManager<IdentityUser> users) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("PurchaseAnalytics.View")]
    public async Task<IActionResult> Get([FromQuery] PurchaseAnalyticsQueryDto query)
    {
        try
        {
            return Ok(await PrepareForPresentationAsync(await service.GetAsync(query)));
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(
                HttpContext,
                ModelState,
                localizer["One or more validation errors occurred."],
                InternalApiProblemCodes.Validation);
        }
    }

    private async Task<PurchaseAnalyticsDto> PrepareForPresentationAsync(PurchaseAnalyticsDto model)
    {
        var names = new Dictionary<string, string>();
        foreach (var id in model.BuyerOptions.Select(x => x.UserId).Distinct())
        {
            var user = await users.FindByIdAsync(id);
            names[id] = user?.Email ?? user?.UserName ?? localizer["Unknown User"].Value;
        }

        model.BuyerOptions = model.BuyerOptions
            .Select(x => new PurchaseAnalyticsBuyerOptionDto(x.UserId, names.GetValueOrDefault(x.UserId, x.DisplayName)))
            .ToList();

        foreach (var row in model.BuyerWorkloads)
            row.BuyerDisplayName = names.GetValueOrDefault(row.BuyerUserId, row.BuyerUserId);

        for (var index = 0; index < model.Trends.Count; index++)
        {
            var row = model.Trends[index];
            if (!row.GroupKey.StartsWith("b:", StringComparison.Ordinal)) continue;
            var buyerId = row.GroupKey[2..];
            model.Trends[index] = new PurchaseAnalyticsTrendDto
            {
                GroupKey = row.GroupKey,
                GroupLabel = names.GetValueOrDefault(buyerId, row.GroupLabel),
                OrderCount = row.OrderCount,
                OrderedValue = row.OrderedValue,
                BillCount = row.BillCount,
                Spend = row.Spend,
                PriceVariance = row.PriceVariance,
                AverageUnitPrice = row.AverageUnitPrice
            };
        }

        for (var index = 0; index < model.Kpis.Count; index++)
        {
            var row = model.Kpis[index];
            model.Kpis[index] = new PurchaseAnalyticsKpiDto(
                localizer[row.Label],
                row.Value,
                localizer[row.Source],
                row.Period,
                localizer[row.Explanation]);
        }

        for (var index = 0; index < model.AcceptanceJourney.Count; index++)
        {
            var row = model.AcceptanceJourney[index];
            model.AcceptanceJourney[index] = new PurchaseAcceptanceStageDto(
                localizer[row.Label],
                row.Count,
                row.Controller,
                row.Action,
                row.RequiredPermission,
                localizer[row.Explanation]);
        }

        return model;
    }
}
