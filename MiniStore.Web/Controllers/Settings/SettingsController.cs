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
    private readonly AccountingSettingsService _accountingSettingsService;
    private readonly AccountService _accountService;
    private readonly InventorySettingsService _inventorySettingsService;
    private readonly InventoryAccessService _inventoryAccessService;

    public SettingsController(
        InvoiceSettingsService invoiceSettingsService,
        GeneralSettingsService generalSettingsService,
        DiscountSettingsService discountSettingsService,
        AccountingSettingsService accountingSettingsService,
        AccountService accountService,
        InventorySettingsService inventorySettingsService,
        InventoryAccessService inventoryAccessService)
    {
        _invoiceSettingsService = invoiceSettingsService;
        _generalSettingsService = generalSettingsService;
        _discountSettingsService = discountSettingsService;
        _accountingSettingsService = accountingSettingsService;
        _accountService = accountService;
        _inventorySettingsService = inventorySettingsService;
        _inventoryAccessService = inventoryAccessService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
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

    [HttpGet]
    public async Task<IActionResult> Accounting()
    {
        await LoadAccountsAsync();
        return View(await _accountingSettingsService.GetAsync());
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
