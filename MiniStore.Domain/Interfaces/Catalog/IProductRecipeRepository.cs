using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IProductRecipeRepository
{
    Task<ProductRecipe?> GetActiveByProductIdAsync(int productId);
    Task<List<ProductRecipe>> GetActiveAsync();
    Task<int> GetNextVersionNumberAsync(int productId);
    Task AddAsync(ProductRecipe recipe);
}
