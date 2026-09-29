using MiniStore.Application.DTOs.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class MeasurementUnitService(
    IMeasurementUnitRepository repository,
    IUnitOfWork unitOfWork)
{
    private static readonly (string Code, string Name, string Symbol, MeasurementDimension Dimension, decimal Factor, int Decimals)[] Defaults =
    [
        ("PC", "Piece", "pc", MeasurementDimension.Count, 1m, 0),
        ("MG", "Milligram", "mg", MeasurementDimension.Mass, 0.001m, 6),
        ("G", "Gram", "g", MeasurementDimension.Mass, 1m, 3),
        ("KG", "Kilogram", "kg", MeasurementDimension.Mass, 1000m, 6),
        ("OZ", "Ounce", "oz", MeasurementDimension.Mass, 28.349523125m, 6),
        ("LB", "Pound", "lb", MeasurementDimension.Mass, 453.59237m, 6),
        ("ML", "Milliliter", "ml", MeasurementDimension.Volume, 1m, 3),
        ("L", "Liter", "L", MeasurementDimension.Volume, 1000m, 6),
        ("MM", "Millimeter", "mm", MeasurementDimension.Length, 1m, 3),
        ("CM", "Centimeter", "cm", MeasurementDimension.Length, 10m, 3),
        ("M", "Meter", "m", MeasurementDimension.Length, 1000m, 6),
        ("IN", "Inch", "in", MeasurementDimension.Length, 25.4m, 6)
    ];

    public async Task<MeasurementUnitsPageDto> GetPageAsync(CreateMeasurementUnitDto? input = null)
    {
        await EnsureDefaultsAsync();
        return new MeasurementUnitsPageDto
        {
            Units = (await repository.GetAllAsync()).Select(Map).ToList(),
            NewUnit = input ?? new CreateMeasurementUnitDto()
        };
    }

    public async Task<List<MeasurementUnitDto>> GetActiveAsync()
    {
        await EnsureDefaultsAsync();
        return (await repository.GetActiveAsync()).Select(Map).ToList();
    }

    public async Task CreateAsync(CreateMeasurementUnitDto dto)
    {
        var code = dto.Code.Trim().ToUpperInvariant();
        if (await repository.GetByCodeAsync(code) is not null)
            throw new InvalidOperationException("A measurement unit with this code already exists.");

        await unitOfWork.ExecuteInTransactionAsync(async () =>
            await repository.AddAsync(new MeasurementUnit(
                code,
                dto.Name,
                dto.Symbol,
                dto.Dimension,
                dto.FactorToBaseUnit,
                dto.DecimalPlaces)));
    }

    public async Task DeactivateAsync(int id)
    {
        var unit = await repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Measurement unit was not found.");
        unit.Deactivate();
        await unitOfWork.ExecuteInTransactionAsync(() => Task.CompletedTask);
    }

    public async Task<decimal> ConvertAsync(decimal quantity, int fromUnitId, int toUnitId)
    {
        var from = await repository.GetByIdAsync(fromUnitId)
            ?? throw new InvalidOperationException("Source measurement unit was not found.");
        var to = await repository.GetByIdAsync(toUnitId)
            ?? throw new InvalidOperationException("Destination measurement unit was not found.");
        return MeasurementUnitConversion.Convert(quantity, from, to);
    }

    private async Task EnsureDefaultsAsync()
    {
        var existingCodes = (await repository.GetAllAsync())
            .Select(x => x.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = Defaults.Where(x => !existingCodes.Contains(x.Code)).ToList();
        if (missing.Count == 0)
            return;

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            foreach (var item in missing)
                await repository.AddAsync(new MeasurementUnit(
                    item.Code,
                    item.Name,
                    item.Symbol,
                    item.Dimension,
                    item.Factor,
                    item.Decimals,
                    isSystem: true));
        });
    }

    private static MeasurementUnitDto Map(MeasurementUnit unit) => new()
    {
        Id = unit.Id,
        Code = unit.Code,
        Name = unit.Name,
        Symbol = unit.Symbol,
        Dimension = unit.Dimension,
        FactorToBaseUnit = unit.FactorToBaseUnit,
        DecimalPlaces = unit.DecimalPlaces,
        IsSystem = unit.IsSystem,
        IsActive = unit.IsActive
    };
}
