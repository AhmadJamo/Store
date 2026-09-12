using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Suppliers;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class SuppliersController : Controller
{
    private readonly SupplierService _supplierService;

    public SuppliersController(
        SupplierService supplierService)
    {
        _supplierService = supplierService;
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
    public IActionResult Create()
    {
        return View();
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

            return View(dto);
        }

        try
        {
            await _supplierService.CreateAsync(dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Supplier created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                ex.Message;

            return View(dto);
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
            Name = supplier.Name,
            Phone = supplier.Phone,
            Address = supplier.Address
        };

        return View(dto);
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

            return View(dto);
        }

        try
        {
            await _supplierService.UpdateAsync(id, dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Supplier updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                ex.Message;

            return View(dto);
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
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}