using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Recipes;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/recipes")]
public sealed class RecipesApiController(
    RecipeService recipes,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("Products.View")]
    public async Task<ActionResult<List<RecipeListItemDto>>> GetAll() => Ok(await recipes.GetAllAsync());

    [HttpGet("{productId:int}")]
    [PermissionAuthorize("Products.Edit")]
    public async Task<ActionResult<RecipeEditorDto>> GetEditor(int productId)
    {
        var editor = await recipes.GetEditorAsync(productId);
        return editor is null
            ? InternalApiProblemFactory.Create(HttpContext, StatusCodes.Status404NotFound,
                localizer["Product not found."], InternalApiProblemCodes.NotFound)
            : Ok(editor);
    }

    [HttpPut("{productId:int}")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Products.Edit")]
    public async Task<ActionResult<RecipeEditorDto>> Save(int productId, RecipeEditorDto form)
    {
        if (productId != form.ProductId)
        {
            ModelState.AddModelError(nameof(form.ProductId), localizer["Product not found."]);
            return ValidationProblem();
        }
        try
        {
            await recipes.SaveNewVersionAsync(form, User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System");
            return Ok(await recipes.GetEditorAsync(productId));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty,
                localizer["Recipe could not be saved. Check ingredients, units, and quantities."]);
            return InternalApiProblemFactory.Validation(HttpContext, ModelState,
                localizer["One or more validation errors occurred."], InternalApiProblemCodes.Validation);
        }
    }
}
