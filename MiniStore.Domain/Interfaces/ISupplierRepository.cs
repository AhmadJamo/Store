using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(int id);

    Task<List<Supplier>> GetAllAsync();

    Task AddAsync(Supplier supplier);

    Task DeleteAsync(Supplier supplier);

    Task SaveChangesAsync();
}