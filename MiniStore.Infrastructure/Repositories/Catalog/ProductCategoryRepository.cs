using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class ProductCategoryRepository(AppDbContext context)
    : IProductCategoryRepository
{
    public Task<List<ProductCategory>> GetAllAsync() => context.ProductCategories
        .OrderBy(category => category.Name)
        .ToListAsync();

    public Task<ProductCategory?> GetByIdAsync(int id) => context.ProductCategories
        .FirstOrDefaultAsync(category => category.Id == id);

    public Task<ProductCategory?> GetByCodeAsync(string code) => context.ProductCategories
        .FirstOrDefaultAsync(category => category.Code == code);

    public Task<ProductCategory?> GetByNameAsync(string name) => context.ProductCategories
        .FirstOrDefaultAsync(category => category.Name == name);

    public Task AddAsync(ProductCategory category) =>
        context.ProductCategories.AddAsync(category).AsTask();

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
