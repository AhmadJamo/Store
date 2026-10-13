using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IPurchaseDocumentReversalRepository
{
    Task<List<PurchaseDocumentReversal>> GetAllAsync();
    Task<PurchaseDocumentReversal?> GetByIdAsync(int id);
    Task<PurchaseDocumentReversal?> GetByRequestKeyAsync(string key);
    Task<PurchaseDocumentReversal?> GetByOriginalAsync(PurchaseDocumentType type, int originalDocumentId);
    Task AddAsync(PurchaseDocumentReversal value);
}
