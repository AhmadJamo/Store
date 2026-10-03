using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class ProductTemplateRepository(AppDbContext context) : IProductTemplateRepository
{
    public Task<List<ProductTemplate>> GetAllAsync() => context.ProductTemplates.OrderBy(x => x.Name).ToListAsync();
    public Task<ProductTemplate?> GetByIdAsync(int id) => context.ProductTemplates.FirstOrDefaultAsync(x => x.Id == id);
    public Task<bool> CodeExistsAsync(string code) => context.ProductTemplates.AnyAsync(x => x.Code == code);
    public Task<bool> VariantSignatureExistsAsync(int templateId, string signature, int? excludedProductId = null) =>
        context.Products.AnyAsync(x => x.ProductTemplateId == templateId && x.VariantSignature == signature &&
            (!excludedProductId.HasValue || x.Id != excludedProductId.Value));
    public Task AddAsync(ProductTemplate template) => context.ProductTemplates.AddAsync(template).AsTask();
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
