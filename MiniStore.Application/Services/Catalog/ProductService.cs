using MiniStore.Application.DTOs.Products;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class ProductService(
    IProductRepository productRepository,
    IMeasurementUnitRepository measurementUnits,
    IProductCategoryRepository productCategories)
{
    public async Task<List<ProductDto>> GetAllAsync(string? search)
    {
        var categoryNames = await GetCategoryNamesAsync();
        return (await productRepository.GetAllAsync(search))
            .Select(product => Map(product, categoryNames))
            .ToList();
    }

    public async Task<ProductListPageDto> SearchAsync(ProductListQueryDto query)
    {
        query.Page = Math.Max(1, query.Page);
        query.PageSize = Math.Clamp(query.PageSize, 10, 100);
        query.SortBy = query.SortBy?.Trim().ToLowerInvariant() switch
        {
            "code" => "code",
            "type" => "type",
            "category" => "category",
            "tracking" => "tracking",
            "price" => "price",
            _ => "name"
        };

        var page = await productRepository.SearchAsync(new ProductSearchCriteria(
            query.Search,
            query.ProductType,
            query.ProductCategoryId,
            query.TrackingPolicy,
            query.IsSellableInPos,
            query.IsSellableInSales,
            query.IsActive,
            query.SortBy,
            query.SortDescending,
            query.Page,
            query.PageSize));

        var categoryNames = await GetCategoryNamesAsync();
        return new ProductListPageDto
        {
            Query = query,
            Items = page.Items.Select(product => Map(product, categoryNames)).ToList(),
            Categories = (await productCategories.GetAllAsync())
                .Select(category => new ProductCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Code = category.Code,
                    IsActive = category.IsActive
                })
                .ToList(),
            TotalCount = page.TotalCount,
            TotalPages = Math.Max(1, (int)Math.Ceiling(page.TotalCount / (double)query.PageSize))
        };
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        return product is null ? null : Map(product, await GetCategoryNamesAsync());
    }

    public async Task CreateAsync(CreateProductDto dto)
    {
        var measurementUnit = await GetActiveUnitAsync(dto.MeasurementUnitId);
        var category = await GetActiveCategoryAsync(dto.ProductCategoryId);
        var weightUnit = await GetOptionalUnitAsync(
            dto.WeightMeasurementUnitId, MeasurementDimension.Mass, "weight");
        var dimensionUnit = await GetOptionalUnitAsync(
            dto.DimensionMeasurementUnitId, MeasurementDimension.Length, "dimension");
        var normalizedBarcode = NormalizeBarcode(dto.Barcode);
        if (normalizedBarcode is not null &&
            await productRepository.GetByBarcodeAsync(normalizedBarcode) is not null)
        {
            throw new InvalidOperationException("A product with this barcode already exists.");
        }

        var product = new Product(
            dto.Name,
            normalizedBarcode,
            dto.PurchasePrice,
            dto.SalePrice,
            dto.WholesalePrice,
            MapInventoryBehavior(dto.ProductType),
            MapLegacyUnit(measurementUnit.Code),
            dto.AllowNegativeRecipeConsumption,
            dto.ProductType,
            dto.IsSellableInPos,
            dto.IsSellableInSales,
            dto.IsActive,
            measurementUnit.Id);

        product.ConfigureLogistics(
            category?.Id,
            dto.NetWeight,
            dto.GrossWeight,
            weightUnit?.Id,
            dto.Length,
            dto.Width,
            dto.Height,
            dimensionUnit?.Id,
            dto.TrackingPolicy,
            MapHandlingRequirements(dto));

        await productRepository.AddAsync(product);
        await productRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateProductDto dto)
    {
        var measurementUnit = await GetActiveUnitAsync(dto.MeasurementUnitId);
        var product = await productRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Product not found.");
        var category = await GetCategoryAsync(
            dto.ProductCategoryId,
            requireActive: product.ProductCategoryId != dto.ProductCategoryId);
        if (product.ProductTemplateId.HasValue && product.ProductCategoryId != dto.ProductCategoryId)
            throw new InvalidOperationException(
                "Remove the product from its template before changing its category.");
        var weightUnit = await GetOptionalUnitAsync(
            dto.WeightMeasurementUnitId, MeasurementDimension.Mass, "weight");
        var dimensionUnit = await GetOptionalUnitAsync(
            dto.DimensionMeasurementUnitId, MeasurementDimension.Length, "dimension");

        var normalizedBarcode = NormalizeBarcode(dto.Barcode);
        var existingProduct = normalizedBarcode is null
            ? null
            : await productRepository.GetByBarcodeAsync(normalizedBarcode);
        if (existingProduct is not null && existingProduct.Id != id)
            throw new InvalidOperationException("A product with this barcode already exists.");

        product.ConfigureProduct(
            dto.Name,
            normalizedBarcode,
            dto.PurchasePrice,
            dto.SalePrice,
            dto.WholesalePrice,
            dto.ProductType,
            MapLegacyUnit(measurementUnit.Code),
            dto.AllowNegativeRecipeConsumption,
            dto.IsSellableInPos,
            dto.IsSellableInSales,
            dto.IsActive,
            measurementUnit.Id);

        if (product.TrackingPolicy != dto.TrackingPolicy &&
            dto.TrackingPolicy != ProductTrackingPolicy.None &&
            await productRepository.HasNonZeroStockAsync(id))
        {
            throw new InvalidOperationException(
                "Lot or serial tracking cannot be enabled while the product has a non-zero stock balance.");
        }

        product.ConfigureLogistics(
            category?.Id,
            dto.NetWeight,
            dto.GrossWeight,
            weightUnit?.Id,
            dto.Length,
            dto.Width,
            dto.Height,
            dimensionUnit?.Id,
            dto.TrackingPolicy,
            MapHandlingRequirements(dto));

        await productRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Product not found.");
        await productRepository.DeleteAsync(product);
        await productRepository.SaveChangesAsync();
    }

    private static ProductDto Map(
        Product product,
        IReadOnlyDictionary<int, string> categoryNames) => new()
    {
        Id = product.Id,
        ProductCode = product.ProductCode,
        Name = product.Name,
        Barcode = product.Barcode,
        PurchasePrice = product.PurchasePrice,
        SalePrice = product.SalePrice,
        WholesalePrice = product.WholesalePrice,
        InventoryBehavior = product.InventoryBehavior,
        ProductType = product.ProductType,
        StockUnit = product.StockUnit,
        MeasurementUnitId = product.MeasurementUnitId,
        AllowNegativeRecipeConsumption = product.AllowNegativeRecipeConsumption,
        IsSellableInPos = product.IsSellableInPos,
        IsSellableInSales = product.IsSellableInSales,
        IsActive = product.IsActive,
        ProductCategoryId = product.ProductCategoryId,
        ProductCategoryName = product.ProductCategoryId.HasValue
            ? categoryNames.GetValueOrDefault(product.ProductCategoryId.Value)
            : null,
        ProductTemplateId = product.ProductTemplateId,
        VariantLabel = product.VariantLabel,
        NetWeight = product.NetWeight,
        GrossWeight = product.GrossWeight,
        WeightMeasurementUnitId = product.WeightMeasurementUnitId,
        Length = product.Length,
        Width = product.Width,
        Height = product.Height,
        DimensionMeasurementUnitId = product.DimensionMeasurementUnitId,
        TrackingPolicy = product.TrackingPolicy,
        HandlingRequirements = product.HandlingRequirements
    };

    private static ProductInventoryBehavior MapInventoryBehavior(ProductType productType) =>
        productType == ProductType.PreparedToOrder
            ? ProductInventoryBehavior.PreparedToOrder
            : ProductInventoryBehavior.Stocked;

    private static string? NormalizeBarcode(string? barcode) =>
        string.IsNullOrWhiteSpace(barcode) ? null : barcode.Trim();

    private async Task<MeasurementUnit> GetActiveUnitAsync(int id)
    {
        var unit = await measurementUnits.GetByIdAsync(id)
            ?? throw new ArgumentException("Selected measurement unit was not found.");
        if (!unit.IsActive)
            throw new ArgumentException("Selected measurement unit is inactive.");
        return unit;
    }

    private async Task<MeasurementUnit?> GetOptionalUnitAsync(
        int? id,
        MeasurementDimension expectedDimension,
        string purpose)
    {
        if (!id.HasValue)
            return null;
        var unit = await measurementUnits.GetByIdAsync(id.Value)
            ?? throw new ArgumentException($"Selected {purpose} unit was not found.");
        if (!unit.IsActive)
            throw new ArgumentException($"Selected {purpose} unit is inactive.");
        if (unit.Dimension != expectedDimension)
            throw new ArgumentException($"Selected {purpose} unit has an incompatible dimension.");
        return unit;
    }

    private async Task<ProductCategory?> GetActiveCategoryAsync(int? id)
        => await GetCategoryAsync(id, requireActive: true);

    private async Task<ProductCategory?> GetCategoryAsync(int? id, bool requireActive)
    {
        if (!id.HasValue)
            return null;
        var category = await productCategories.GetByIdAsync(id.Value)
            ?? throw new ArgumentException("Selected product category was not found.");
        if (requireActive && !category.IsActive)
            throw new ArgumentException("Selected product category is inactive.");
        return category;
    }

    private async Task<Dictionary<int, string>> GetCategoryNamesAsync() =>
        (await productCategories.GetAllAsync())
            .ToDictionary(category => category.Id, category => category.Name);

    private static ProductHandlingRequirements MapHandlingRequirements(CreateProductDto dto) =>
        (dto.IsFragile ? ProductHandlingRequirements.Fragile : 0) |
        (dto.KeepDry ? ProductHandlingRequirements.KeepDry : 0) |
        (dto.RequiresRefrigeration ? ProductHandlingRequirements.Refrigerated : 0) |
        (dto.RequiresFrozenStorage ? ProductHandlingRequirements.Frozen : 0) |
        (dto.IsHazardous ? ProductHandlingRequirements.Hazardous : 0);

    private static ProductHandlingRequirements MapHandlingRequirements(UpdateProductDto dto) =>
        (dto.IsFragile ? ProductHandlingRequirements.Fragile : 0) |
        (dto.KeepDry ? ProductHandlingRequirements.KeepDry : 0) |
        (dto.RequiresRefrigeration ? ProductHandlingRequirements.Refrigerated : 0) |
        (dto.RequiresFrozenStorage ? ProductHandlingRequirements.Frozen : 0) |
        (dto.IsHazardous ? ProductHandlingRequirements.Hazardous : 0);

    private static UnitOfMeasure MapLegacyUnit(string code) => code switch
    {
        "G" => UnitOfMeasure.Gram,
        "KG" => UnitOfMeasure.Kilogram,
        "ML" => UnitOfMeasure.Milliliter,
        "L" => UnitOfMeasure.Liter,
        _ => UnitOfMeasure.Piece
    };
}
