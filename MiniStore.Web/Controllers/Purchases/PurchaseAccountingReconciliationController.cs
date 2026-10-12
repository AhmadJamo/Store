using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class PurchaseAccountingReconciliationController(
    PurchaseAccountingReconciliationService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet]
    [PermissionAuthorize("PurchaseAccountingReconciliation.View")]
    public async Task<IActionResult> Index(
        DateOnly? asOfDate,
        bool exceptionsOnly = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return View(await service.GetAsync(new PurchaseAccountingReconciliationQueryDto
            {
                AsOfDate = asOfDate,
                ExceptionsOnly = exceptionsOnly
            }, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message].Value);
            return View(await service.GetAsync(new PurchaseAccountingReconciliationQueryDto
            {
                AsOfDate = DateOnly.FromDateTime(DateTime.Today),
                ExceptionsOnly = exceptionsOnly
            }, cancellationToken));
        }
    }
}
