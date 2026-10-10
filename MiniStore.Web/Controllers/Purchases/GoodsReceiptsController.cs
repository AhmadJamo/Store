using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class GoodsReceiptsController(GoodsReceiptService service, IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet]
    [PermissionAuthorize("GoodsReceipts.View")]
    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.Search = search;
        return View(await service.GetAllAsync(search));
    }

    [HttpGet]
    [PermissionAuthorize("GoodsReceipts.View")]
    public async Task<IActionResult> Details(int id)
    {
        var receipt = await service.GetAsync(id);
        return receipt is null ? NotFound() : View(receipt);
    }
    [HttpGet]
    [PermissionAuthorize("GoodsReceipts.Create")]
    public async Task<IActionResult> Create(int purchaseOrderId)
    {
        var page = await service.GetCreatePageAsync(purchaseOrderId);
        return page.PurchaseOrder is null ? NotFound() : View(page);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("GoodsReceipts.Create")]
    public async Task<IActionResult> Create(CreateGoodsReceiptDto dto)
    {
        try
        {
            var id = await service.CreateAndPostAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Goods receipt posted."].Value;
            return RedirectToAction("Details", "PurchaseOrders", new { id = dto.PurchaseOrderId });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            var page = await service.GetCreatePageAsync(dto.PurchaseOrderId, dto);
            return page.PurchaseOrder is null ? NotFound() : View(page);
        }
    }
}
