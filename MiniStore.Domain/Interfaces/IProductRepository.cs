using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);

    Task<Product?> GetByBarcodeAsync(string barcode);

    Task<List<Product>> GetAllAsync(string? search);

    Task AddAsync(Product product);

    Task DeleteAsync(Product product);

    Task SaveChangesAsync();
}