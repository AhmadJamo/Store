using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IProductAttributeRepository
{
    Task<List<ProductAttributeDefinition>> GetDefinitionsAsync();
    Task<List<ProductAttributeOption>> GetOptionsAsync();
    Task<List<ProductCategoryAttribute>> GetCategoryLinksAsync();
    Task<List<ProductAttributeValue>> GetValuesAsync(int productId);
    Task<ProductAttributeDefinition?> GetDefinitionAsync(int id);
    Task<bool> CodeExistsAsync(string code);
    Task AddDefinitionAsync(ProductAttributeDefinition definition);
    Task AddOptionAsync(ProductAttributeOption option);
    Task AddCategoryLinkAsync(ProductCategoryAttribute link);
    Task AddValueAsync(ProductAttributeValue value);
    void RemoveValues(IEnumerable<ProductAttributeValue> values);
    Task SaveChangesAsync();
}
