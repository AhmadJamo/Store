using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class SupplierCreditNotesController(SupplierCreditNoteService service, IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("SupplierCreditNotes.View")]
    public async Task<IActionResult> Index() => View(await service.GetAllAsync());

    [HttpGet, PermissionAuthorize("SupplierCreditNotes.View")]
    public async Task<IActionResult> Details(int id) => await service.GetAsync(id) is { } value ? View(value) : NotFound();

    [HttpGet, PermissionAuthorize("SupplierCreditNotes.Create")]
    public async Task<IActionResult> Create(int vendorBillId)
    {
        var page = await service.GetCreatePageAsync(vendorBillId);
        return string.IsNullOrEmpty(page.VendorBillNumber) ? NotFound() : View(page);
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SupplierCreditNotes.Create")]
    public async Task<IActionResult> Create(CreateSupplierCreditNoteDto dto)
    {
        try
        {
            var id = await service.CreateAndPostAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Supplier credit note posted."].Value;
            return RedirectToAction(nameof(Details), new { id, vendorBillId = dto.VendorBillId });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            var page = await service.GetCreatePageAsync(dto.VendorBillId, dto);
            return string.IsNullOrEmpty(page.VendorBillNumber) ? NotFound() : View(page);
        }
    }
}
