using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class GoodsReceiptReturnsController(GoodsReceiptReturnService service, IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("GoodsReceiptReturns.View")]
    public async Task<IActionResult> Index() => View(await service.GetAllAsync());

    [HttpGet, PermissionAuthorize("GoodsReceiptReturns.View")]
    public async Task<IActionResult> Details(int id) => await service.GetAsync(id) is { } value ? View(value) : NotFound();

    [HttpGet, PermissionAuthorize("GoodsReceiptReturns.Create")]
    public async Task<IActionResult> Create(int goodsReceiptId)
    {
        var page = await service.GetCreatePageAsync(goodsReceiptId);
        return string.IsNullOrEmpty(page.ReceiptNumber) ? NotFound() : View(page);
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("GoodsReceiptReturns.Create")]
    public async Task<IActionResult> Create(CreateGoodsReceiptReturnDto dto)
    {
        try
        {
            var id = await service.CreateAndPostAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Goods receipt return posted."].Value;
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            var page = await service.GetCreatePageAsync(dto.GoodsReceiptId, dto);
            return string.IsNullOrEmpty(page.ReceiptNumber) ? NotFound() : View(page);
        }
    }
}
