using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Settings;

public sealed class MeasurementUnitsPageDto
{
    public List<MeasurementUnitDto> Units { get; init; } = [];
    public CreateMeasurementUnitDto NewUnit { get; init; } = new();
}

public sealed class MeasurementUnitDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public MeasurementDimension Dimension { get; init; }
    public decimal FactorToBaseUnit { get; init; }
    public int DecimalPlaces { get; init; }
    public bool IsSystem { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CreateMeasurementUnitDto
{
    [Required, StringLength(20)]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Symbol { get; set; } = string.Empty;

    public MeasurementDimension Dimension { get; set; } = MeasurementDimension.Mass;

    [Range(typeof(decimal), "0.000000000001", "999999999999.999999999999",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true)]
    public decimal FactorToBaseUnit { get; set; } = 1m;

    [Range(0, 6)]
    public int DecimalPlaces { get; set; } = 3;
}
