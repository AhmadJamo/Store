using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class PurchaseDocumentReversalsController(PurchaseDocumentReversalService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("PurchaseDocumentReversals.View")]
    public async Task<IActionResult> Index() => View(await service.GetAllAsync());

    [HttpGet, PermissionAuthorize("PurchaseDocumentReversals.View")]
    public async Task<IActionResult> Details(int id) => await service.GetAsync(id) is { } value ? View(value) : NotFound();

    [HttpGet, PermissionAuthorize("PurchaseDocumentReversals.Create")]
    public async Task<IActionResult> Create(PurchaseDocumentType documentType, int originalDocumentId)
    {
        try { return View(await service.GetCreatePageAsync(documentType, originalDocumentId)); }
        catch (InvalidOperationException) { return NotFound(); }
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("PurchaseDocumentReversals.Create")]
    public async Task<IActionResult> Create(CreatePurchaseDocumentReversalDto dto)
    {
        try
        {
            var id = await service.ReverseAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Purchase document reversed."].Value;
            return RedirectToAction(nameof(Details), new { id, originalController = Controller(dto.DocumentType), originalDocumentId = dto.OriginalDocumentId });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return View(await service.GetCreatePageAsync(dto.DocumentType, dto.OriginalDocumentId, dto));
        }
    }

    private static string Controller(PurchaseDocumentType type) => type switch
    {
        PurchaseDocumentType.GoodsReceipt => "GoodsReceipts", PurchaseDocumentType.VendorBill => "VendorBills",
        PurchaseDocumentType.SupplierPayment => "SupplierPayments", PurchaseDocumentType.LandedCost => "LandedCosts", _ => string.Empty
    };
}
