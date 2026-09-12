using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MiniStore.Application.DTOs.Sales;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class SalesController : Controller
{
    private readonly ISaleService _saleService;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IProductStockRepository _productStockRepository;

    public SalesController(
        ISaleService saleService,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        IProductStockRepository productStockRepository)
    {
        _saleService = saleService;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _productStockRepository = productStockRepository;
    }

    [PermissionAuthorize("Sales.View")]
    public async Task<IActionResult> Index()
    {
        var sales =
            await _saleService.GetAllAsync();

        return View(sales);
    }

    [PermissionAuthorize("Sales.View")]
    public async Task<IActionResult> Details(int id)
    {
        var sale =
            await _saleService.GetByIdAsync(id);

        if (sale == null)
            return NotFound();

        return View(sale);
    }

    [PermissionAuthorize("Sales.Create")]
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();

        var dto = new CreateSaleDto
        {
            Date = DateTime.Today
        };

        return View(dto);
    }

    [HttpGet]
    [PermissionAuthorize("Sales.Create")]
    public async Task<IActionResult> Pos()
    {
        await LoadDropdowns();

        return View(new CreateSaleDto
        {
            Date = DateTime.Today
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Sales.Create")]
    public async Task<IActionResult> Create(
        CreateSaleDto dto,
        SaleChannel channel = SaleChannel.Wholesale)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns();
            return View(dto);
        }

        try
        {
            var saleId =
                await _saleService.CreateAsync(
                    dto,
                    channel,
                    User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? throw new InvalidOperationException(
                            "The current user could not be identified."));

            return RedirectToAction(
                nameof(Details),
                new { id = saleId });
        }
        catch (ArgumentException ex)
        {
            await LoadDropdowns();

            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            return View(dto);
        }
        catch (InvalidOperationException ex)
        {
            await LoadDropdowns();

            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            return View(dto);
        }
    }

    private async Task LoadDropdowns()
    {
        ViewBag.Products =
            await _productRepository.GetAllAsync(null);

        ViewBag.Warehouses =
            await _warehouseRepository.GetAllAsync();

        ViewBag.ProductStocks =
            await _productStockRepository.GetAllAsync();
    }
}
