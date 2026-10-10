using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MiniStore.Application.Dtos.Settings;
using MiniStore.Application.DTOs.Settings;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;
using Microsoft.Extensions.Localization;

namespace MiniStore.Web.Controllers;

[PermissionAuthorize("Administration.Access")]
public class SettingsController : Controller
{
    private readonly DocumentNumberService _documentNumberService;
    private readonly GeneralSettingsService _generalSettingsService;
    private readonly DiscountSettingsService _discountSettingsService;
    private readonly AccountingSettingsService _accountingSettingsService;
    private readonly PurchaseMatchingSettingsService _purchaseMatchingSettingsService;
    private readonly AccountService _accountService;
    private readonly InventorySettingsService _inventorySettingsService;
    private readonly InventoryAccessService _inventoryAccessService;
    private readonly PosExperienceSettingsService _posExperienceSettingsService;
    private readonly MeasurementUnitService _measurementUnitService;
    private readonly FiscalPeriodService _fiscalPeriodService;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IMemoryCache _memoryCache;
    private readonly ITenantContext _tenantContext;

    public SettingsController(
        DocumentNumberService documentNumberService,
        GeneralSettingsService generalSettingsService,
        DiscountSettingsService discountSettingsService,
        AccountingSettingsService accountingSettingsService,
        PurchaseMatchingSettingsService purchaseMatchingSettingsService,
        AccountService accountService,
        InventorySettingsService inventorySettingsService,
        InventoryAccessService inventoryAccessService,
        PosExperienceSettingsService posExperienceSettingsService,
        MeasurementUnitService measurementUnitService,
        FiscalPeriodService fiscalPeriodService,
        IStringLocalizer<SharedResource> localizer,
        IMemoryCache memoryCache,
        ITenantContext tenantContext)
    {
        _documentNumberService = documentNumberService;
        _generalSettingsService = generalSettingsService;
        _discountSettingsService = discountSettingsService;
        _accountingSettingsService = accountingSettingsService;
        _purchaseMatchingSettingsService = purchaseMatchingSettingsService;
        _accountService = accountService;
        _inventorySettingsService = inventorySettingsService;
        _inventoryAccessService = inventoryAccessService;
        _posExperienceSettingsService = posExperienceSettingsService;
        _measurementUnitService = measurementUnitService;
        _fiscalPeriodService = fiscalPeriodService;
        _localizer = localizer;
        _memoryCache = memoryCache;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> FiscalPeriods() =>
        View(await _fiscalPeriodService.GetPageAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FiscalPeriods(CreateFiscalPeriodDto dto)
    {
        if (!ModelState.IsValid)
            return View(await _fiscalPeriodService.GetPageAsync(dto));
        try
        {
            await _fiscalPeriodService.CreateAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = _localizer["Fiscal period created successfully."].Value;
            return RedirectToAction(nameof(FiscalPeriods));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, _localizer[ex.Message]);
            return View(await _fiscalPeriodService.GetPageAsync(dto));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeFiscalPeriodStatus(ChangeFiscalPeriodStatusDto dto)
    {
        try
        {
            await _fiscalPeriodService.ChangeStatusAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = _localizer["Fiscal period status updated."].Value;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = _localizer[ex.Message].Value;
        }
        return RedirectToAction(nameof(FiscalPeriods));
    }

    [HttpGet]
    public async Task<IActionResult> Units()
    {
        return View(await _measurementUnitService.GetPageAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Units(CreateMeasurementUnitDto dto)
    {
        if (!ModelState.IsValid)
            return View(await _measurementUnitService.GetPageAsync(dto));

        try
        {
            await _measurementUnitService.CreateAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = _localizer["Measurement unit created successfully."].Value;
            return RedirectToAction(nameof(Units));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, _localizer[ex.Message]);
            return View(await _measurementUnitService.GetPageAsync(dto));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateUnit(int id)
    {
        try
        {
            await _measurementUnitService.DeactivateAsync(id);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = _localizer["Measurement unit deactivated successfully."].Value;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = _localizer[ex.Message].Value;
        }
        return RedirectToAction(nameof(Units));
    }

    [HttpGet]
    public async Task<IActionResult> Inventory()
    {
        return View(await _inventorySettingsService.GetAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inventory(InventorySettingsDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            await _inventorySettingsService.UpdateAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = "Inventory settings updated successfully.";
            return RedirectToAction(nameof(Inventory));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> InventoryAccess()
    {
        return View(await _inventoryAccessService.GetPageAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfigureBranchWarehouse(
        ConfigureBranchWarehouseDto dto)
    {
        try
        {
            await _inventoryAccessService.ConfigureBranchWarehouseAsync(dto);
            SetInventoryAccessMessage("success", "Branch warehouse access saved.");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            SetInventoryAccessMessage("error", ex.Message);
        }

        return RedirectToAction(nameof(InventoryAccess));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePosTerminal(CreatePosTerminalDto dto)
    {
        try
        {
            await _inventoryAccessService.CreatePosTerminalAsync(dto);
            SetInventoryAccessMessage("success", "POS terminal created.");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            SetInventoryAccessMessage("error", ex.Message);
        }

        return RedirectToAction(nameof(InventoryAccess));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfigurePosTerminalWarehouse(
        ConfigurePosTerminalWarehouseDto dto)
    {
        try
        {
            await _inventoryAccessService.ConfigurePosTerminalWarehouseAsync(dto);
            SetInventoryAccessMessage("success", "POS warehouse priority saved.");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            SetInventoryAccessMessage("error", ex.Message);
        }

        return RedirectToAction(nameof(InventoryAccess));
    }

    private void SetInventoryAccessMessage(string type, string message)
    {
        TempData["NotificationType"] = type;
        TempData["NotificationMessage"] = message;
    }

    [HttpGet]
    public async Task<IActionResult> Pos(int? terminalId)
    {
        return View(await _posExperienceSettingsService.GetPageAsync(terminalId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pos(PosExperienceSettingsDto dto)
    {
        try
        {
            await _posExperienceSettingsService.UpdateAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = dto.ApplyProfileDefaults
                ? "POS profile defaults applied."
                : "POS appearance settings saved.";
            return RedirectToAction(nameof(Pos), new { terminalId = dto.PosTerminalId });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(await _posExperienceSettingsService.GetPageAsync(dto.PosTerminalId));
        }
    }

    [HttpGet]
    public IActionResult Invoices() => RedirectToAction(nameof(DocumentNumbers));

    [HttpGet]
    public async Task<IActionResult> DocumentNumbers() =>
        View(await _documentNumberService.GetPageAsync());

    [HttpPost]
    public async Task<IActionResult> DocumentNumbers(DocumentNumberSettingsPageDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        try
        {
            await _documentNumberService.UpdateAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = "Document numbering settings updated successfully.";
            return RedirectToAction(nameof(DocumentNumbers));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(await _documentNumberService.GetPageAsync());
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
            _memoryCache.Remove(
                MiniStore.Web.Localization.DatabaseRequestCultureProvider.GetCacheKey(_tenantContext.TenantId));

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

    [HttpGet]
    public async Task<IActionResult> Accounting()
    {
        await LoadAccountsAsync();
        return View(await _accountingSettingsService.GetAsync());
    }

    [HttpGet]
    public async Task<IActionResult> PurchaseMatching() => View(await _purchaseMatchingSettingsService.GetAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PurchaseMatching(PurchaseMatchingSettingsDto dto)
    {
        try
        {
            await _purchaseMatchingSettingsService.UpdateAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = _localizer["Purchase matching settings updated."].Value;
            return RedirectToAction(nameof(PurchaseMatching));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, _localizer[ex.Message]);
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accounting(AccountingSettingsDto dto)
    {
        try
        {
            await _accountingSettingsService.UpdateAsync(dto);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = "Accounting posting accounts updated successfully.";
            return RedirectToAction(nameof(Accounting));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadAccountsAsync();
            return View(dto);
        }
    }

    private async Task LoadAccountsAsync()
    {
        ViewBag.Accounts = await _accountService.GetAllAsync();
    }

    [HttpGet]
    public async Task<IActionResult> BranchSalesAccounts([FromServices] BranchService branches)
    {
        await LoadAccountsAsync();
        ViewBag.Branches = await branches.GetAllAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BranchSalesAccounts(int branchId, int accountId, [FromServices] BranchService branches)
    {
        try
        {
            await branches.ConfigureSalesAccountAsync(branchId, accountId);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = "Branch sales account updated.";
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(BranchSalesAccounts));
    }
}
