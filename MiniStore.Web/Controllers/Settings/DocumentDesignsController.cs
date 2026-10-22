using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Settings;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

[PermissionAuthorize("Administration.Access")]
public sealed class DocumentDesignsController(DocumentDesignService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(DocumentDesignStatus? status, DocumentDesignType? documentType, string? search)
    {
        ViewBag.Status = status; ViewBag.DocumentType = documentType; ViewBag.Search = search;
        return View(await service.GetAllAsync(status, documentType, search));
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? sourceDesignId, DocumentDesignType? documentType)
    {
        try { return View(await service.GetCreateAsync(sourceDesignId, documentType)); }
        catch (InvalidOperationException ex) { Notify("danger", ex.Message); return RedirectToAction(nameof(Index)); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDocumentDesignDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        try
        {
            var id = await service.CreateAsync(dto); Notify("success", "Document design draft created.");
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        { ModelState.AddModelError(string.Empty, localizer[ex.Message]); return View(dto); }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    { var dto = await service.GetAsync(id); return dto is null ? NotFound() : View(dto); }

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> Activate(DocumentDesignCommandDto dto) => Change(dto, service.ActivateAsync, "Document design activated.");
    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> Archive(DocumentDesignCommandDto dto) => Change(dto, service.ArchiveAsync, "Document design archived.");
    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> SetDefault(DocumentDesignCommandDto dto) => Change(dto, service.SetDefaultAsync, "Default document design updated.");

    private async Task<IActionResult> Change(DocumentDesignCommandDto dto,
        Func<DocumentDesignCommandDto, Task> command, string success)
    {
        try { await command(dto); Notify("success", success); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { Notify("danger", ex.Message); }
        return RedirectToAction(nameof(Details), new { id = dto.Id });
    }

    private void Notify(string type, string key)
    { TempData["NotificationType"] = type; TempData["NotificationMessage"] = localizer[key].Value; }
}
