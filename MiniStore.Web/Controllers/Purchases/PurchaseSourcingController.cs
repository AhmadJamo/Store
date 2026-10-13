using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class PurchaseSourcingController(PurchaseSourcingService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet]
    [PermissionAuthorize("PurchaseSourcing.View")]
    public async Task<IActionResult> Index(PurchaseSourcingStatus? status, string? search)
    {
        ViewBag.Status = status;
        ViewBag.Search = search;
        return View(await service.GetAllAsync(status, search));
    }

    [HttpGet]
    [PermissionAuthorize("PurchaseSourcing.Create")]
    public async Task<IActionResult> Create(int purchaseRequestId)
    {
        var page = await service.GetCreatePageAsync(purchaseRequestId);
        return page.Request is null ? NotFound() : View(page);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("PurchaseSourcing.Create")]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] CreatePurchaseSourcingDto dto,
        List<IFormFile>? attachments)
    {
        try
        {
            dto.Attachments = await ReadAttachmentsAsync(attachments);
            var id = await service.CreateAndSendAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Purchase sourcing event sent."].Value;
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            var page = await service.GetCreatePageAsync(dto.PurchaseRequestId, dto);
            return page.Request is null ? NotFound() : View(page);
        }
    }

    [HttpGet]
    [PermissionAuthorize("PurchaseSourcing.CreateDirect")]
    public async Task<IActionResult> Direct() => View(await service.GetDirectCreatePageAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("PurchaseSourcing.CreateDirect")]
    public async Task<IActionResult> Direct([Bind(Prefix = "Form")] CreateDirectPurchaseSourcingDto dto,
        List<IFormFile>? attachments)
    {
        try
        {
            dto.Attachments = await ReadAttachmentsAsync(attachments);
            var id = await service.CreateAndSendDirectAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Direct RFQ sent within company policy."].Value;
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or UnauthorizedAccessException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return View(await service.GetDirectCreatePageAsync(dto));
        }
    }

    [HttpGet]
    [PermissionAuthorize("PurchaseSourcing.View")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await service.GetAsync(id);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpGet]
    [PermissionAuthorize("PurchaseSourcing.View")]
    public async Task<IActionResult> Print(int id)
    {
        var dto = await service.GetAsync(id);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpGet]
    [PermissionAuthorize("PurchaseSourcing.View")]
    public async Task<IActionResult> Attachment(int id, int attachmentId)
    {
        var attachment = await service.GetAttachmentAsync(id, attachmentId);
        return attachment is null ? NotFound() : File(attachment.Content, attachment.ContentType, attachment.FileName);
    }

    private static async Task<List<PurchaseSourcingAttachmentInputDto>> ReadAttachmentsAsync(
        IEnumerable<IFormFile>? attachments)
    {
        var files = attachments?.Where(x => x.Length > 0).ToList() ?? [];
        if (files.Count > 10) throw new ArgumentException("An RFQ can contain up to 10 attachments.");
        var result = new List<PurchaseSourcingAttachmentInputDto>();
        foreach (var file in files)
        {
            if (file.Length > 5 * 1024 * 1024)
                throw new ArgumentException("Each RFQ attachment must be between 1 byte and 5 MB.");
            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            result.Add(new PurchaseSourcingAttachmentInputDto
            {
                FileName = Path.GetFileName(file.FileName), ContentType = file.ContentType, Content = stream.ToArray()
            });
        }
        return result;
    }
}
