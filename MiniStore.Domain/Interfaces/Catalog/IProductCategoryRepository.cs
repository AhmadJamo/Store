using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IProductCategoryRepository
{
    Task<List<ProductCategory>> GetAllAsync();
    Task<ProductCategory?> GetByIdAsync(int id);
    Task<ProductCategory?> GetByCodeAsync(string code);
    Task<ProductCategory?> GetByNameAsync(string name);
    Task AddAsync(ProductCategory category);
    Task SaveChangesAsync();
}
