using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class VendorBillsController(VendorBillService service, PurchaseDocumentReversalService reversalService,
    IStringLocalizer<SharedResource> localizer) : Controller
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
        ViewBag.Reversal = await reversalService.GetByOriginalAsync(MiniStore.Domain.Entities.PurchaseDocumentType.VendorBill, id);
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
    [RequestFormLimits(MultipartBodyLengthLimit = 52_428_800)]
    public async Task<IActionResult> Create(CreateVendorBillDto dto, List<IFormFile>? attachments)
    {
        try
        {
            if ((attachments?.Count ?? 0) > 10) throw new ArgumentException("A vendor bill can contain at most ten attachments.");
            var inputs = new List<VendorBillAttachmentInputDto>();
            foreach (var file in attachments ?? [])
            {
                if (file.Length is <= 0 or > 5 * 1024 * 1024) throw new ArgumentException("Each vendor bill attachment must be between 1 byte and 5 MB.");
                await using var stream = new MemoryStream(); await file.CopyToAsync(stream);
                inputs.Add(new VendorBillAttachmentInputDto { FileName = file.FileName, ContentType = file.ContentType, Content = stream.ToArray() });
            }
            var id = await service.CreateDraftAsync(dto, inputs);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Vendor bill draft saved."].Value;
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or FormatException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            var page = await service.GetCreatePageAsync(dto.PurchaseOrderId, dto);
            return string.IsNullOrWhiteSpace(page.PurchaseOrderNumber) ? NotFound() : View(page);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("VendorBills.Approve")]
    public async Task<IActionResult> Approve(int id, string rowVersion)
    {
        try
        {
            await service.ApproveAsync(id, Convert.FromBase64String(rowVersion));
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Vendor bill approved."].Value;
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or FormatException)
        {
            TempData["NotificationType"] = "danger"; TempData["NotificationMessage"] = localizer[exception.Message].Value;
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("VendorBills.OverrideMatch")]
    public async Task<IActionResult> ApproveOverride(int id, string rowVersion, string reason)
    {
        try { await service.ApproveAsync(id, Convert.FromBase64String(rowVersion), true, reason); TempData["NotificationType"] = "success"; TempData["NotificationMessage"] = localizer["Vendor bill approved with a documented match override."].Value; }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or FormatException) { TempData["NotificationType"] = "danger"; TempData["NotificationMessage"] = localizer[exception.Message].Value; }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("VendorBills.Post")]
    public async Task<IActionResult> Post(int id, string rowVersion)
    {
        try { await service.PostAsync(id, Convert.FromBase64String(rowVersion)); TempData["NotificationType"] = "success"; TempData["NotificationMessage"] = localizer["Vendor bill posted."].Value; }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or FormatException) { TempData["NotificationType"] = "danger"; TempData["NotificationMessage"] = localizer[exception.Message].Value; }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("VendorBills.Cancel")]
    public async Task<IActionResult> Cancel(int id, string rowVersion, string reason)
    {
        try { await service.CancelAsync(id, Convert.FromBase64String(rowVersion), reason); TempData["NotificationType"] = "success"; TempData["NotificationMessage"] = localizer["Vendor bill cancelled."].Value; }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or FormatException) { TempData["NotificationType"] = "danger"; TempData["NotificationMessage"] = localizer[exception.Message].Value; }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet, PermissionAuthorize("VendorBills.View")]
    public async Task<IActionResult> DownloadAttachment(int id, int attachmentId)
    {
        var value = await service.GetAttachmentAsync(id, attachmentId);
        return value is null ? NotFound() : File(value.Content, value.ContentType, value.FileName);
    }
}
