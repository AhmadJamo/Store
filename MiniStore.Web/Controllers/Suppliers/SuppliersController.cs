using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Suppliers;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using Microsoft.Extensions.Localization;
using MiniStore.Domain.Entities;

namespace MiniStore.Web.Controllers;

public class SuppliersController : Controller
{
    private readonly SupplierService _supplierService;
    private readonly AccountService _accountService;
    private readonly ProductService _productService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public SuppliersController(
        SupplierService supplierService, AccountService accountService,
        ProductService productService, IStringLocalizer<SharedResource> localizer)
    {
        _supplierService = supplierService;
        _accountService = accountService;
        _productService = productService;
        _localizer = localizer;
    }

    [HttpGet]
    [Authorize]
    [PermissionAuthorize("Suppliers.View")]
    public async Task<IActionResult> Index()
    {
        var suppliers =
            await _supplierService.GetAllAsync();

        return View(suppliers);
    }

    [HttpGet]
    [PermissionAuthorize("Suppliers.Create")]
    public async Task<IActionResult> Create()
    {
        await LoadFormOptions();
        return View(new CreateSupplierDto());
    }

    [HttpPost]
    [PermissionAuthorize("Suppliers.Create")]
    public async Task<IActionResult> Create(
        CreateSupplierDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "Please check the entered data.";

            await LoadFormOptions(); return View(dto);
        }

        try
        {
            var supplierId = await _supplierService.CreateAsync(dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Supplier created successfully.";

            return RedirectToAction(nameof(Index), new { supplierId });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, _localizer[ex.Message]);
            await LoadFormOptions(); return View(dto);
        }
        catch (Exception ex)
        {
            HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType()).LogError(ex, "Request operation failed");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "The operation could not be completed. Please try again or contact the administrator.";

            await LoadFormOptions(); return View(dto);
        }
    }

    [HttpGet]
    [PermissionAuthorize("Suppliers.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var supplier =
            await _supplierService.GetByIdAsync(id);

        if (supplier == null)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "Supplier not found.";

            return RedirectToAction(nameof(Index));
        }

        var dto = new UpdateSupplierDto
        {
            Name = supplier.Name, LegalName = supplier.LegalName, ContactPerson = supplier.ContactPerson,
            Phone = supplier.Phone, Email = supplier.Email, Address = supplier.Address,
            TaxNumber = supplier.TaxNumber, RegistrationNumber = supplier.RegistrationNumber,
            Website = supplier.Website, PaymentTermsDays = supplier.PaymentTermsDays,
            Notes = supplier.Notes, IsActive = supplier.IsActive, AccountId = supplier.AccountId ?? 0,
            ProductIds = supplier.ProductIds, RowVersion = supplier.RowVersion
        };

        await LoadFormOptions(); return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize("Suppliers.Edit")]
    public async Task<IActionResult> Edit(
        int id,
        UpdateSupplierDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "Please check the entered data.";

            await LoadFormOptions(); return View(dto);
        }

        try
        {
            await _supplierService.UpdateAsync(id, dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Supplier updated successfully.";

            return RedirectToAction(nameof(Index), new { supplierId = id });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, _localizer[ex.Message]);
            await LoadFormOptions(); return View(dto);
        }
        catch (Exception ex)
        {
            HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType()).LogError(ex, "Request operation failed");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "The operation could not be completed. Please try again or contact the administrator.";

            await LoadFormOptions(); return View(dto);
        }
    }

    [HttpPost]
    [PermissionAuthorize("Suppliers.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _supplierService.DeleteAsync(id);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Supplier deleted successfully.";
        }
        catch (Exception ex)
        {
            HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType()).LogError(ex, "Request operation failed");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "The operation could not be completed. Please try again or contact the administrator.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadFormOptions()
    {
        ViewBag.Accounts = await _accountService.GetAllAsync();
        ViewBag.Products = (await _productService.GetAllAsync(null))
            .Where(x => x.IsActive && x.ProductType != ProductType.PreparedToOrder)
            .OrderBy(x => x.Name).ToList();
    }
}
