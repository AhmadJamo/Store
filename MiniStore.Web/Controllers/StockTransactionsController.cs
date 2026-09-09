using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.ProductStocks;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class StockTransactionsController : Controller
{
    private readonly StockTransactionService
        _stockTransactionService;

    private readonly IProductRepository
        _productRepository;

    private readonly IWarehouseRepository
        _warehouseRepository;

    public StockTransactionsController(
        StockTransactionService stockTransactionService,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository)
    {
        _stockTransactionService =
            stockTransactionService;

        _productRepository =
            productRepository;

        _warehouseRepository =
            warehouseRepository;
    }

    [HttpGet]
    [Authorize]
    [PermissionAuthorize("StockTransactions.View")]
    public async Task<IActionResult> Index()
    {
        var transactions =
            await _stockTransactionService.GetAllAsync();

        return View(transactions);
    }

    [HttpGet]
    [PermissionAuthorize("StockTransactions.Create")]
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();

        return View();
    }

    [HttpPost]
    [PermissionAuthorize("StockTransactions.Create")]
    public async Task<IActionResult> Create(
        CreateStockTransactionDto dto)
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
            await _stockTransactionService
                .CreateAsync(dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Stock transaction created successfully.";

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

    [Authorize]
    private async Task LoadDropdowns()
    {
        ViewBag.Products =
            await _productRepository.GetAllAsync(null);

        ViewBag.Warehouses =
            await _warehouseRepository.GetAllAsync();
    }
}
