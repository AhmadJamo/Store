using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class CustomerPaymentsController(CustomerPaymentService service, IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("CustomerPayments.View")]
    public async Task<IActionResult> Index(string? search) { ViewBag.Search = search; return View(await service.GetAllAsync(search)); }

    [HttpGet, PermissionAuthorize("CustomerPayments.View")]
    public async Task<IActionResult> Details(int id) { var value = await service.GetAsync(id); return value is null ? NotFound() : View(value); }

    [HttpGet, PermissionAuthorize("CustomerPayments.Create")]
    public async Task<IActionResult> Create(int customerInvoiceId)
    {
        var page = await service.GetCreatePageAsync(customerInvoiceId);
        return string.IsNullOrWhiteSpace(page.CustomerName) ? NotFound() : View(page);
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerPayments.Create")]
    public async Task<IActionResult> Create(CreateCustomerPaymentDto dto)
    {
        try
        {
            var id = await service.CreateAndPostAsync(dto); Notify("success", "Customer payment posted.");
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[ex.Message]);
            var page = await service.GetCreatePageAsync(dto.SourceCustomerInvoiceId, dto);
            return string.IsNullOrWhiteSpace(page.CustomerName) ? NotFound() : View(page);
        }
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("CustomerPayments.Reverse")]
    public async Task<IActionResult> Reverse(ReverseCustomerPaymentDto dto)
    {
        try { await service.ReverseAsync(dto); Notify("success", "Customer payment reversed."); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { Notify("danger", ex.Message); }
        return RedirectToAction(nameof(Details), new { id = dto.Id });
    }

    private void Notify(string type, string key)
    { TempData["NotificationType"] = type; TempData["NotificationMessage"] = localizer[key].Value; }
}
