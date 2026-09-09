using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISaleRepository
{
    Task<List<Sale>> GetAllAsync();

    Task<Sale?> GetByIdAsync(int id);

    Task AddAsync(Sale sale);
}