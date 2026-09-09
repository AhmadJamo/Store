using MiniStore.Application.DTOs.Products;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> GetAllAsync(string? search)
    {
        var products = await _productRepository.GetAllAsync(search);

        return products.Select(product => new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Barcode = product.Barcode,
            PurchasePrice = product.PurchasePrice,
            SalePrice = product.SalePrice,
            WholesalePrice = product.WholesalePrice
        }).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Barcode = product.Barcode,
            PurchasePrice = product.PurchasePrice,
            SalePrice = product.SalePrice,
            WholesalePrice = product.WholesalePrice
        };
    }

    public async Task CreateAsync(CreateProductDto dto)
    {
        var existingProduct =
            await _productRepository.GetByBarcodeAsync(dto.Barcode);

        if (existingProduct != null)
            throw new InvalidOperationException(
                "A product with this barcode already exists.");

        var product = new Product(
            dto.Name,
            dto.Barcode,
            dto.PurchasePrice,
            dto.SalePrice,
            dto.WholesalePrice);

        await _productRepository.AddAsync(product);

        await _productRepository.SaveChangesAsync();
    }
    public async Task UpdateAsync(int id, UpdateProductDto dto )
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            throw new InvalidOperationException(
                "Product not found.");

        var existingProduct =
            await _productRepository.GetByBarcodeAsync(dto.Barcode);

        if (existingProduct != null &&
            existingProduct.Id != id)
        {
            throw new InvalidOperationException(
                "A product with this barcode already exists.");
        }

        product.ChangeName(dto.Name);

        product.ChangeBarcode(dto.Barcode);

        product.ChangeSalePrice(dto.SalePrice);

        product.ChangeWholesalePrice(dto.WholesalePrice);

        product.ChangePurchasePrice(dto.PurchasePrice);

        await _productRepository.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            throw new InvalidOperationException(
                "Product not found.");

        await _productRepository.DeleteAsync(product);

        await _productRepository.SaveChangesAsync();
    }

}
