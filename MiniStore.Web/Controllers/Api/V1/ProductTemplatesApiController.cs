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
[Route("api/v1/product-templates")]
public sealed class ProductTemplatesApiController(
    ProductTemplateService templates,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("Products.View")]
    public async Task<ActionResult<ProductTemplatePageDto>> GetPage() => Ok(await templates.GetPageAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<ActionResult<ProductTemplatePageDto>> Create(CreateProductTemplateDto form) =>
        await Execute(async () => { await templates.CreateAsync(form); return await templates.GetPageAsync(); });

    [HttpPut("assignment")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<ActionResult<ProductTemplatePageDto>> Assign(AssignProductTemplateDto form) =>
        await Execute(async () => { await templates.AssignAsync(form); return await templates.GetPageAsync(); });

    [HttpPost("preview")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<ActionResult<ProductTemplatePageDto>> Preview(VariantGenerationInputDto form) =>
        await Execute(() => templates.PreviewGenerationAsync(form));

    [HttpPost("generate")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<ActionResult<VariantGenerationResultDto>> Generate(VariantGenerationInputDto form) =>
        await Execute(async () =>
        {
            var count = await templates.GenerateAsync(form);
            return new VariantGenerationResultDto { CreatedCount = count, Page = await templates.GetPageAsync() };
        });

    private async Task<ActionResult<T>> Execute<T>(Func<Task<T>> operation)
    {
        try { return Ok(await operation()); }
        catch (InvalidOperationException exception) when (exception is ResourceNotFoundException or BusinessConflictException or DuplicateResourceException or OptimisticConcurrencyException)
        {
            return InternalApiProblemFactory.ApplicationProblem(HttpContext, exception, localizer[exception.Message]);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, localizer[exception.Message]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
        }
    }
}
