using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MiniStore.Application.DTOs.Sales;
using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;
using Microsoft.Extensions.Localization;

namespace MiniStore.Web.Controllers;

public class SalesController : Controller
{
    private readonly ISaleService _saleService;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IProductStockRepository _productStockRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly InventoryAccessService _inventoryAccessService;
    private readonly PosExperienceSettingsService _posExperienceSettingsService;
    private readonly SalePostingService _salePostingService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public SalesController(
        ISaleService saleService,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        IProductStockRepository productStockRepository,
        ICustomerRepository customerRepository,
        IPaymentMethodRepository paymentMethodRepository,
        InventoryAccessService inventoryAccessService,
        PosExperienceSettingsService posExperienceSettingsService,
        SalePostingService salePostingService,
        IStringLocalizer<SharedResource> localizer)
    {
        _saleService = saleService;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _productStockRepository = productStockRepository;
        _customerRepository = customerRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _inventoryAccessService = inventoryAccessService;
        _posExperienceSettingsService = posExperienceSettingsService;
        _salePostingService = salePostingService;
        _localizer = localizer;
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

        ViewBag.IsPosted = await _salePostingService.IsPostedAsync(sale.InvoiceNumber);
        return View(sale);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Sales.Create")]
    public async Task<IActionResult> Post(int id)
    {
        try
        {
            await _salePostingService.PostAsync(id);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = _localizer["Sale posted to the general ledger."].Value;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = _localizer[ex.Message].Value;
        }

        return RedirectToAction(nameof(Details), new { id });
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
        await LoadDropdowns(posOnly: true);

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
            await LoadDropdowns(channel == SaleChannel.RetailPos);
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
            await LoadDropdowns(channel == SaleChannel.RetailPos);

            ModelState.AddModelError(
                string.Empty,
                _localizer[ex.Message]);

            return View(dto);
        }
        catch (InvalidOperationException ex)
        {
            await LoadDropdowns(channel == SaleChannel.RetailPos);

            ModelState.AddModelError(
                string.Empty,
                _localizer[ex.Message]);

            return View(dto);
        }
    }

    private async Task LoadDropdowns(bool posOnly = false)
    {
        var products = await _productRepository.GetAllAsync(null);
        ViewBag.Products = products
            .Where(product => product.IsActive &&
                (posOnly ? product.IsSellableInPos : product.IsSellableInSales))
            .ToList();

        var warehouses = await _warehouseRepository.GetAllAsync();
        ViewBag.Warehouses = posOnly
            ? warehouses.Where(warehouse => warehouse.AllowPosSales).ToList()
            : warehouses;

        if (posOnly)
        {
            ViewBag.PosTerminals = (await _inventoryAccessService.GetPageAsync())
                .PosTerminals
                .Where(terminal => terminal.IsActive)
                .ToList();
            ViewBag.PosRuntimeSettings =
                await _posExperienceSettingsService.GetRuntimeSettingsAsync();
        }

        ViewBag.ProductStocks =
            await _productStockRepository.GetAllAsync();

        ViewBag.Customers = await _customerRepository.GetAllAsync();
        ViewBag.PaymentMethods = await _paymentMethodRepository.GetAllAsync();
    }
}
