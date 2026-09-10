using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Dtos.Settings;
using MiniStore.Application.DTOs.Settings;
using MiniStore.Application.Services;

namespace MiniStore.Web.Controllers;

[Authorize(Roles = "Admin")]
public class SettingsController : Controller
{
    private readonly InvoiceSettingsService _invoiceSettingsService;
    private readonly GeneralSettingsService _generalSettingsService;
    private readonly DiscountSettingsService _discountSettingsService;

    public SettingsController(
        InvoiceSettingsService invoiceSettingsService,
        GeneralSettingsService generalSettingsService,
        DiscountSettingsService discountSettingsService)
    {
        _invoiceSettingsService = invoiceSettingsService;
        _generalSettingsService = generalSettingsService;
        _discountSettingsService = discountSettingsService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Invoices()
    {
        return View(await _invoiceSettingsService.GetAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Invoices(
        InvoiceSettingsDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            await _invoiceSettingsService.UpdateAsync(dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Invoice settings updated successfully.";

            return RedirectToAction(nameof(Invoices));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> General()
    {
        return View(await _generalSettingsService.GetAsync());
    }

    [HttpPost]
    public async Task<IActionResult> General(
        GeneralSettingsDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            await _generalSettingsService.UpdateAsync(dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "General settings updated successfully.";

            return RedirectToAction(nameof(General));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }


    [HttpGet]
    public async Task<IActionResult> Discounts()
    {
        var settings = await _discountSettingsService.GetAsync();

        return View(settings);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Discounts(DiscountSettingsDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            await _discountSettingsService.UpdateAsync(dto);

            TempData["SuccessMessage"] =
                "Discount settings saved successfully.";

            return RedirectToAction(nameof(Discounts));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(dto);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(dto);
        }
    }
}