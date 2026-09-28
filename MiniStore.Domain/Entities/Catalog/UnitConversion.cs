namespace MiniStore.Domain.Entities;

public static class UnitConversion
{
    public static decimal Convert(
        decimal quantity,
        UnitOfMeasure from,
        UnitOfMeasure to)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");
        if (!Enum.IsDefined(from) || !Enum.IsDefined(to))
            throw new ArgumentException("Unit of measure is invalid.");
        if (from == to)
            return quantity;

        var fromDimension = GetDimension(from);
        if (fromDimension != GetDimension(to))
            throw new InvalidOperationException(
                $"Cannot convert {from} to {to} because they measure different dimensions.");

        return quantity * GetBaseFactor(from) / GetBaseFactor(to);
    }

    public static bool AreCompatible(UnitOfMeasure first, UnitOfMeasure second) =>
        GetDimension(first) == GetDimension(second);

    private static string GetDimension(UnitOfMeasure unit) => unit switch
    {
        UnitOfMeasure.Piece => "count",
        UnitOfMeasure.Gram or UnitOfMeasure.Kilogram => "mass",
        UnitOfMeasure.Milliliter or UnitOfMeasure.Liter => "volume",
        _ => throw new ArgumentException("Unit of measure is invalid.")
    };

    private static decimal GetBaseFactor(UnitOfMeasure unit) => unit switch
    {
        UnitOfMeasure.Piece => 1m,
        UnitOfMeasure.Gram => 1m,
        UnitOfMeasure.Kilogram => 1000m,
        UnitOfMeasure.Milliliter => 1m,
        UnitOfMeasure.Liter => 1000m,
        _ => throw new ArgumentException("Unit of measure is invalid.")
    };
}
