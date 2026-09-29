using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);

    Task<Product?> GetByBarcodeAsync(string barcode);

    Task<ProductSearchPage> SearchAsync(ProductSearchCriteria criteria);

    Task<List<Product>> GetAllAsync(string? search);

    Task<bool> HasNonZeroStockAsync(int productId);

    Task AddAsync(Product product);

    Task DeleteAsync(Product product);

    Task SaveChangesAsync();
}

public sealed record ProductSearchCriteria(
    string? Search,
    ProductType? ProductType,
    int? ProductCategoryId,
    ProductTrackingPolicy? TrackingPolicy,
    bool? IsSellableInPos,
    bool? IsSellableInSales,
    bool? IsActive,
    string SortBy,
    bool SortDescending,
    int Page,
    int PageSize);

public sealed record ProductSearchPage(List<Product> Items, int TotalCount);
