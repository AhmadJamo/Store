using MiniStore.Application.DTOs.Products;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class ProductAttributeService(
    IProductAttributeRepository repository,
    IProductCategoryRepository categories,
    IProductRepository products,
    ProductTemplateService productTemplates,
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

    public async Task<ProductAttributeValuesPageDto> GetValuesPageAsync(int productId)
    {
        var product = await products.GetByIdAsync(productId)
            ?? throw new InvalidOperationException("Product not found.");
        var definitions = (await repository.GetDefinitionsAsync()).Where(x => x.IsActive).ToList();
        var links = await repository.GetCategoryLinksAsync();
        var options = await repository.GetOptionsAsync();
        var values = await repository.GetValuesAsync(productId);
        HashSet<int> linkedDefinitionIds = product.ProductCategoryId.HasValue
            ? links.Where(x => x.ProductCategoryId == product.ProductCategoryId.Value)
                .Select(x => x.ProductAttributeDefinitionId).ToHashSet()
            : [];
        var definitionsWithLinks = links.Select(x => x.ProductAttributeDefinitionId).ToHashSet();
        var applicable = definitions.Where(x => !definitionsWithLinks.Contains(x.Id) || linkedDefinitionIds.Contains(x.Id))
            .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToList();
        var category = product.ProductCategoryId.HasValue
            ? await categories.GetByIdAsync(product.ProductCategoryId.Value)
            : null;

        return new ProductAttributeValuesPageDto
        {
            ProductId = product.Id,
            ProductName = product.Name,
            ProductCode = product.ProductCode,
            CategoryName = category?.Name,
            Fields = applicable.Select(definition =>
            {
                var existing = values.FirstOrDefault(x => x.ProductAttributeDefinitionId == definition.Id);
                return new ProductAttributeValueFieldDto
                {
                    DefinitionId = definition.Id,
                    Name = definition.Name,
                    Code = definition.Code,
                    DataType = definition.DataType,
                    IsRequired = definition.IsRequired,
                    IsVariantDefining = definition.IsVariantDefining,
                    Value = FormatValue(existing),
                    Options = options.Where(x => x.ProductAttributeDefinitionId == definition.Id && x.IsActive)
                        .Select(x => new ProductAttributeOptionFieldDto { Id = x.Id, Name = x.Name, Code = x.Code }).ToList()
                };
            }).ToList()
        };
    }

    public async Task SaveValuesAsync(int productId, IReadOnlyCollection<ProductAttributeValueInputDto> inputs)
    {
        var page = await GetValuesPageAsync(productId);
        var fields = page.Fields.ToDictionary(x => x.DefinitionId);
        var supplied = inputs.GroupBy(x => x.DefinitionId).ToDictionary(x => x.Key, x => x.Last().Value?.Trim());
        var newValues = new List<ProductAttributeValue>();

        foreach (var field in page.Fields)
        {
            supplied.TryGetValue(field.DefinitionId, out var rawValue);
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                if (field.IsRequired) throw new ArgumentException("A required product attribute is missing.");
                continue;
            }

            newValues.Add(ParseValue(productId, field, rawValue));
        }
        if (supplied.Keys.Any(id => !fields.ContainsKey(id)))
            throw new ArgumentException("One or more product attributes are not applicable to this product category.");

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var product = await products.GetByIdAsync(productId)
                ?? throw new InvalidOperationException("Product not found.");
            await productTemplates.RefreshAssignedVariantAsync(product, newValues);
            repository.RemoveValues(await repository.GetValuesAsync(productId));
            foreach (var value in newValues) await repository.AddValueAsync(value);
        });
    }

    private static ProductAttributeValue ParseValue(int productId, ProductAttributeValueFieldDto field, string value)
    {
        return field.DataType switch
        {
            ProductAttributeDataType.Text => new(productId, field.DefinitionId, value, null, null, null),
            ProductAttributeDataType.Number when decimal.TryParse(value,
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture, out var number)
                => new(productId, field.DefinitionId, null, number, null, null),
            ProductAttributeDataType.Boolean when bool.TryParse(value, out var boolean)
                => new(productId, field.DefinitionId, null, null, boolean, null),
            ProductAttributeDataType.Selection when int.TryParse(value, out var optionId) &&
                field.Options.Any(x => x.Id == optionId)
                => new(productId, field.DefinitionId, null, null, null, optionId),
            ProductAttributeDataType.Number => throw new ArgumentException("A number attribute contains an invalid value."),
            ProductAttributeDataType.Boolean => throw new ArgumentException("A yes/no attribute contains an invalid value."),
            ProductAttributeDataType.Selection => throw new ArgumentException("A selection attribute contains an invalid option."),
            _ => throw new ArgumentException("Product attribute data type is invalid.")
        };
    }

    private static string? FormatValue(ProductAttributeValue? value)
    {
        if (value is null) return null;
        if (value.TextValue is not null) return value.TextValue;
        if (value.NumberValue.HasValue) return value.NumberValue.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (value.BooleanValue.HasValue) return value.BooleanValue.Value.ToString().ToLowerInvariant();
        return value.ProductAttributeOptionId?.ToString(System.Globalization.CultureInfo.InvariantCulture);
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
