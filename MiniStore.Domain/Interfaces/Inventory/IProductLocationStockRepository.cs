using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IProductLocationStockRepository
{
    Task<List<ProductLocationStock>> GetAllAsync();
    Task<ProductLocationStock?> GetAsync(int productId, int storageLocationId);
    Task AddAsync(ProductLocationStock stock);
    Task RemoveAllAsync(int productId, int warehouseId);
}
