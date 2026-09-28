using MiniStore.Application.DTOs.Recipes;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class RecipeService
{
    private readonly IProductRecipeRepository _recipeRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMeasurementUnitRepository _measurementUnitRepository;
    private readonly MeasurementUnitService _measurementUnitService;
    private readonly IUnitOfWork _unitOfWork;

    public RecipeService(
        IProductRecipeRepository recipeRepository,
        IProductRepository productRepository,
        IMeasurementUnitRepository measurementUnitRepository,
        MeasurementUnitService measurementUnitService,
        IUnitOfWork unitOfWork)
    {
        _recipeRepository = recipeRepository;
        _productRepository = productRepository;
        _measurementUnitRepository = measurementUnitRepository;
        _measurementUnitService = measurementUnitService;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<RecipeListItemDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync(null);
        var recipes = (await _recipeRepository.GetActiveAsync())
            .ToDictionary(x => x.ProductId);

        return products
            .Where(x => x.InventoryBehavior == ProductInventoryBehavior.PreparedToOrder ||
                        recipes.ContainsKey(x.Id))
            .OrderBy(x => x.Name)
            .Select(product =>
            {
                recipes.TryGetValue(product.Id, out var recipe);
                return new RecipeListItemDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    InventoryBehavior = product.InventoryBehavior,
                    ActiveVersion = recipe?.VersionNumber,
                    YieldQuantity = recipe?.YieldQuantity,
                    IngredientCount = recipe?.Ingredients.Count ?? 0
                };
            })
            .ToList();
    }

    public async Task<RecipeEditorDto?> GetEditorAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product is null)
            return null;

        var products = await _productRepository.GetAllAsync(null);
        var recipe = await _recipeRepository.GetActiveByProductIdAsync(productId);
        var units = await _measurementUnitService.GetActiveAsync();
        var unitByCode = units.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

        return new RecipeEditorDto
        {
            ProductId = product.Id,
            ProductName = product.Name,
            ActiveVersion = recipe?.VersionNumber,
            YieldQuantity = recipe?.YieldQuantity ?? 1m,
            Ingredients = recipe?.Ingredients
                .OrderBy(x => x.Id)
                .Select(x => new RecipeIngredientInputDto
                {
                    IngredientProductId = x.IngredientProductId,
                    Quantity = x.Quantity,
                    MeasurementUnitId = x.MeasurementUnitId ??
                        unitByCode[MapLegacyUnitCode(x.Unit)].Id
                })
                .ToList() ?? [],
            IngredientOptions = products
                .Where(x => x.Id != productId &&
                            x.InventoryBehavior == ProductInventoryBehavior.Stocked)
                .OrderBy(x => x.Name)
                .Select(x => new RecipeIngredientOptionDto
                {
                    ProductId = x.Id,
                    ProductName = x.Name,
                    StockUnit = x.StockUnit,
                    StockMeasurementUnitId = x.MeasurementUnitId ??
                        unitByCode[MapLegacyUnitCode(x.StockUnit)].Id,
                    StockUnitName = (x.MeasurementUnitId.HasValue
                        ? units.FirstOrDefault(unit => unit.Id == x.MeasurementUnitId.Value)?.Name
                        : unitByCode[MapLegacyUnitCode(x.StockUnit)].Name) ?? string.Empty,
                    StockUnitSymbol = (x.MeasurementUnitId.HasValue
                        ? units.FirstOrDefault(unit => unit.Id == x.MeasurementUnitId.Value)?.Symbol
                        : unitByCode[MapLegacyUnitCode(x.StockUnit)].Symbol) ?? string.Empty,
                    StockUnitDimension = (x.MeasurementUnitId.HasValue
                        ? units.FirstOrDefault(unit => unit.Id == x.MeasurementUnitId.Value)?.Dimension
                        : unitByCode[MapLegacyUnitCode(x.StockUnit)].Dimension) ?? MeasurementDimension.Count,
                    AllowNegativeRecipeConsumption = x.AllowNegativeRecipeConsumption
                })
                .ToList(),
            MeasurementUnits = units
        };
    }

    public async Task SaveNewVersionAsync(
        RecipeEditorDto dto,
        string createdByUserId)
    {
        if (dto.ProductId <= 0)
            throw new ArgumentException("Prepared product is required.");
        if (dto.YieldQuantity <= 0)
            throw new ArgumentException("Recipe yield must be greater than zero.");
        if (dto.Ingredients.Count == 0)
            throw new ArgumentException("Add at least one recipe ingredient.");

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var product = await _productRepository.GetByIdAsync(dto.ProductId)
                ?? throw new InvalidOperationException("Prepared product was not found.");
            var allProducts = (await _productRepository.GetAllAsync(null))
                .ToDictionary(x => x.Id);
            var activeUnits = (await _measurementUnitRepository.GetActiveAsync())
                .ToDictionary(x => x.Id);

            var ingredients = new List<RecipeIngredient>();
            foreach (var line in dto.Ingredients)
            {
                if (!allProducts.TryGetValue(line.IngredientProductId, out var ingredient))
                    throw new ArgumentException("One of the selected ingredients was not found.");
                if (ingredient.InventoryBehavior != ProductInventoryBehavior.Stocked)
                    throw new ArgumentException(
                        $"Ingredient '{ingredient.Name}' must be a stocked product.");
                if (!activeUnits.TryGetValue(line.MeasurementUnitId, out var inputUnit))
                    throw new ArgumentException("The selected recipe unit was not found or is inactive.");
                if (!ingredient.MeasurementUnitId.HasValue ||
                    !activeUnits.TryGetValue(ingredient.MeasurementUnitId.Value, out var stockUnit))
                    throw new ArgumentException($"Ingredient '{ingredient.Name}' does not have an active stock unit.");
                if (inputUnit.Dimension != stockUnit.Dimension)
                    throw new ArgumentException(
                        $"The unit selected for '{ingredient.Name}' is not compatible with its stock unit {stockUnit.Symbol}.");

                ingredients.Add(new RecipeIngredient(
                    line.IngredientProductId,
                    line.Quantity,
                    inputUnit,
                    stockUnit));
            }

            var current = await _recipeRepository.GetActiveByProductIdAsync(dto.ProductId);
            current?.Deactivate();

            var recipe = new ProductRecipe(
                dto.ProductId,
                await _recipeRepository.GetNextVersionNumberAsync(dto.ProductId),
                dto.YieldQuantity,
                createdByUserId,
                ingredients);

            product.MarkPreparedToOrder();
            await _recipeRepository.AddAsync(recipe);
        });
    }

    private static string MapLegacyUnitCode(UnitOfMeasure unit) => unit switch
    {
        UnitOfMeasure.Piece => "PC",
        UnitOfMeasure.Gram => "G",
        UnitOfMeasure.Kilogram => "KG",
        UnitOfMeasure.Milliliter => "ML",
        UnitOfMeasure.Liter => "L",
        _ => "PC"
    };
}
