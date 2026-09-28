using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;
using MiniStore.Application.DTOs.Settings;

namespace MiniStore.Application.DTOs.Recipes;

public class RecipeListItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public ProductInventoryBehavior InventoryBehavior { get; set; }
    public int? ActiveVersion { get; set; }
    public decimal? YieldQuantity { get; set; }
    public int IngredientCount { get; set; }
}

public class RecipeEditorDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int? ActiveVersion { get; set; }

    [Range(typeof(decimal), "0.000001", "999999999999.999999",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true)]
    public decimal YieldQuantity { get; set; } = 1m;

    public List<RecipeIngredientInputDto> Ingredients { get; set; } = [];
    public List<RecipeIngredientOptionDto> IngredientOptions { get; set; } = [];
    public List<MeasurementUnitDto> MeasurementUnits { get; set; } = [];
}

public class RecipeIngredientInputDto
{
    [Range(1, int.MaxValue)]
    public int IngredientProductId { get; set; }

    [Range(typeof(decimal), "0.000001", "999999999999.999999",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true)]
    public decimal Quantity { get; set; }

    [Range(1, int.MaxValue)]
    public int MeasurementUnitId { get; set; }
}

public class RecipeIngredientOptionDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public UnitOfMeasure StockUnit { get; set; }
    public int StockMeasurementUnitId { get; set; }
    public string StockUnitName { get; set; } = string.Empty;
    public string StockUnitSymbol { get; set; } = string.Empty;
    public MeasurementDimension StockUnitDimension { get; set; }
    public bool AllowNegativeRecipeConsumption { get; set; }
}
