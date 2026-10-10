using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class VendorBillsController(VendorBillService service, IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet]
    [PermissionAuthorize("VendorBills.View")]
    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.Search = search;
        return View(await service.GetAllAsync(search));
    }

    [HttpGet]
    [PermissionAuthorize("VendorBills.View")]
    public async Task<IActionResult> Details(int id)
    {
        var bill = await service.GetAsync(id);
        return bill is null ? NotFound() : View(bill);
    }

    [HttpGet]
    [PermissionAuthorize("VendorBills.Create")]
    public async Task<IActionResult> Create(int purchaseOrderId)
    {
        var page = await service.GetCreatePageAsync(purchaseOrderId);
        return string.IsNullOrWhiteSpace(page.PurchaseOrderNumber) ? NotFound() : View(page);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("VendorBills.Create")]
    public async Task<IActionResult> Create(CreateVendorBillDto dto)
    {
        try
        {
            var id = await service.CreateAndPostAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Vendor bill posted."].Value;
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            var page = await service.GetCreatePageAsync(dto.PurchaseOrderId, dto);
            return string.IsNullOrWhiteSpace(page.PurchaseOrderNumber) ? NotFound() : View(page);
        }
    }
}
