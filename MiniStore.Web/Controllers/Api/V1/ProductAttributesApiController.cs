using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Products;
using MiniStore.Application.Exceptions;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/product-attributes")]
public sealed class ProductAttributesApiController(
    ProductAttributeService attributes,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("Products.View")]
    public async Task<ActionResult<ProductAttributePageDto>> GetPage() =>
        Ok(await attributes.GetPageAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<ActionResult<ProductAttributeDefinitionDto>> Create(CreateProductAttributeDto form)
    {
        try
        {
            var id = await attributes.CreateAsync(form);
            var created = (await attributes.GetPageAsync()).Definitions.Single(item => item.Id == id);
            return CreatedAtAction(nameof(GetPage), created);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BusinessProblem(exception);
        }
    }

    [HttpPut("{id:int}/status")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<ActionResult<ProductAttributeDefinitionDto>> SetStatus(
        int id,
        SetProductAttributeStatusDto form)
    {
        try
        {
            await attributes.SetActiveAsync(id, form.IsActive);
            var updated = (await attributes.GetPageAsync()).Definitions.Single(item => item.Id == id);
            return Ok(updated);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BusinessProblem(exception);
        }
    }

    [HttpGet("products/{productId:int}/values")]
    [PermissionAuthorize("Products.Edit")]
    public async Task<ActionResult<ProductAttributeValuesPageDto>> GetValues(int productId)
    {
        try { return Ok(await attributes.GetValuesPageAsync(productId)); }
        catch (ResourceNotFoundException exception)
        {
            return InternalApiProblemFactory.ApplicationProblem(HttpContext, exception, localizer[exception.Message]);
        }
    }

    [HttpPut("products/{productId:int}/values")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<ActionResult<ProductAttributeValuesPageDto>> SaveValues(
        int productId,
        SaveProductAttributeValuesDto form)
    {
        try
        {
            await attributes.SaveValuesAsync(productId, form.Values);
            return Ok(await attributes.GetValuesPageAsync(productId));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BusinessProblem(exception);
        }
    }

    private ActionResult BusinessProblem(Exception exception)
    {
        if (exception is InvalidOperationException typed &&
            typed is ResourceNotFoundException or BusinessConflictException or DuplicateResourceException or OptimisticConcurrencyException)
            return InternalApiProblemFactory.ApplicationProblem(HttpContext, typed, localizer[typed.Message]);
        ModelState.AddModelError(string.Empty, localizer[exception.Message]);
        return InternalApiProblemFactory.Validation(HttpContext, ModelState,
            localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
    }
}
