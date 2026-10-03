using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IStockMovementRepository
{
    Task<List<StockMovement>> GetAllAsync();
    Task<StockMovement?> GetByIdempotencyKeyAsync(string idempotencyKey);
    Task AddAsync(StockMovement movement);
}
