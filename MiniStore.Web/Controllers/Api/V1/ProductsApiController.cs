using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Products;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/products")]
public sealed class ProductsApiController(
    ProductService products,
    IStringLocalizer<SharedResource> localizer,
    ILogger<ProductsApiController> logger) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("Products.View")]
    public async Task<ActionResult<ProductListPageDto>> GetPage([FromQuery] ProductListQueryDto query) =>
        Ok(await products.SearchAsync(query));

    [HttpGet("{id:int}")]
    [PermissionAuthorize("Products.View")]
    public async Task<ActionResult<ProductDto>> Get(int id)
    {
        var product = await products.GetByIdAsync(id);
        return product is null ? NotFoundProblem() : Ok(product);
    }

    [HttpGet("form")]
    [PermissionAuthorize("Products.Create")]
    public async Task<ActionResult<ProductFormPageDto>> GetCreateForm() =>
        Ok(await products.GetFormPageAsync());

    [HttpGet("{id:int}/form")]
    [PermissionAuthorize("Products.Edit")]
    public async Task<ActionResult<ProductFormPageDto>> GetEditForm(int id) =>
        await products.GetFormPageAsync(id) is { } page ? Ok(page) : NotFoundProblem();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Create")]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto form)
    {
        try
        {
            var id = await products.CreateAsync(form);
            return CreatedAtAction(nameof(Get), new { id }, await products.GetByIdAsync(id));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BusinessProblem(exception);
        }
    }

    [HttpPut("{id:int}")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<ActionResult<ProductDto>> Update(int id, UpdateProductDto form)
    {
        try
        {
            await products.UpdateAsync(id, form);
            return Ok(await products.GetByIdAsync(id));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BusinessProblem(exception);
        }
    }

    [HttpDelete("{id:int}")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await products.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return BusinessProblem(exception);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(exception, "Product {ProductId} could not be deleted", id);
            return InternalApiProblemFactory.Create(HttpContext, StatusCodes.Status409Conflict,
                localizer["Product cannot be deleted."], InternalApiProblemCodes.Validation,
                localizer["The product is used by existing operations and cannot be deleted."]);
        }
    }

    private ActionResult BusinessProblem(Exception exception)
    {
        ModelState.AddModelError(string.Empty, localizer[exception.Message]);
        var code = exception.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase)
            ? InternalApiProblemCodes.Duplicate
            : InternalApiProblemCodes.Validation;
        return InternalApiProblemFactory.Validation(HttpContext, ModelState,
            localizer["One or more validation errors occurred."], code);
    }

    private ObjectResult NotFoundProblem() => InternalApiProblemFactory.Create(
        HttpContext, StatusCodes.Status404NotFound, localizer["Product not found."],
        InternalApiProblemCodes.NotFound);
}
