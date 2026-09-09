namespace MiniStore.Domain.Entities;

public class SaleItem
{
    public int Id { get; private set; }

    public int SaleId { get; private set; }

    public int ProductId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal SalePrice { get; private set; }

    public decimal Total { get; private set; }

    private SaleItem()
    {
    }

    public SaleItem(
        int productId,
        decimal quantity,
        decimal salePrice)
    {
        if (productId <= 0)
            throw new ArgumentException(
                "Product ID must be greater than zero.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (salePrice < 0)
            throw new ArgumentException(
                "Sale price cannot be negative.");

        ProductId = productId;
        Quantity = quantity;
        SalePrice = salePrice;

        Total = Quantity * SalePrice;
    }
}