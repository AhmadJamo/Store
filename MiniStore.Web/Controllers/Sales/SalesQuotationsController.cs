using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class SalesQuotationsController(SalesQuotationService service, IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("SalesQuotations.View")]
    public async Task<IActionResult> Index(SalesQuotationStatus? status, string? search)
    { ViewBag.Status = status; ViewBag.Search = search; return View(await service.GetAllAsync(status, search)); }

    [HttpGet, PermissionAuthorize("SalesQuotations.Create")]
    public async Task<IActionResult> Create() => View(await service.GetCreatePageAsync());

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotations.Create")]
    public async Task<IActionResult> Create(CreateSalesQuotationDto dto)
    {
        if (!ModelState.IsValid) return View(await service.GetCreatePageAsync(dto));
        try { var id = await service.CreateAsync(dto); Notify("success", "Sales quotation draft created."); return RedirectToAction(nameof(Details), new { id }); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { ModelState.AddModelError(string.Empty, localizer[ex.Message]); return View(await service.GetCreatePageAsync(dto)); }
    }

    [HttpGet, PermissionAuthorize("SalesQuotations.View")]
    public async Task<IActionResult> Details(int id) { var dto = await service.GetAsync(id); return dto is null ? NotFound() : View(dto); }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotations.Send")]
    public Task<IActionResult> Send(SalesQuotationStatusCommandDto dto) => Change(dto, service.SendAsync, "Sales quotation sent.");
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotations.Accept")]
    public Task<IActionResult> Accept(SalesQuotationStatusCommandDto dto) => Change(dto, service.AcceptAsync, "Sales quotation accepted.");
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotations.Reject")]
    public Task<IActionResult> Reject(SalesQuotationStatusCommandDto dto) => Change(dto, service.RejectAsync, "Sales quotation rejected.");
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotations.Cancel")]
    public Task<IActionResult> Expire(SalesQuotationStatusCommandDto dto) => Change(dto, service.ExpireAsync, "Sales quotation expired.");
    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("SalesQuotations.Cancel")]
    public Task<IActionResult> Cancel(SalesQuotationStatusCommandDto dto) => Change(dto, service.CancelAsync, "Sales quotation cancelled.");

    private async Task<IActionResult> Change(SalesQuotationStatusCommandDto dto, Func<SalesQuotationStatusCommandDto, Task> command, string success)
    { try { await command(dto); Notify("success", success); } catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { Notify("danger", ex.Message); } return RedirectToAction(nameof(Details), new { id = dto.Id }); }
    private void Notify(string type, string key) { TempData["NotificationType"] = type; TempData["NotificationMessage"] = localizer[key].Value; }
}
