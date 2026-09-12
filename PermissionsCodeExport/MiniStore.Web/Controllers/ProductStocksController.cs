using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.ProductStocks;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class ProductStocksController : Controller
{
    private readonly ProductStockService _productStockService;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;

    public ProductStocksController(
        ProductStockService productStockService,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository)
    {
        _productStockService = productStockService;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
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

        ViewBag.Products = products;
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
            TempData["NotificationMessage"] =
                "Please check the entered data.";

            return View(dto);
        }

        try
        {
            await _productStockService.CreateAsync(dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Stock created successfully.";

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
    [PermissionAuthorize("ProductStock.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var stock =
            await _productStockService.GetByIdAsync(id);

        if (stock == null)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "Stock record not found.";

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
            TempData["NotificationMessage"] =
                "Please check the entered data.";

            return View(dto);
        }

        try
        {
            await _productStockService.UpdateAsync(id, dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Stock updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            var stock =
                await _productStockService.GetByIdAsync(id);

            if (stock != null)
            {
                ViewBag.ProductName = stock.ProductName;
                ViewBag.WarehouseName = stock.WarehouseName;
            }

            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                ex.Message;

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
            TempData["NotificationMessage"] =
                "Stock deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
   
    private async Task LoadDropdowns()
    {
        ViewBag.Products =
            await _productRepository.GetAllAsync(null);

        ViewBag.Warehouses =
            await _warehouseRepository.GetAllAsync();
    }
}