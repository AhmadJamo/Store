using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using System.Text;

namespace MiniStore.Web.Controllers;

public sealed class SupplierPurchasingController(
    SupplierPurchasingInfoService service,
    IStringLocalizer<SharedResource> localizer,
    ILogger<SupplierPurchasingController> logger) : Controller
{
    [HttpGet]
    [PermissionAuthorize("Purchases.SupplierTerms.View")]
    public async Task<IActionResult> Index(int? supplierId = null)
    {
        try { return View(await service.GetPageAsync(supplierId)); }
        catch (InvalidOperationException exception)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Purchases.SupplierTerms.Manage")]
    public async Task<IActionResult> SaveBulk([Bind(Prefix = "BulkForm")] BulkSaveSupplierPurchasingInfoDto dto)
    {
        try
        {
            var count = await service.SaveBulkAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Saved {0} supplier purchasing rows.", count].Value;
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to save supplier purchasing rows in bulk");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer["The operation could not be completed. Please try again or contact the administrator."].Value;
        }
        return RedirectToAction(nameof(Index), new { supplierId = dto.SupplierId });
    }

    [HttpGet]
    [PermissionAuthorize("Purchases.SupplierTerms.View")]
    public async Task<IActionResult> ExportCsv(int supplierId)
    {
        try
        {
            var csv = await service.ExportCsvAsync(supplierId);
            return File(new UTF8Encoding(true).GetBytes(csv), "text/csv; charset=utf-8", $"supplier-{supplierId}-purchasing.csv");
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
            return RedirectToAction(nameof(Index), new { supplierId });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Purchases.SupplierTerms.Manage")]
    [RequestFormLimits(MultipartBodyLengthLimit = 1_100_000)]
    public async Task<IActionResult> PreviewCsv(int supplierId, IFormFile? csvFile)
    {
        try
        {
            if (csvFile is null || csvFile.Length == 0) throw new ArgumentException("Select a supplier purchasing CSV file.");
            if (csvFile.Length > 1_048_576) throw new ArgumentException("The supplier purchasing CSV file cannot exceed 1 MB.");
            if (!string.Equals(Path.GetExtension(csvFile.FileName), ".csv", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Supplier purchasing import accepts CSV files only.");
            using var reader = new StreamReader(csvFile.OpenReadStream(), Encoding.UTF8, true);
            var content = await reader.ReadToEndAsync();
            return View("ImportPreview", await service.PreviewCsvAsync(supplierId, content, Path.GetFileName(csvFile.FileName)));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
            return RedirectToAction(nameof(Index), new { supplierId });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Purchases.SupplierTerms.Manage")]
    [RequestFormLimits(ValueLengthLimit = 1_100_000, MultipartBodyLengthLimit = 1_200_000)]
    public async Task<IActionResult> ImportCsv(ImportSupplierPurchasingCsvDto dto)
    {
        try
        {
            var count = await service.ImportCsvAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Imported {0} supplier purchasing rows.", count].Value;
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
        }
        return RedirectToAction(nameof(Index), new { supplierId = dto.SupplierId });
    }

    [HttpGet]
    [PermissionAuthorize("Purchases.SupplierTerms.View")]
    public async Task<IActionResult> PriceBreaks(int id)
    {
        try { return View(await service.GetPriceBreaksPageAsync(id)); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [PermissionAuthorize("Purchases.SupplierTerms.View")]
    public async Task<IActionResult> PriceHistory(int id)
    {
        try { return View(await service.GetPriceHistoryPageAsync(id)); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Purchases.SupplierTerms.Manage")]
    public async Task<IActionResult> SavePriceBreaks([Bind(Prefix = "Form")] SaveSupplierPriceBreaksDto dto)
    {
        try
        {
            var supplierId = await service.SavePriceBreaksAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Supplier price breaks saved."].Value;
            return RedirectToAction(nameof(Index), new { supplierId });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to save supplier price breaks");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer["The operation could not be completed. Please try again or contact the administrator."].Value;
        }
        return RedirectToAction(nameof(PriceBreaks), new { id = dto.PurchasingInfoId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Purchases.SupplierTerms.Manage")]
    public async Task<IActionResult> Save(SaveSupplierPurchasingInfoDto dto)
    {
        try
        {
            if (!ModelState.IsValid) throw new ArgumentException("Please check the entered supplier purchasing data.");
            await service.SaveAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Supplier purchasing information saved."].Value;
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to save supplier purchasing information");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer["The operation could not be completed. Please try again or contact the administrator."].Value;
        }
        return RedirectToAction(nameof(Index), new { supplierId = dto.SupplierId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Purchases.SupplierTerms.Manage")]
    public async Task<IActionResult> SetActive(int id, int? supplierId, bool isActive, byte[] rowVersion)
    {
        try
        {
            await service.SetActiveAsync(id, isActive, rowVersion);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = localizer["Supplier purchasing information updated."].Value;
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = localizer[exception.Message].Value;
        }
        return RedirectToAction(nameof(Index), new { supplierId });
    }
}
