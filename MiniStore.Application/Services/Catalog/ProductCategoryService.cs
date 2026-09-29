using MiniStore.Application.DTOs.Products;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class ProductCategoryService(IProductCategoryRepository repository)
{
    public async Task<List<ProductCategoryDto>> GetAllAsync(bool activeOnly = false) =>
        (await repository.GetAllAsync())
            .Where(category => !activeOnly || category.IsActive)
            .Select(Map)
            .ToList();

    public async Task CreateAsync(CreateProductCategoryDto dto)
    {
        var code = dto.Code.Trim().ToUpperInvariant();
        if (await repository.GetByCodeAsync(code) is not null)
            throw new InvalidOperationException("A product category with this code already exists.");
        if (await repository.GetByNameAsync(dto.Name.Trim()) is not null)
            throw new InvalidOperationException("A product category with this name already exists.");
        await repository.AddAsync(new ProductCategory(dto.Name, code));
        await repository.SaveChangesAsync();
    }

    public async Task SetActiveAsync(int id, bool isActive)
    {
        var category = await repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Product category was not found.");
        category.SetActive(isActive);
        await repository.SaveChangesAsync();
    }

    private static ProductCategoryDto Map(ProductCategory category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Code = category.Code,
        IsActive = category.IsActive
    };
}
