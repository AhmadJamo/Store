using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Products;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class ProductsController : Controller
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    [PermissionAuthorize("Products.View")]
    public async Task<IActionResult> Index(string? search)
    {
        var products =
            await _productService.GetAllAsync(search);

        ViewBag.Search = search;

        return View(products);
    }

    [HttpGet]
    [PermissionAuthorize("Products.Create")]
    public IActionResult Create()
    {
        return View();
    }

   
    [HttpPost]
    [PermissionAuthorize("Products.Create")]
    public async Task<IActionResult> Create(CreateProductDto dto)
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
            await _productService.CreateAsync(dto);

            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] =
                "Product created successfully.";

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

        var dto = new UpdateProductDto
        {
            Name = product.Name,
            Barcode = product.Barcode,
            PurchasePrice = product.PurchasePrice,
            SalePrice = product.SalePrice,
            WholesalePrice = product.WholesalePrice
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
        catch (Exception ex)
        {
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                ex.Message;

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
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] =
                ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
