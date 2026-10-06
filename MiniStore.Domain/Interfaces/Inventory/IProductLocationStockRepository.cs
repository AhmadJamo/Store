using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IProductLocationStockRepository
{
    Task<List<ProductLocationStock>> GetAllAsync();
    Task<List<ProductLocationStock>> GetWarehouseProductAsync(int productId, int warehouseId);
    Task<ProductLocationStock?> GetAsync(int productId, int storageLocationId);
    Task AddAsync(ProductLocationStock stock);
    Task RemoveAllAsync(int productId, int warehouseId);
}
