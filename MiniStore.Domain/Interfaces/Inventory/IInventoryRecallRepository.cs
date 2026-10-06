using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IInventoryRecallRepository
{
    Task<List<InventoryRecall>> GetAllAsync();
    Task<List<InventoryRecallCommunication>> GetCommunicationsAsync();
    Task<InventoryRecall?> GetByIdForUpdateAsync(long id);
    Task<bool> HasActiveAsync(int productId,string identifier);
    Task<bool> ReferenceExistsAsync(string reference);
    Task AddAsync(InventoryRecall recall);
    Task AddCommunicationAsync(InventoryRecallCommunication communication);
}
