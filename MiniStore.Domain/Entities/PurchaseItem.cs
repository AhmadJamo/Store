namespace MiniStore.Domain.Entities;

public class PurchaseItem
{
    public int Id { get; private set; }

    public int PurchaseId { get; private set; }

    public int ProductId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal PurchasePrice { get; private set; }

    public decimal Total { get; private set; }

    public PurchaseItem(
        int productId,
        decimal quantity,
        decimal purchasePrice)
    {
        if (productId <= 0)
            throw new ArgumentException(
                "Product is required.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (purchasePrice < 0)
            throw new ArgumentException(
                "Purchase price cannot be negative.");

        ProductId = productId;
        Quantity = quantity;
        PurchasePrice = purchasePrice;

        CalculateTotal();
    }

    public void ChangeQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        Quantity = quantity;

        CalculateTotal();
    }

    public void ChangePurchasePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException(
                "Purchase price cannot be negative.");

        PurchasePrice = price;

        CalculateTotal();
    }

    private void CalculateTotal()
    {
        Total = Quantity * PurchasePrice;
    }
}