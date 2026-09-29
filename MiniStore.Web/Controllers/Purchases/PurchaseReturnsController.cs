using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class PurchaseReturnsController(
    PurchaseReturnService returnService,
    PurchaseService purchaseService,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [PermissionAuthorize("PurchaseReturns.View")]
    public async Task<IActionResult> Index() => View(await returnService.GetAllAsync());

    [PermissionAuthorize("PurchaseReturns.View")]
    public async Task<IActionResult> Details(int id)
    {
        var value = await returnService.GetByIdAsync(id);
        return value is null ? NotFound() : View(value);
    }

    [PermissionAuthorize("PurchaseReturns.Create")]
    public async Task<IActionResult> Create(int purchaseId)
    {
        var purchase = await purchaseService.GetByIdAsync(purchaseId);
        if (purchase is null) return NotFound();
        ViewBag.Purchase = purchase;
        ViewBag.ReturnedQuantities = await returnService.GetReturnedQuantitiesAsync(purchaseId);
        return View(new CreatePurchaseReturnDto { PurchaseId = purchaseId, Date = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [PermissionAuthorize("PurchaseReturns.Create")]
    public async Task<IActionResult> Create(CreatePurchaseReturnDto dto)
    {
        try
        {
            var id = await returnService.CreateAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Purchase return created and posted."].Value;
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[ex.Message]);
            var purchase = await purchaseService.GetByIdAsync(dto.PurchaseId);
            if (purchase is null) return NotFound();
            ViewBag.Purchase = purchase;
            ViewBag.ReturnedQuantities = await returnService.GetReturnedQuantitiesAsync(dto.PurchaseId);
            return View(dto);
        }
    }
}
