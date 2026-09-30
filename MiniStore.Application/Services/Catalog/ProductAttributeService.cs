using MiniStore.Application.DTOs.Products;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class ProductAttributeService(
    IProductAttributeRepository repository,
    IProductCategoryRepository categories,
    IUnitOfWork unitOfWork)
{
    public async Task<ProductAttributePageDto> GetPageAsync(CreateProductAttributeDto? form = null)
    {
        var definitions = await repository.GetDefinitionsAsync();
        var options = await repository.GetOptionsAsync();
        var links = await repository.GetCategoryLinksAsync();
        var categoryRows = await categories.GetAllAsync();
        var names = categoryRows.ToDictionary(x => x.Id, x => x.Name);
        return new ProductAttributePageDto
        {
            Form = form ?? new(),
            Categories = categoryRows.Select(x => new ProductCategoryDto
            { Id = x.Id, Name = x.Name, Code = x.Code, IsActive = x.IsActive }).ToList(),
            Definitions = definitions.Select(definition => new ProductAttributeDefinitionDto
            {
                Id = definition.Id,
                Name = definition.Name,
                Code = definition.Code,
                DataType = definition.DataType,
                IsRequired = definition.IsRequired,
                IsVariantDefining = definition.IsVariantDefining,
                IsActive = definition.IsActive,
                DisplayOrder = definition.DisplayOrder,
                Categories = links.Where(x => x.ProductAttributeDefinitionId == definition.Id)
                    .Select(x => names.GetValueOrDefault(x.ProductCategoryId, "Unknown")).ToList(),
                Options = options.Where(x => x.ProductAttributeDefinitionId == definition.Id)
                    .Select(x => x.Name).ToList()
            }).ToList()
        };
    }

    public async Task CreateAsync(CreateProductAttributeDto dto)
    {
        var normalizedCode = dto.Code.Trim().ToUpperInvariant();
        if (await repository.CodeExistsAsync(normalizedCode))
            throw new InvalidOperationException("An attribute with this code already exists.");
        var distinctCategoryIds = dto.ProductCategoryIds.Where(x => x > 0).Distinct().ToList();
        foreach (var categoryId in distinctCategoryIds)
        {
            var category = await categories.GetByIdAsync(categoryId)
                ?? throw new ArgumentException("Selected product category was not found.");
            if (!category.IsActive) throw new ArgumentException("Selected product category is inactive.");
        }

        var parsedOptions = ParseOptions(dto.OptionsText);
        if (dto.DataType == ProductAttributeDataType.Selection && parsedOptions.Count == 0)
            throw new ArgumentException("A selection attribute requires at least one option.");
        if (dto.DataType != ProductAttributeDataType.Selection && parsedOptions.Count > 0)
            throw new ArgumentException("Options can only be added to a selection attribute.");

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var definition = new ProductAttributeDefinition(dto.Name, normalizedCode, dto.DataType,
                dto.IsRequired, dto.IsVariantDefining, dto.DisplayOrder);
            await repository.AddDefinitionAsync(definition);
            await repository.SaveChangesAsync();
            foreach (var categoryId in distinctCategoryIds)
                await repository.AddCategoryLinkAsync(new ProductCategoryAttribute(categoryId, definition.Id));
            for (var index = 0; index < parsedOptions.Count; index++)
                await repository.AddOptionAsync(new ProductAttributeOption(
                    definition.Id, parsedOptions[index].Name, parsedOptions[index].Code, index));
        });
    }

    public async Task SetActiveAsync(int id, bool isActive)
    {
        var definition = await repository.GetDefinitionAsync(id)
            ?? throw new InvalidOperationException("Product attribute was not found.");
        definition.SetActive(isActive);
        await repository.SaveChangesAsync();
    }

    private static List<(string Name, string Code)> ParseOptions(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return [];
        var result = new List<(string, string)>();
        var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var rawLine in text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = rawLine.Split('|', 2, StringSplitOptions.TrimEntries);
            var name = parts[0];
            var code = (parts.Length == 2 ? parts[1] : name).Trim().ToUpperInvariant().Replace(' ', '_');
            if (!codes.Add(code)) throw new ArgumentException("Attribute option codes must be unique.");
            result.Add((name, code));
        }
        return result;
    }
}
