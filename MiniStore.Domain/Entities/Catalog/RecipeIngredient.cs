namespace MiniStore.Domain.Entities;

public class RecipeIngredient
{
    public int Id { get; private set; }
    public int ProductRecipeId { get; private set; }
    public int IngredientProductId { get; private set; }
    public decimal Quantity { get; private set; }
    public UnitOfMeasure Unit { get; private set; }
    public decimal StockQuantity { get; private set; }
    public UnitOfMeasure StockUnitSnapshot { get; private set; }
    public int? MeasurementUnitId { get; private set; }
    public int? StockMeasurementUnitId { get; private set; }
    public string UnitCodeSnapshot { get; private set; } = string.Empty;
    public string StockUnitCodeSnapshot { get; private set; } = string.Empty;
    public decimal UnitFactorSnapshot { get; private set; }
    public decimal StockUnitFactorSnapshot { get; private set; }

    private RecipeIngredient()
    {
    }

    public RecipeIngredient(
        int ingredientProductId,
        decimal quantity,
        UnitOfMeasure unit,
        UnitOfMeasure stockUnit)
    {
        if (ingredientProductId <= 0)
            throw new ArgumentException("Ingredient product is required.");
        if (quantity <= 0)
            throw new ArgumentException("Ingredient quantity must be greater than zero.");
        if (!Enum.IsDefined(unit))
            throw new ArgumentException("Ingredient unit is invalid.");
        if (!Enum.IsDefined(stockUnit))
            throw new ArgumentException("Ingredient stock unit is invalid.");

        var stockQuantity = Math.Round(
            UnitConversion.Convert(quantity, unit, stockUnit),
            6,
            MidpointRounding.AwayFromZero);
        if (stockQuantity <= 0)
            throw new ArgumentException(
                "Ingredient quantity is below the supported stock precision.");

        IngredientProductId = ingredientProductId;
        Quantity = quantity;
        Unit = unit;
        StockQuantity = stockQuantity;
        StockUnitSnapshot = stockUnit;
    }

    public RecipeIngredient(
        int ingredientProductId,
        decimal quantity,
        MeasurementUnit unit,
        MeasurementUnit stockUnit)
    {
        if (ingredientProductId <= 0)
            throw new ArgumentException("Ingredient product is required.");
        if (quantity <= 0)
            throw new ArgumentException("Ingredient quantity must be greater than zero.");
        if (!unit.IsActive || !stockUnit.IsActive)
            throw new ArgumentException("Recipe units must be active.");
        if (unit.Dimension != stockUnit.Dimension)
            throw new ArgumentException("Ingredient unit is not compatible with its stock unit.");

        var converted = Math.Round(
            quantity * unit.FactorToBaseUnit / stockUnit.FactorToBaseUnit,
            6,
            MidpointRounding.AwayFromZero);
        if (converted <= 0)
            throw new ArgumentException("Ingredient quantity is below the supported stock precision.");

        IngredientProductId = ingredientProductId;
        Quantity = quantity;
        MeasurementUnitId = unit.Id;
        StockMeasurementUnitId = stockUnit.Id;
        UnitCodeSnapshot = unit.Code;
        StockUnitCodeSnapshot = stockUnit.Code;
        UnitFactorSnapshot = unit.FactorToBaseUnit;
        StockUnitFactorSnapshot = stockUnit.FactorToBaseUnit;
        StockQuantity = converted;
        Unit = MapLegacyUnit(unit.Code);
        StockUnitSnapshot = MapLegacyUnit(stockUnit.Code);
    }

    private static UnitOfMeasure MapLegacyUnit(string code) => code switch
    {
        "PC" => UnitOfMeasure.Piece,
        "G" => UnitOfMeasure.Gram,
        "KG" => UnitOfMeasure.Kilogram,
        "ML" => UnitOfMeasure.Milliliter,
        "L" => UnitOfMeasure.Liter,
        _ => UnitOfMeasure.Piece
    };
}
