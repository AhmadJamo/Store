using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using MiniStore.Application.DTOs.Products;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class ProductTemplateService(
    IProductTemplateRepository templates,
    IProductRepository products,
    IProductCategoryRepository categories,
    IProductAttributeRepository attributes,
    IUnitOfWork unitOfWork)
{
    public const int MaximumGeneratedVariants = 50;
    public async Task<ProductTemplatePageDto> GetPageAsync()
    {
        var templateRows = await templates.GetAllAsync();
        var productRows = await products.GetAllAsync(null);
        var categoryRows = await categories.GetAllAsync();
        var categoryNames = categoryRows.ToDictionary(x => x.Id, x => x.Name);
        var definitions = await attributes.GetDefinitionsAsync();
        var options = await attributes.GetOptionsAsync();
        var links = await attributes.GetCategoryLinksAsync();
        return new ProductTemplatePageDto
        {
            Categories = categoryRows.Select(x => new ProductCategoryDto
                { Id = x.Id, Name = x.Name, Code = x.Code, IsActive = x.IsActive }).ToList(),
            Products = productRows.OrderBy(x => x.Name).Select(x => new ProductTemplateProductOptionDto
            {
                Id = x.Id, Name = x.Name, ProductCode = x.ProductCode,
                ProductCategoryId = x.ProductCategoryId, ProductTemplateId = x.ProductTemplateId
            }).ToList(),
            Templates = templateRows.Select(template => new ProductTemplateRowDto
            {
                Id = template.Id,
                Name = template.Name,
                Code = template.Code,
                CategoryName = categoryNames.GetValueOrDefault(template.ProductCategoryId, "Unknown"),
                IsActive = template.IsActive,
                Variants = productRows.Where(x => x.ProductTemplateId == template.Id)
                    .OrderBy(x => x.VariantLabel).Select(x => new ProductVariantRowDto
                    {
                        ProductId = x.Id, ProductCode = x.ProductCode,
                        ProductName = x.Name, VariantLabel = x.VariantLabel
                    }).ToList(),
                GenerationAttributes = GetGenerationDefinitions(template.ProductCategoryId, definitions, links)
                    .Select(definition => new VariantGenerationAttributeDto
                    {
                        DefinitionId = definition.Id,
                        Name = definition.Name,
                        Options = options.Where(option => option.ProductAttributeDefinitionId == definition.Id && option.IsActive)
                            .Select(option => new ProductAttributeOptionFieldDto { Id = option.Id, Name = option.Name, Code = option.Code })
                            .ToList()
                    }).ToList()
            }).ToList()
        };
    }

    public async Task<ProductTemplatePageDto> PreviewGenerationAsync(VariantGenerationInputDto dto)
    {
        var page = await GetPageAsync();
        var plan = await BuildGenerationPlanAsync(dto);
        page.Preview = new VariantGenerationPreviewDto
        {
            ProductTemplateId = dto.ProductTemplateId,
            SourceProductId = dto.SourceProductId,
            OptionIds = dto.OptionIds.Distinct().ToList(),
            Rows = plan.Select(x => new VariantGenerationPreviewRowDto
            {
                Label = x.Label,
                AlreadyExists = x.Exists
            }).ToList(),
            ExistingCount = plan.Count(x => x.Exists)
        };
        return page;
    }

    public async Task<int> GenerateAsync(VariantGenerationInputDto dto)
    {
        var plan = await BuildGenerationPlanAsync(dto);
        var source = await products.GetByIdAsync(dto.SourceProductId)
            ?? throw new InvalidOperationException("Source product was not found.");
        var template = await templates.GetByIdAsync(dto.ProductTemplateId)
            ?? throw new InvalidOperationException("Product template was not found.");
        if (source.ProductType == ProductType.PreparedToOrder)
            throw new InvalidOperationException("Prepared-to-order variants must be created manually because recipes are not cloned.");
        var sourceValues = await attributes.GetValuesAsync(source.Id);
        var definitions = await attributes.GetDefinitionsAsync();
        var variantDefinitionIds = definitions.Where(x => x.IsActive && x.IsVariantDefining).Select(x => x.Id).ToHashSet();
        var created = 0;

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            foreach (var combination in plan.Where(x => !x.Exists))
            {
                var name = $"{template.Name} - {combination.ShortLabel}";
                if (name.Length > 200) name = name[..200];
                var product = new Product(name, null, source.PurchasePrice, source.SalePrice, source.WholesalePrice,
                    source.InventoryBehavior, source.StockUnit, source.AllowNegativeRecipeConsumption, source.ProductType,
                    source.IsSellableInPos, source.IsSellableInSales, source.IsActive, source.MeasurementUnitId);
                product.ConfigureLogistics(source.ProductCategoryId, source.NetWeight, source.GrossWeight,
                    source.WeightMeasurementUnitId, source.Length, source.Width, source.Height,
                    source.DimensionMeasurementUnitId, source.TrackingPolicy, source.HandlingRequirements);
                product.AssignTemplate(dto.ProductTemplateId, combination.Signature, combination.Label);
                await products.AddAsync(product);
                await products.SaveChangesAsync();

                foreach (var value in sourceValues.Where(x => !variantDefinitionIds.Contains(x.ProductAttributeDefinitionId)))
                    await attributes.AddValueAsync(CloneValue(product.Id, value));
                foreach (var option in combination.Options)
                    await attributes.AddValueAsync(new ProductAttributeValue(product.Id, option.ProductAttributeDefinitionId, null, null, null, option.Id));
                created++;
            }
            await attributes.SaveChangesAsync();
        });
        return created;
    }

    public async Task CreateAsync(CreateProductTemplateDto dto)
    {
        var category = await categories.GetByIdAsync(dto.ProductCategoryId)
            ?? throw new ArgumentException("Selected product category was not found.");
        if (!category.IsActive) throw new ArgumentException("Selected product category is inactive.");
        var code = dto.Code.Trim().ToUpperInvariant();
        if (await templates.CodeExistsAsync(code))
            throw new InvalidOperationException("A product template with this code already exists.");
        await templates.AddAsync(new ProductTemplate(dto.Name, code, category.Id));
        await templates.SaveChangesAsync();
    }

    public async Task AssignAsync(AssignProductTemplateDto dto)
    {
        var product = await products.GetByIdAsync(dto.ProductId)
            ?? throw new InvalidOperationException("Product not found.");
        if (!dto.ProductTemplateId.HasValue)
        {
            product.AssignTemplate(null, null, null);
            await products.SaveChangesAsync();
            return;
        }
        var template = await templates.GetByIdAsync(dto.ProductTemplateId.Value)
            ?? throw new InvalidOperationException("Product template was not found.");
        if (!template.IsActive) throw new InvalidOperationException("Product template is inactive.");
        if (product.ProductCategoryId != template.ProductCategoryId)
            throw new InvalidOperationException("Product and template must use the same category.");

        var (signature, label) = await BuildVariantIdentityAsync(product);
        if (await templates.VariantSignatureExistsAsync(template.Id, signature, product.Id))
            throw new InvalidOperationException("This template already contains a product with the same variant values.");
        product.AssignTemplate(template.Id, signature, label);
        await products.SaveChangesAsync();
    }

    public async Task RefreshAssignedVariantAsync(Product product, IReadOnlyCollection<ProductAttributeValue> pendingValues)
    {
        if (!product.ProductTemplateId.HasValue) return;
        var template = await templates.GetByIdAsync(product.ProductTemplateId.Value)
            ?? throw new InvalidOperationException("Product template was not found.");
        var (signature, label) = await BuildVariantIdentityAsync(product, pendingValues);
        if (await templates.VariantSignatureExistsAsync(template.Id, signature, product.Id))
            throw new InvalidOperationException("This template already contains a product with the same variant values.");
        product.AssignTemplate(template.Id, signature, label);
    }

    private async Task<(string Signature, string Label)> BuildVariantIdentityAsync(
        Product product, IReadOnlyCollection<ProductAttributeValue>? suppliedValues = null)
    {
        var definitions = (await attributes.GetDefinitionsAsync()).Where(x => x.IsActive && x.IsVariantDefining).ToList();
        var links = await attributes.GetCategoryLinksAsync();
        HashSet<int> linkedIds = product.ProductCategoryId.HasValue
            ? links.Where(x => x.ProductCategoryId == product.ProductCategoryId.Value).Select(x => x.ProductAttributeDefinitionId).ToHashSet()
            : [];
        var scopedIds = links.Select(x => x.ProductAttributeDefinitionId).ToHashSet();
        var applicable = definitions.Where(x => !scopedIds.Contains(x.Id) || linkedIds.Contains(x.Id))
            .OrderBy(x => x.Code).ToList();
        if (applicable.Count == 0)
            throw new InvalidOperationException("The product category has no active variant-defining attributes.");
        var values = suppliedValues ?? await attributes.GetValuesAsync(product.Id);
        var options = await attributes.GetOptionsAsync();
        var canonicalParts = new List<string>();
        var labels = new List<string>();
        foreach (var definition in applicable)
        {
            var value = values.FirstOrDefault(x => x.ProductAttributeDefinitionId == definition.Id)
                ?? throw new InvalidOperationException("Every variant-defining attribute must have a value before template assignment.");
            var display = FormatValue(value, options);
            canonicalParts.Add($"{definition.Code}={display.Code}");
            labels.Add($"{definition.Name}: {display.Label}");
        }
        var canonical = string.Join("|", canonicalParts);
        var signature = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
        return (signature, string.Join(" · ", labels));
    }

    private async Task<List<GenerationCombination>> BuildGenerationPlanAsync(VariantGenerationInputDto dto)
    {
        var template = await templates.GetByIdAsync(dto.ProductTemplateId)
            ?? throw new InvalidOperationException("Product template was not found.");
        if (!template.IsActive) throw new InvalidOperationException("Product template is inactive.");
        var source = await products.GetByIdAsync(dto.SourceProductId)
            ?? throw new InvalidOperationException("Source product was not found.");
        if (source.ProductTemplateId != template.Id)
            throw new InvalidOperationException("Source product must already belong to the selected template.");

        var definitions = await attributes.GetDefinitionsAsync();
        var links = await attributes.GetCategoryLinksAsync();
        var applicable = GetGenerationDefinitions(template.ProductCategoryId, definitions, links);
        var allApplicableVariantDefinitions = GetApplicableVariantDefinitions(template.ProductCategoryId, definitions, links);
        if (allApplicableVariantDefinitions.Any(x => x.DataType != ProductAttributeDataType.Selection))
            throw new InvalidOperationException("Automatic generation supports selection-based variant attributes only.");
        if (applicable.Count == 0)
            throw new InvalidOperationException("The template category has no selectable variant attributes.");
        var allOptions = await attributes.GetOptionsAsync();
        var selected = allOptions.Where(x => x.IsActive && dto.OptionIds.Contains(x.Id)).ToList();
        var groups = applicable.Select(definition => selected.Where(x => x.ProductAttributeDefinitionId == definition.Id).ToList()).ToList();
        if (groups.Any(x => x.Count == 0) || selected.Count != dto.OptionIds.Distinct().Count())
            throw new InvalidOperationException("Select at least one valid option for every variant attribute.");
        var combinations = VariantCombinationBuilder.Build(groups, MaximumGeneratedVariants);
        var result = new List<GenerationCombination>();
        foreach (var combination in combinations)
        {
            var ordered = combination.OrderBy(x => definitions.Single(d => d.Id == x.ProductAttributeDefinitionId).Code).ToList();
            var canonical = string.Join("|", ordered.Select(x => $"{definitions.Single(d => d.Id == x.ProductAttributeDefinitionId).Code}=O:{x.Code}"));
            var signature = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
            var labels = ordered.Select(x => $"{definitions.Single(d => d.Id == x.ProductAttributeDefinitionId).Name}: {x.Name}").ToList();
            result.Add(new GenerationCombination(signature, string.Join(" · ", labels), string.Join(" / ", ordered.Select(x => x.Name)), ordered,
                await templates.VariantSignatureExistsAsync(template.Id, signature)));
        }
        return result;
    }

    private static List<ProductAttributeDefinition> GetGenerationDefinitions(int categoryId,
        IReadOnlyCollection<ProductAttributeDefinition> definitions, IReadOnlyCollection<ProductCategoryAttribute> links)
        => GetApplicableVariantDefinitions(categoryId, definitions, links)
            .Where(x => x.DataType == ProductAttributeDataType.Selection).ToList();

    private static List<ProductAttributeDefinition> GetApplicableVariantDefinitions(int categoryId,
        IReadOnlyCollection<ProductAttributeDefinition> definitions, IReadOnlyCollection<ProductCategoryAttribute> links)
    {
        var scoped = links.Select(x => x.ProductAttributeDefinitionId).ToHashSet();
        var linked = links.Where(x => x.ProductCategoryId == categoryId).Select(x => x.ProductAttributeDefinitionId).ToHashSet();
        return definitions.Where(x => x.IsActive && x.IsVariantDefining &&
                                      (!scoped.Contains(x.Id) || linked.Contains(x.Id)))
            .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Code).ToList();
    }

    private static ProductAttributeValue CloneValue(int productId, ProductAttributeValue value) =>
        new(productId, value.ProductAttributeDefinitionId, value.TextValue, value.NumberValue, value.BooleanValue, value.ProductAttributeOptionId);

    private sealed record GenerationCombination(string Signature, string Label, string ShortLabel,
        List<ProductAttributeOption> Options, bool Exists);

    private static (string Code, string Label) FormatValue(
        ProductAttributeValue value, IReadOnlyCollection<ProductAttributeOption> options)
    {
        if (value.TextValue is not null) return ($"T:{value.TextValue.Trim().ToUpperInvariant()}", value.TextValue);
        if (value.NumberValue.HasValue)
        {
            var number = value.NumberValue.Value.ToString(CultureInfo.InvariantCulture);
            return ($"N:{number}", number);
        }
        if (value.BooleanValue.HasValue)
            return (value.BooleanValue.Value ? "B:1" : "B:0", value.BooleanValue.Value ? "Yes" : "No");
        var option = options.FirstOrDefault(x => x.Id == value.ProductAttributeOptionId)
            ?? throw new InvalidOperationException("A variant selection option was not found.");
        return ($"O:{option.Code}", option.Name);
    }
}
