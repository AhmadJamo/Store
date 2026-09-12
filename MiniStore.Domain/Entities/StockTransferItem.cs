namespace MiniStore.Domain.Entities;

public class StockTransferItem
{
    public int Id { get; private set; }

    public int StockTransferId { get; private set; }

    public int ProductId { get; private set; }

    public decimal Quantity { get; private set; }

    private StockTransferItem()
    {
    }

    public StockTransferItem(
        int productId,
        decimal quantity)
    {
        if (productId <= 0)
            throw new ArgumentException("Product is required.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Transfer quantity must be greater than zero.");

        ProductId = productId;
        Quantity = quantity;
    }

    public void ChangeQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Transfer quantity must be greater than zero.");

        Quantity = quantity;
    }
}