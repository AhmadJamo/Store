using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISalesReturnRepository
{
    Task<List<SalesReturn>> GetAllAsync();
    Task<SalesReturn?> GetByIdAsync(int id);
    Task<List<SalesReturn>> GetBySaleIdAsync(int saleId);
    Task AddAsync(SalesReturn salesReturn);
}
