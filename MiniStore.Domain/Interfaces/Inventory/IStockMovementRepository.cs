using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IStockMovementRepository
{
    Task<List<StockMovement>> GetAllAsync();
    Task<StockMovement?> GetByIdempotencyKeyAsync(string idempotencyKey);
    Task<List<StockMovement>> GetBySourceAsync(string sourceDocumentType, int sourceDocumentId);
    Task AddAsync(StockMovement movement);
}
