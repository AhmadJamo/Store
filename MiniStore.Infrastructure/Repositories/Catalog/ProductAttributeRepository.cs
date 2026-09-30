using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class ProductAttributeRepository(AppDbContext context) : IProductAttributeRepository
{
    public Task<List<ProductAttributeDefinition>> GetDefinitionsAsync() => context.ProductAttributeDefinitions
        .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync();
    public Task<List<ProductAttributeOption>> GetOptionsAsync() => context.ProductAttributeOptions
        .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync();
    public Task<List<ProductCategoryAttribute>> GetCategoryLinksAsync() => context.ProductCategoryAttributes.ToListAsync();
    public Task<List<ProductAttributeValue>> GetValuesAsync(int productId) => context.ProductAttributeValues
        .Where(x => x.ProductId == productId).ToListAsync();
    public Task<ProductAttributeDefinition?> GetDefinitionAsync(int id) => context.ProductAttributeDefinitions.FirstOrDefaultAsync(x => x.Id == id);
    public Task<bool> CodeExistsAsync(string code) => context.ProductAttributeDefinitions.AnyAsync(x => x.Code == code);
    public Task AddDefinitionAsync(ProductAttributeDefinition definition) => context.ProductAttributeDefinitions.AddAsync(definition).AsTask();
    public Task AddOptionAsync(ProductAttributeOption option) => context.ProductAttributeOptions.AddAsync(option).AsTask();
    public Task AddCategoryLinkAsync(ProductCategoryAttribute link) => context.ProductCategoryAttributes.AddAsync(link).AsTask();
    public Task AddValueAsync(ProductAttributeValue value) => context.ProductAttributeValues.AddAsync(value).AsTask();
    public void RemoveValues(IEnumerable<ProductAttributeValue> values) => context.ProductAttributeValues.RemoveRange(values);
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
