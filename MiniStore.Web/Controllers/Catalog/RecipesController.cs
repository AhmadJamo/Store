using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.DTOs.Recipes;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public class RecipesController : Controller
{
    private readonly RecipeService _recipeService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public RecipesController(
        RecipeService recipeService,
        IStringLocalizer<SharedResource> localizer)
    {
        _recipeService = recipeService;
        _localizer = localizer;
    }

    [HttpGet]
    [PermissionAuthorize("Products.View")]
    public async Task<IActionResult> Index()
    {
        return View(await _recipeService.GetAllAsync());
    }

    [HttpGet]
    [PermissionAuthorize("Products.Edit")]
    public async Task<IActionResult> Edit(int productId)
    {
        var model = await _recipeService.GetEditorAsync(productId);
        if (model is null)
            return NotFound();
        return View(model);
    }

    [HttpPost]
    [PermissionAuthorize("Products.Edit")]
    public async Task<IActionResult> Edit(RecipeEditorDto model)
    {
        if (!ModelState.IsValid)
            return View(await RebuildEditorAsync(model));

        try
        {
            await _recipeService.SaveNewVersionAsync(
                model,
                User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System");
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = _localizer["Recipe version saved successfully."].Value;
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException)
        {
            ModelState.AddModelError(
                string.Empty,
                _localizer["Recipe could not be saved. Check ingredients, units, and quantities."]);
        }
        catch (InvalidOperationException)
        {
            ModelState.AddModelError(
                string.Empty,
                _localizer["Recipe could not be saved. Check ingredients, units, and quantities."]);
        }

        return View(await RebuildEditorAsync(model));
    }

    private async Task<RecipeEditorDto> RebuildEditorAsync(RecipeEditorDto submitted)
    {
        var editor = await _recipeService.GetEditorAsync(submitted.ProductId)
            ?? submitted;
        editor.YieldQuantity = submitted.YieldQuantity;
        editor.Ingredients = submitted.Ingredients;
        return editor;
    }
}
