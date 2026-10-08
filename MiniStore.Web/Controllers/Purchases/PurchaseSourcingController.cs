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
    public async Task<IActionResult> Create(CreatePurchaseSourcingDto dto)
    {
        try
        {
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
    [PermissionAuthorize("PurchaseSourcing.View")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await service.GetAsync(id);
        return dto is null ? NotFound() : View(dto);
    }
}
