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
[Route("api/v1/product-categories")]
public sealed class ProductCategoriesApiController(
    ProductCategoryService categories,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("Products.View")]
    public async Task<ActionResult<IReadOnlyList<ProductCategoryDto>>> GetAll() =>
        Ok(await categories.GetAllAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<ActionResult<ProductCategoryDto>> Create(CreateProductCategoryDto form)
    {
        try
        {
            var id = await categories.CreateAsync(form);
            return CreatedAtAction(nameof(GetAll), await categories.GetByIdAsync(id));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BusinessProblem(exception);
        }
    }

    [HttpPut("{id:int}/status")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<ActionResult<ProductCategoryDto>> SetStatus(
        int id,
        SetProductCategoryStatusDto form)
    {
        try
        {
            await categories.SetActiveAsync(id, form.IsActive);
            return Ok(await categories.GetByIdAsync(id));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BusinessProblem(exception);
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
}
