using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IPurchaseRepository
{
    Task<Purchase?> GetByIdAsync(int id);

    Task<List<Purchase>> GetAllAsync();

    Task AddAsync(Purchase purchase);

    Task SaveChangesAsync();
}