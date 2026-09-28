namespace MiniStore.Domain.Entities;

public static class MeasurementUnitConversion
{
    public static decimal Convert(
        decimal quantity,
        MeasurementUnit from,
        MeasurementUnit to)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");
        if (from.Dimension != to.Dimension)
            throw new InvalidOperationException("Units from different measurement dimensions cannot be converted.");

        return Math.Round(
            quantity * from.FactorToBaseUnit / to.FactorToBaseUnit,
            to.DecimalPlaces,
            MidpointRounding.AwayFromZero);
    }
}
