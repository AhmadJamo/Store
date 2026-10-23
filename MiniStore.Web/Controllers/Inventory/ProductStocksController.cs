using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.ProductStocks;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

[AutoValidateAntiforgeryToken]
public class ProductStocksController : Controller
{
    private readonly ProductStockService _productStockService;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ProductStocksController(
        ProductStockService productStockService,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        IStringLocalizer<SharedResource> localizer)
    {
        _productStockService = productStockService;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _localizer = localizer;
    }

    [HttpGet]
    [Authorize]
    [PermissionAuthorize("ProductStock.View")]
    public async Task<IActionResult> Index()
    {
        var stocks =
            await _productStockService.GetAllAsync();

        return View(stocks);
    }

    [HttpGet]
    [PermissionAuthorize("ProductStock.Create")]
    public async Task<IActionResult> Create()
    {
        var products =
            await _productRepository.GetAllAsync(null);

        var warehouses =
            await _warehouseRepository.GetAllAsync();

        ViewBag.Products = products
            .Where(product => product.IsActive &&
                product.ProductType != MiniStore.Domain.Entities.ProductType.PreparedToOrder)
            .ToList();
        ViewBag.Warehouses = warehouses;

        return View();
    }

    [HttpPost]
    [PermissionAuthorize("ProductStock.Create")]
    public async Task<IActionResult> Create(
        CreateProductStockDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns();

            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = _localizer["Please check the entered data."].Value;

            return View(dto);
        }

        try
        {
            await _productStockService.CreateAsync(dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = _localizer["Stock created successfully."].Value;

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType()).LogError(ex, "Request operation failed");
            await LoadDropdowns();

            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = _localizer["The operation could not be completed. Please try again or contact the administrator."].Value;

            return View(dto);
        }
    }

    [HttpGet]
    [PermissionAuthorize("ProductStock.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var stock =
            await _productStockService.GetByIdAsync(id);

        if (stock == null)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = _localizer["Stock record not found."].Value;

            return RedirectToAction(nameof(Index));
        }

        var dto = new UpdateProductStockDto
        {
            Quantity = stock.Quantity
        };

        ViewBag.ProductName = stock.ProductName;
        ViewBag.WarehouseName = stock.WarehouseName;

        return View(dto);
    }

    [HttpPost]
    [PermissionAuthorize("ProductStock.Edit")]
    public async Task<IActionResult> Edit(
        int id,
        UpdateProductStockDto dto)
    {
        if (!ModelState.IsValid)
        {
            var stock =
                await _productStockService.GetByIdAsync(id);

            if (stock != null)
            {
                ViewBag.ProductName = stock.ProductName;
                ViewBag.WarehouseName = stock.WarehouseName;
            }

            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = _localizer["Please check the entered data."].Value;

            return View(dto);
        }

        try
        {
            await _productStockService.UpdateAsync(id, dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = _localizer["Stock updated successfully."].Value;

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType()).LogError(ex, "Request operation failed");
            var stock =
                await _productStockService.GetByIdAsync(id);

            if (stock != null)
            {
                ViewBag.ProductName = stock.ProductName;
                ViewBag.WarehouseName = stock.WarehouseName;
            }

            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = _localizer["The operation could not be completed. Please try again or contact the administrator."].Value;

            return View(dto);
        }
    }

    [HttpPost]
    [PermissionAuthorize("ProductStock.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _productStockService.DeleteAsync(id);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = _localizer["Stock deleted successfully."].Value;
        }
        catch (Exception ex)
        {
            HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType()).LogError(ex, "Request operation failed");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = _localizer["The operation could not be completed. Please try again or contact the administrator."].Value;
        }

        return RedirectToAction(nameof(Index));
    }
   
    private async Task LoadDropdowns()
    {
        ViewBag.Products = (await _productRepository.GetAllAsync(null))
            .Where(product => product.IsActive &&
                product.ProductType != MiniStore.Domain.Entities.ProductType.PreparedToOrder)
            .ToList();

        ViewBag.Warehouses =
            await _warehouseRepository.GetAllAsync();
    }
}
