using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Settings;
using MiniStore.Application.Services;

namespace MiniStore.Web.Controllers;

[Authorize(Roles = "Admin")]
public class SettingsController : Controller
{
    private readonly InvoiceSettingsService _invoiceSettingsService;

    public SettingsController(
        InvoiceSettingsService invoiceSettingsService)
    {
        _invoiceSettingsService = invoiceSettingsService;
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
}
