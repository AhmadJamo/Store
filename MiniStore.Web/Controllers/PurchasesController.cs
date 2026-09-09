using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Purchases;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class PurchasesController : Controller
{
    private readonly PurchaseService _purchaseService;

    private readonly ISupplierRepository _supplierRepository;

    private readonly IWarehouseRepository _warehouseRepository;

    private readonly IProductRepository _productRepository;

    public PurchasesController(
        PurchaseService purchaseService,
        ISupplierRepository supplierRepository,
        IWarehouseRepository warehouseRepository,
        IProductRepository productRepository)
    {
        _purchaseService = purchaseService;
        _supplierRepository = supplierRepository;
        _warehouseRepository = warehouseRepository;
        _productRepository = productRepository;
    }

    [HttpGet]
    [Authorize]
    [PermissionAuthorize("Purchases.View")]
    public async Task<IActionResult> Index()
    {
        var purchases =
            await _purchaseService.GetAllAsync();

        return View(purchases);
    }

    [HttpGet]
    [PermissionAuthorize("Purchases.Create")]
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();

        var dto = new CreatePurchaseDto
        {
            Date = DateTime.Today
        };

        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize("Purchases.Create")]
    public async Task<IActionResult> Create(
        CreatePurchaseDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns();

            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "Please check the entered data.";

            return View(dto);
        }

        try
        {
            await _purchaseService.CreateAsync(dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Purchase created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LoadDropdowns();

            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                ex.Message;

            return View(dto);
        }
    }

    [HttpGet]
    [PermissionAuthorize("Purchases.View")]
    public async Task<IActionResult> Details(int id)
    {
        var purchase =
            await _purchaseService.GetByIdAsync(id);

        if (purchase == null)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "Purchase not found.";

            return RedirectToAction(nameof(Index));
        }

        return View(purchase);
    }


    [Authorize]
    private async Task LoadDropdowns()
    {
        ViewBag.Suppliers =
            await _supplierRepository.GetAllAsync();

        ViewBag.Warehouses =
            await _warehouseRepository.GetAllAsync();

        ViewBag.Products =
            await _productRepository.GetAllAsync(null);
    }
}
