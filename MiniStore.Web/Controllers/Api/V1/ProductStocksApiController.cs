using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.ProductStocks;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/product-stocks")]
public sealed class ProductStocksApiController(
    ProductStockService stocks,
    ProductService products,
    WarehouseService warehouses,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet, PermissionAuthorize("ProductStock.View")]
    public async Task<ActionResult<List<ProductStockDto>>> GetAll() => Ok(await stocks.GetAllAsync());

    [HttpGet("form"), PermissionAuthorize("ProductStock.Create")]
    public async Task<ActionResult<ProductStockFormPageDto>> GetCreateForm() => Ok(await Form(new()));

    [HttpGet("{id:int}/form"), PermissionAuthorize("ProductStock.Edit")]
    public async Task<ActionResult<ProductStockFormPageDto>> GetEditForm(int id)
    {
        var stock = await stocks.GetByIdAsync(id);
        return stock is null
            ? InternalApiProblemFactory.Create(HttpContext, 404, localizer["Stock record not found."], InternalApiProblemCodes.NotFound)
            : Ok(await Form(new CreateProductStockDto { ProductId = stock.ProductId, WarehouseId = stock.WarehouseId, Quantity = stock.Quantity }));
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("ProductStock.Create")]
    public async Task<ActionResult<ProductStockDto>> Create(CreateProductStockDto form) => await Execute(async () =>
    {
        await stocks.CreateAsync(form);
        return (await stocks.GetAllAsync()).Single(x => x.ProductId == form.ProductId && x.WarehouseId == form.WarehouseId);
    });

    [HttpPut("{id:int}"), ValidateAntiForgeryToken, PermissionAuthorize("ProductStock.Edit")]
    public async Task<ActionResult<ProductStockDto>> Update(int id, UpdateProductStockDto form) => await Execute(async () =>
    {
        await stocks.UpdateAsync(id, form);
        return (await stocks.GetByIdAsync(id))!;
    });

    [HttpDelete("{id:int}"), ValidateAntiForgeryToken, PermissionAuthorize("ProductStock.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        try { await stocks.DeleteAsync(id); return NoContent(); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { return Problem(exception); }
    }

    private async Task<ProductStockFormPageDto> Form(CreateProductStockDto form) => new()
    {
        Form = form,
        Products = (await products.GetAllAsync(null)).Where(x => x.IsActive && x.ProductType != ProductType.PreparedToOrder)
            .Select(x => new StockSelectionOptionDto(x.Id, x.Name)).ToList(),
        Warehouses = (await warehouses.GetAllAsync()).Select(x => new StockSelectionOptionDto(x.Id, x.Name)).ToList()
    };

    private async Task<ActionResult<T>> Execute<T>(Func<Task<T>> action)
    {
        try { return Ok(await action()); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { return Problem(exception); }
    }

    private ActionResult Problem(Exception exception)
    {
        ModelState.AddModelError(string.Empty, localizer[exception.Message]);
        return InternalApiProblemFactory.Validation(HttpContext, ModelState,
            localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
    }
}
