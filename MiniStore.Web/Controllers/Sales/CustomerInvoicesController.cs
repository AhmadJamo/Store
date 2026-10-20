using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class CustomerInvoicesController(CustomerInvoiceService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("CustomerInvoices.View")]
    public async Task<IActionResult> Index(CustomerInvoiceStatus? status, string? search)
    { ViewBag.Status = status; ViewBag.Search = search; return View(await service.GetAllAsync(status, search)); }

    [HttpGet, PermissionAuthorize("CustomerInvoices.View")]
    public async Task<IActionResult> Details(int id)
    { var dto = await service.GetAsync(id); return dto is null ? NotFound() : View(dto); }

    [HttpGet, PermissionAuthorize("CustomerInvoices.Create")]
    public async Task<IActionResult> Create(int salesOrderId)
    { var dto = await service.GetCreatePageAsync(salesOrderId); return dto is null ? NotFound() : View(dto); }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerInvoices.Create")]
    public async Task<IActionResult> Create(CreateCustomerInvoiceDto dto)
    {
        try
        {
            var id = await service.CreateDraftAsync(dto); Notify("success", "Customer invoice draft created.");
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        { Notify("danger", ex.Message); return RedirectToAction(nameof(Create), new { salesOrderId = dto.SalesOrderId }); }
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerInvoices.Approve")]
    public async Task<IActionResult> Approve(CustomerInvoiceStatusCommandDto dto)
    {
        try { await service.ApproveAsync(dto); Notify("success", "Customer invoice approved."); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { Notify("danger", ex.Message); }
        return RedirectToAction(nameof(Details), new { id = dto.Id });
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerInvoices.Cancel")]
    public async Task<IActionResult> Cancel(CustomerInvoiceStatusCommandDto dto)
    {
        try { await service.CancelAsync(dto); Notify("success", "Customer invoice cancelled."); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { Notify("danger", ex.Message); }
        return RedirectToAction(nameof(Details), new { id = dto.Id });
    }

    private void Notify(string type, string key)
    { TempData["NotificationType"] = type; TempData["NotificationMessage"] = localizer[key].Value; }
}
