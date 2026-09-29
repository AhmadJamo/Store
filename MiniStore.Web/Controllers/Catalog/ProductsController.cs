using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Products;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;
using Microsoft.Extensions.Localization;

namespace MiniStore.Web.Controllers;

public class ProductsController : Controller
{
    private readonly ProductService _productService;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly MeasurementUnitService _measurementUnitService;
    private readonly ProductCategoryService _productCategoryService;

    public ProductsController(
        ProductService productService,
        IStringLocalizer<SharedResource> localizer,
        MeasurementUnitService measurementUnitService,
        ProductCategoryService productCategoryService)
    {
        _productService = productService;
        _localizer = localizer;
        _measurementUnitService = measurementUnitService;
        _productCategoryService = productCategoryService;
    }

    [PermissionAuthorize("Products.View")]
    public async Task<IActionResult> Index([FromQuery] ProductListQueryDto query)
    {
        return View(await _productService.SearchAsync(query));
    }

    [HttpGet]
    [PermissionAuthorize("Products.Create")]
    public async Task<IActionResult> Create()
    {
        await LoadMeasurementUnitsAsync();
        return View();
    }

   
    [HttpPost]
    [PermissionAuthorize("Products.Create")]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadMeasurementUnitsAsync();
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "Please check the entered data.";

            return View(dto);
        }

        try
        {
            await _productService.CreateAsync(dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Product created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            await LoadMeasurementUnitsAsync();
            ModelState.AddModelError(string.Empty, _localizer[ex.Message]);
            return View(dto);
        }
        catch (Exception ex)
        {
            await LoadMeasurementUnitsAsync();
            HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType()).LogError(ex, "Request operation failed");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "The operation could not be completed. Please try again or contact the administrator.";

            return View(dto);
        }
    }
   
    
    
    [HttpGet]
    [PermissionAuthorize("Products.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "Product not found.";

            return RedirectToAction(nameof(Index));
        }

        ViewBag.ProductId = id;
        await LoadMeasurementUnitsAsync();
        var dto = new UpdateProductDto
        {
            Name = product.Name,
            Barcode = product.Barcode,
            PurchasePrice = product.PurchasePrice,
            SalePrice = product.SalePrice,
            WholesalePrice = product.WholesalePrice,
            InventoryBehavior = product.InventoryBehavior,
            ProductType = product.ProductType,
            StockUnit = product.StockUnit,
            MeasurementUnitId = product.MeasurementUnitId ?? 0,
            AllowNegativeRecipeConsumption = product.AllowNegativeRecipeConsumption,
            IsSellableInPos = product.IsSellableInPos,
            IsSellableInSales = product.IsSellableInSales,
            IsActive = product.IsActive,
            ProductCategoryId = product.ProductCategoryId,
            NetWeight = product.NetWeight,
            GrossWeight = product.GrossWeight,
            WeightMeasurementUnitId = product.WeightMeasurementUnitId,
            Length = product.Length,
            Width = product.Width,
            Height = product.Height,
            DimensionMeasurementUnitId = product.DimensionMeasurementUnitId,
            TrackingPolicy = product.TrackingPolicy,
            IsFragile = product.HandlingRequirements.HasFlag(ProductHandlingRequirements.Fragile),
            KeepDry = product.HandlingRequirements.HasFlag(ProductHandlingRequirements.KeepDry),
            RequiresRefrigeration = product.HandlingRequirements.HasFlag(ProductHandlingRequirements.Refrigerated),
            RequiresFrozenStorage = product.HandlingRequirements.HasFlag(ProductHandlingRequirements.Frozen),
            IsHazardous = product.HandlingRequirements.HasFlag(ProductHandlingRequirements.Hazardous)
        };

        return View(dto);
    }
   
    
    [HttpPost]
    [PermissionAuthorize("Products.Edit")]
    public async Task<IActionResult> Edit(
    int id,
    UpdateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ProductId = id;
            await LoadMeasurementUnitsAsync();
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "Please check the entered data.";

            return View(dto);
        }

        try
        {
            await _productService.UpdateAsync(id, dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Product updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ViewBag.ProductId = id;
            await LoadMeasurementUnitsAsync();
            ModelState.AddModelError(string.Empty, _localizer[ex.Message]);
            return View(dto);
        }
        catch (Exception ex)
        {
            ViewBag.ProductId = id;
            await LoadMeasurementUnitsAsync();
            HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(GetType()).LogError(ex, "Request operation failed");
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                "The operation could not be completed. Please try again or contact the administrator.";

            return View(dto);
        }
    }
    
    
    [HttpPost]
    [PermissionAuthorize("Products.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _productService.DeleteAsync(id);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Product deleted successfully.";
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

    private async Task LoadMeasurementUnitsAsync()
    {
        var units = await _measurementUnitService.GetActiveAsync();
        ViewBag.MeasurementUnits = units;
        ViewBag.MassUnits = units.Where(unit => unit.Dimension == MeasurementDimension.Mass).ToList();
        ViewBag.LengthUnits = units.Where(unit => unit.Dimension == MeasurementDimension.Length).ToList();
        ViewBag.ProductCategories = await _productCategoryService.GetAllAsync();
    }
}
