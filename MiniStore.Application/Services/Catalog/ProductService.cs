using MiniStore.Application.DTOs.Products;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class ProductService(
    IProductRepository productRepository,
    IMeasurementUnitRepository measurementUnits)
{
    public async Task<List<ProductDto>> GetAllAsync(string? search) =>
        (await productRepository.GetAllAsync(search)).Select(Map).ToList();

    public async Task<ProductListPageDto> SearchAsync(ProductListQueryDto query)
    {
        query.Page = Math.Max(1, query.Page);
        query.PageSize = Math.Clamp(query.PageSize, 10, 100);
        query.SortBy = query.SortBy?.Trim().ToLowerInvariant() switch
        {
            "code" => "code",
            "type" => "type",
            "price" => "price",
            _ => "name"
        };

        var page = await productRepository.SearchAsync(new ProductSearchCriteria(
            query.Search,
            query.ProductType,
            query.IsSellableInPos,
            query.IsSellableInSales,
            query.IsActive,
            query.SortBy,
            query.SortDescending,
            query.Page,
            query.PageSize));

        return new ProductListPageDto
        {
            Query = query,
            Items = page.Items.Select(Map).ToList(),
            TotalCount = page.TotalCount,
            TotalPages = Math.Max(1, (int)Math.Ceiling(page.TotalCount / (double)query.PageSize))
        };
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        return product is null ? null : Map(product);
    }

    public async Task CreateAsync(CreateProductDto dto)
    {
        var measurementUnit = await GetActiveUnitAsync(dto.MeasurementUnitId);
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

        await productRepository.AddAsync(product);
        await productRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateProductDto dto)
    {
        var measurementUnit = await GetActiveUnitAsync(dto.MeasurementUnitId);
        var product = await productRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Product not found.");

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

        await productRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Product not found.");
        await productRepository.DeleteAsync(product);
        await productRepository.SaveChangesAsync();
    }

    private static ProductDto Map(Product product) => new()
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
        IsActive = product.IsActive
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

    private static UnitOfMeasure MapLegacyUnit(string code) => code switch
    {
        "G" => UnitOfMeasure.Gram,
        "KG" => UnitOfMeasure.Kilogram,
        "ML" => UnitOfMeasure.Milliliter,
        "L" => UnitOfMeasure.Liter,
        _ => UnitOfMeasure.Piece
    };
}
