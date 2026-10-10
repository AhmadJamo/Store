using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class SupplierPaymentsController(SupplierPaymentService service, IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("SupplierPayments.View")]
    public async Task<IActionResult> Index(string? search) { ViewBag.Search = search; return View(await service.GetAllAsync(search)); }

    [HttpGet, PermissionAuthorize("SupplierPayments.View")]
    public async Task<IActionResult> Details(int id) { var payment = await service.GetAsync(id); return payment is null ? NotFound() : View(payment); }

    [HttpGet, PermissionAuthorize("SupplierPayments.Create")]
    public async Task<IActionResult> Create(int vendorBillId)
    {
        var page = await service.GetCreatePageAsync(vendorBillId);
        return string.IsNullOrWhiteSpace(page.SupplierName) ? NotFound() : View(page);
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SupplierPayments.Create")]
    public async Task<IActionResult> Create(CreateSupplierPaymentDto dto)
    {
        try
        {
            var id = await service.CreateAndPostAsync(dto);
            TempData["NotificationType"] = "success"; TempData["NotificationMessage"] = localizer["Supplier payment posted."].Value;
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            var page = await service.GetCreatePageAsync(dto.SourceVendorBillId, dto);
            return string.IsNullOrWhiteSpace(page.SupplierName) ? NotFound() : View(page);
        }
    }
}
