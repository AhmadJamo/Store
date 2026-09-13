namespace MiniStore.Domain.Entities;

public class PurchaseItem
{
    public int Id { get; private set; }

    public int PurchaseId { get; private set; }

    public int ProductId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal PurchasePrice { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public int? TaxRateId { get; private set; }

    public decimal Total { get; private set; }

    public PurchaseItem(
        int productId,
        decimal quantity,
        decimal purchasePrice,
        decimal discountAmount = 0,
        int? taxRateId = null)
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

        if (discountAmount < 0 || discountAmount > quantity * purchasePrice)
            throw new ArgumentException("Purchase discount is invalid.");

        ProductId = productId;
        Quantity = quantity;
        PurchasePrice = purchasePrice;
        DiscountAmount = discountAmount;
        TaxRateId = taxRateId;

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
        Total = Quantity * PurchasePrice - DiscountAmount;
    }
}
