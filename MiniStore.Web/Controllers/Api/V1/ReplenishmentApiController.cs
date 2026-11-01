using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Inventory.Replenishment;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/replenishment")]
public sealed class ReplenishmentApiController(
    ReplenishmentService replenishment,
    ReplenishmentPurchaseDemandService purchaseDemand,
    ICurrentUserService currentUser,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("Inventory.Replenishment.View")]
    public async Task<ActionResult<ReplenishmentPageDto>> Get(
        int? warehouseId,
        string? search,
        bool exceptionsOnly = false) =>
        Ok(await replenishment.GetPageAsync(warehouseId, search, exceptionsOnly));

    [HttpPost("rules"), ValidateAntiForgeryToken, PermissionAuthorize("Inventory.Replenishment.Manage")]
    public Task<IActionResult> SaveRule(SaveReplenishmentRuleDto input) =>
        Command(() => replenishment.SaveAsync(input));

    [HttpPost("rules/{id:int}/active"), ValidateAntiForgeryToken, PermissionAuthorize("Inventory.Replenishment.Manage")]
    public Task<IActionResult> SetActive(int id, SetReplenishmentRuleActiveDto input) =>
        Command(() => replenishment.SetActiveAsync(id, input.Active));

    [HttpPost("rules/{id:int}/transfer-draft"), ValidateAntiForgeryToken]
    [PermissionAuthorize("Inventory.Replenishment.Manage")]
    [PermissionAuthorize("StockTransfers.Create")]
    public Task<IActionResult> CreateTransferDraft(int id) => Execute(async () =>
        new ReplenishmentDocumentResultDto(await replenishment.CreateTransferDraftAsync(id, currentUser.UserId)));

    [HttpPost("rules/{id:int}/purchase-request-draft"), ValidateAntiForgeryToken]
    [PermissionAuthorize("Inventory.Replenishment.Manage")]
    [PermissionAuthorize("PurchaseRequests.Create")]
    public Task<IActionResult> CreatePurchaseRequestDraft(int id) => Execute(async () =>
        new ReplenishmentDocumentResultDto(await purchaseDemand.CreateDraftAsync(id)));

    private async Task<IActionResult> Command(Func<Task> action)
    {
        try
        {
            await action();
            return NoContent();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return Validation(exception);
        }
    }

    private async Task<IActionResult> Execute<T>(Func<Task<T>> action)
    {
        try
        {
            return Ok(await action());
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return Validation(exception);
        }
    }

    private IActionResult Validation(Exception exception)
    {
        ModelState.AddModelError(string.Empty, localizer[exception.Message]);
        return InternalApiProblemFactory.Validation(
            HttpContext,
            ModelState,
            localizer["One or more validation errors occurred."],
            InternalApiProblemCodes.Validation);
    }
}
