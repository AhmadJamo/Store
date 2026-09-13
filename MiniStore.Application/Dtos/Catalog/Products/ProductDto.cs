namespace MiniStore.Application.DTOs.Products;

public class ProductDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Barcode { get; set; } = string.Empty;

    public decimal PurchasePrice { get; set; }

    public decimal SalePrice { get; set; }

    public decimal WholesalePrice { get; set; }
}
