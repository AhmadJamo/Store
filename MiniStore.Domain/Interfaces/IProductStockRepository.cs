using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IProductStockRepository
{
    Task<ProductStock?> GetByIdAsync(int id);

    Task<ProductStock?> GetByProductAndWarehouseAsync(
        int productId,
        int warehouseId);

    Task<List<ProductStock>> GetAllAsync();

    Task AddAsync(ProductStock productStock);

    Task DeleteAsync(ProductStock productStock);

    Task SaveChangesAsync();
}