using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class SalesDeliveriesController(SalesDeliveryService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("SalesDeliveries.View")]
    public async Task<IActionResult> Index(string? search) { ViewBag.Search = search; return View(await service.GetAllAsync(search)); }

    [HttpGet, PermissionAuthorize("SalesDeliveries.View")]
    public async Task<IActionResult> Details(int id) { var dto = await service.GetAsync(id); return dto is null ? NotFound() : View(dto); }

    [HttpGet, PermissionAuthorize("SalesDeliveries.Create")]
    public async Task<IActionResult> Create(int salesOrderId)
    { var page = await service.GetCreatePageAsync(salesOrderId); return page is null ? NotFound() : View(page); }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesDeliveries.Create")]
    public async Task<IActionResult> Create(CreateSalesDeliveryDto dto)
    {
        try
        {
            var id = await service.CreateDraftAsync(dto); Notify("success", "Sales delivery draft prepared.");
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        { Notify("danger", ex.Message); return RedirectToAction(nameof(Create), new { salesOrderId = dto.SalesOrderId }); }
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesDeliveries.Post")]
    public async Task<IActionResult> Post(SalesDeliveryStatusCommandDto dto)
    {
        try { await service.PostAsync(dto); Notify("success", "Sales delivery posted."); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { Notify("danger", ex.Message); }
        return RedirectToAction(nameof(Details), new { id = dto.Id });
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesDeliveries.Cancel")]
    public async Task<IActionResult> Cancel(SalesDeliveryStatusCommandDto dto)
    {
        try { await service.CancelAsync(dto); Notify("success", "Sales delivery draft cancelled."); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { Notify("danger", ex.Message); }
        return RedirectToAction(nameof(Details), new { id = dto.Id });
    }
    private void Notify(string type, string key) { TempData["NotificationType"] = type; TempData["NotificationMessage"] = localizer[key].Value; }
}
