using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class MeasurementUnitRepository(AppDbContext context) : IMeasurementUnitRepository
{
    public Task<List<MeasurementUnit>> GetAllAsync() => context.MeasurementUnits
        .OrderBy(x => x.Dimension)
        .ThenBy(x => x.FactorToBaseUnit)
        .ThenBy(x => x.Name)
        .ToListAsync();

    public Task<List<MeasurementUnit>> GetActiveAsync() => context.MeasurementUnits
        .Where(x => x.IsActive)
        .OrderBy(x => x.Dimension)
        .ThenBy(x => x.FactorToBaseUnit)
        .ThenBy(x => x.Name)
        .ToListAsync();

    public Task<MeasurementUnit?> GetByIdAsync(int id) =>
        context.MeasurementUnits.FirstOrDefaultAsync(x => x.Id == id);

    public Task<MeasurementUnit?> GetByCodeAsync(string code) =>
        context.MeasurementUnits.FirstOrDefaultAsync(x => x.Code == code);

    public async Task AddAsync(MeasurementUnit unit) =>
        await context.MeasurementUnits.AddAsync(unit);
}
