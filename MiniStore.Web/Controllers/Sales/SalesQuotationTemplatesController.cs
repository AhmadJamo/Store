using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class SalesQuotationTemplatesController(SalesQuotationTemplateService service,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("SalesQuotationTemplates.View")]
    public async Task<IActionResult> Index(SalesQuotationTemplateStatus? status, string? search)
    { ViewBag.Status = status; ViewBag.Search = search; return View(await service.GetAllAsync(status, search)); }

    [HttpGet, PermissionAuthorize("SalesQuotationTemplates.Manage")]
    public async Task<IActionResult> Create(int? sourceTemplateId)
    { try { return View(await service.GetCreatePageAsync(null, sourceTemplateId)); } catch (InvalidOperationException ex) { Notify("danger", ex.Message); return RedirectToAction(nameof(Index)); } }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotationTemplates.Manage")]
    public async Task<IActionResult> Create(CreateSalesQuotationTemplateDto dto)
    {
        if (!ModelState.IsValid) return View(await service.GetCreatePageAsync(dto));
        try { var id = await service.CreateAsync(dto); Notify("success", "Quotation template draft created."); return RedirectToAction(nameof(Details), new { id }); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        { ModelState.AddModelError(string.Empty, localizer[ex.Message]); return View(await service.GetCreatePageAsync(dto)); }
    }

    [HttpGet, PermissionAuthorize("SalesQuotationTemplates.View")]
    public async Task<IActionResult> Details(int id) { var dto = await service.GetAsync(id); return dto is null ? NotFound() : View(dto); }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotationTemplates.Manage")]
    public Task<IActionResult> Activate(SalesQuotationTemplateCommandDto dto) => Change(dto, service.ActivateAsync, "Quotation template activated.");
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotationTemplates.Manage")]
    public Task<IActionResult> Archive(SalesQuotationTemplateCommandDto dto) => Change(dto, service.ArchiveAsync, "Quotation template archived.");
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotationTemplates.Manage")]
    public Task<IActionResult> SetDefault(SalesQuotationTemplateCommandDto dto) => Change(dto, service.SetDefaultAsync, "Default quotation template updated.");

    private async Task<IActionResult> Change(SalesQuotationTemplateCommandDto dto,
        Func<SalesQuotationTemplateCommandDto, Task> command, string success)
    { try { await command(dto); Notify("success", success); } catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { Notify("danger", ex.Message); } return RedirectToAction(nameof(Details), new { id = dto.Id }); }
    private void Notify(string type, string key) { TempData["NotificationType"] = type; TempData["NotificationMessage"] = localizer[key].Value; }
}
