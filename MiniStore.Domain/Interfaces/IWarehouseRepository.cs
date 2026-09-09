using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(int id);

    Task<List<Warehouse>> GetAllAsync();

    Task AddAsync(Warehouse warehouse);

    Task DeleteAsync(Warehouse warehouse);

    Task SaveChangesAsync();
}