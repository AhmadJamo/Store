using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class PurchaseServiceAcceptancesController(
    PurchaseServiceAcceptanceService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("PurchaseServiceAcceptances.View")]
    public async Task<IActionResult> Index() => View(await service.GetAllAsync());

    [HttpGet, PermissionAuthorize("PurchaseServiceAcceptances.View")]
    public async Task<IActionResult> Details(int id) =>
        await service.GetAsync(id) is { } value ? View(value) : NotFound();

    [HttpGet, PermissionAuthorize("PurchaseServiceAcceptances.Create")]
    public async Task<IActionResult> Create(int purchaseOrderId)
    {
        var page = await service.GetCreatePageAsync(purchaseOrderId);
        return string.IsNullOrWhiteSpace(page.PurchaseOrderNumber) ? NotFound() : View(page);
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("PurchaseServiceAcceptances.Create")]
    public async Task<IActionResult> Create(CreatePurchaseServiceAcceptanceDto dto)
    {
        try
        {
            await service.CompleteAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Purchase service completion recorded."].Value;
            return RedirectToAction("Details", "PurchaseOrders", new { id = dto.PurchaseOrderId });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            var page = await service.GetCreatePageAsync(dto.PurchaseOrderId, dto);
            return string.IsNullOrWhiteSpace(page.PurchaseOrderNumber) ? NotFound() : View(page);
        }
    }
}
