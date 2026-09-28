namespace MiniStore.Domain.Entities;

public class MeasurementUnit
{
    public int Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Symbol { get; private set; }
    public MeasurementDimension Dimension { get; private set; }
    public decimal FactorToBaseUnit { get; private set; }
    public int DecimalPlaces { get; private set; }
    public bool IsSystem { get; private set; }
    public bool IsActive { get; private set; }

    private MeasurementUnit()
    {
        Code = string.Empty;
        Name = string.Empty;
        Symbol = string.Empty;
    }

    public MeasurementUnit(
        string code,
        string name,
        string symbol,
        MeasurementDimension dimension,
        decimal factorToBaseUnit,
        int decimalPlaces,
        bool isSystem = false)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Trim().Length > 20)
            throw new ArgumentException("Unit code is required and cannot exceed 20 characters.");
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            throw new ArgumentException("Unit name is required and cannot exceed 100 characters.");
        if (string.IsNullOrWhiteSpace(symbol) || symbol.Trim().Length > 20)
            throw new ArgumentException("Unit symbol is required and cannot exceed 20 characters.");
        if (!Enum.IsDefined(dimension))
            throw new ArgumentException("Measurement dimension is invalid.");
        if (factorToBaseUnit <= 0)
            throw new ArgumentException("Conversion factor must be greater than zero.");
        if (decimalPlaces is < 0 or > 6)
            throw new ArgumentException("Unit decimal places must be between zero and six.");

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Symbol = symbol.Trim();
        Dimension = dimension;
        FactorToBaseUnit = factorToBaseUnit;
        DecimalPlaces = decimalPlaces;
        IsSystem = isSystem;
        IsActive = true;
    }

    public void Deactivate()
    {
        if (IsSystem)
            throw new InvalidOperationException("Built-in measurement units cannot be deactivated.");
        IsActive = false;
    }
}
