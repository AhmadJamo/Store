using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface ITaxRateRepository { Task<List<TaxRate>> GetAllAsync(); Task AddAsync(TaxRate rate); Task SaveChangesAsync(); }
