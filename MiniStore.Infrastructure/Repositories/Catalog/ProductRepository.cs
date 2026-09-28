using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode)
    {
        return await _context.Products
            .FirstOrDefaultAsync(x => x.Barcode == barcode);
    }

    public async Task<ProductSearchPage> SearchAsync(ProductSearchCriteria criteria)
    {
        var query = _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(criteria.Search))
        {
            var search = criteria.Search.Trim();
            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.ProductCode.Contains(search) ||
                (x.Barcode != null && x.Barcode.Contains(search)));
        }

        if (criteria.ProductType.HasValue)
            query = query.Where(x => x.ProductType == criteria.ProductType.Value);
        if (criteria.IsSellableInPos.HasValue)
            query = query.Where(x => x.IsSellableInPos == criteria.IsSellableInPos.Value);
        if (criteria.IsSellableInSales.HasValue)
            query = query.Where(x => x.IsSellableInSales == criteria.IsSellableInSales.Value);
        if (criteria.IsActive.HasValue)
            query = query.Where(x => x.IsActive == criteria.IsActive.Value);

        query = (criteria.SortBy.ToLowerInvariant(), criteria.SortDescending) switch
        {
            ("code", false) => query.OrderBy(x => x.ProductCode),
            ("code", true) => query.OrderByDescending(x => x.ProductCode),
            ("type", false) => query.OrderBy(x => x.ProductType).ThenBy(x => x.Name),
            ("type", true) => query.OrderByDescending(x => x.ProductType).ThenBy(x => x.Name),
            ("price", false) => query.OrderBy(x => x.SalePrice).ThenBy(x => x.Name),
            ("price", true) => query.OrderByDescending(x => x.SalePrice).ThenBy(x => x.Name),
            ("name", true) => query.OrderByDescending(x => x.Name),
            _ => query.OrderBy(x => x.Name)
        };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();
        return new ProductSearchPage(items, totalCount);
    }

    public async Task<List<Product>> GetAllAsync(string? search)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.ProductCode.Contains(search) ||
                (x.Barcode != null && x.Barcode.Contains(search)));
        }

        return await query.ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);

        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
