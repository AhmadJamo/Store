using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IProductTemplateRepository
{
    Task<List<ProductTemplate>> GetAllAsync();
    Task<ProductTemplate?> GetByIdAsync(int id);
    Task<bool> CodeExistsAsync(string code);
    Task<bool> VariantSignatureExistsAsync(int templateId, string signature, int? excludedProductId = null);
    Task AddAsync(ProductTemplate template);
    Task SaveChangesAsync();
}
