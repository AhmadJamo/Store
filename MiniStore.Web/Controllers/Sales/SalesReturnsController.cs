using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class SalesReturnsController(
    SalesReturnService salesReturnService,
    ISaleService saleService,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [PermissionAuthorize("SalesReturns.View")]
    public async Task<IActionResult> Index() => View(await salesReturnService.GetAllAsync());

    [PermissionAuthorize("SalesReturns.View")]
    public async Task<IActionResult> Details(int id)
    {
        var value = await salesReturnService.GetByIdAsync(id);
        return value is null ? NotFound() : View(value);
    }

    [PermissionAuthorize("SalesReturns.Create")]
    public async Task<IActionResult> Create(int saleId)
    {
        var sale = await saleService.GetByIdAsync(saleId);
        if (sale is null) return NotFound();
        ViewBag.Sale = sale;
        ViewBag.ReturnedQuantities = await salesReturnService.GetReturnedQuantitiesAsync(saleId);
        return View(new CreateSalesReturnDto { SaleId = saleId, Date = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("SalesReturns.Create")]
    public async Task<IActionResult> Create(CreateSalesReturnDto dto)
    {
        try
        {
            var id = await salesReturnService.CreateAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Sales return created and posted."].Value;
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[ex.Message]);
            var sale = await saleService.GetByIdAsync(dto.SaleId);
            if (sale is null) return NotFound();
            ViewBag.Sale = sale;
            ViewBag.ReturnedQuantities = await salesReturnService.GetReturnedQuantitiesAsync(dto.SaleId);
            return View(dto);
        }
    }
}
