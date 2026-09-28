using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class ProductRecipeRepository : IProductRecipeRepository
{
    private readonly AppDbContext _context;

    public ProductRecipeRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<ProductRecipe?> GetActiveByProductIdAsync(int productId) =>
        _context.ProductRecipes
            .Include(x => x.Ingredients)
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.IsActive);

    public Task<List<ProductRecipe>> GetActiveAsync() =>
        _context.ProductRecipes
            .Include(x => x.Ingredients)
            .Where(x => x.IsActive)
            .ToListAsync();

    public async Task<int> GetNextVersionNumberAsync(int productId)
    {
        var lastVersion = await _context.ProductRecipes
            .Where(x => x.ProductId == productId)
            .MaxAsync(x => (int?)x.VersionNumber);
        return (lastVersion ?? 0) + 1;
    }

    public async Task AddAsync(ProductRecipe recipe)
    {
        await _context.ProductRecipes.AddAsync(recipe);
    }
}
