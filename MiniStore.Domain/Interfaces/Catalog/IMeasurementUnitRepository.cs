using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IMeasurementUnitRepository
{
    Task<List<MeasurementUnit>> GetAllAsync();
    Task<List<MeasurementUnit>> GetActiveAsync();
    Task<MeasurementUnit?> GetByIdAsync(int id);
    Task<MeasurementUnit?> GetByCodeAsync(string code);
    Task AddAsync(MeasurementUnit unit);
}
