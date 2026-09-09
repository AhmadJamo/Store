namespace MiniStore.Domain.Entities;

public class StockTransaction
{
    public int Id { get; private set; }

    public int ProductId { get; private set; }

    public int WarehouseId { get; private set; }

    public decimal Quantity { get; private set; }

    public StockTransactionType Type { get; private set; }

    public string? Reference { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public StockTransaction(
        int productId,
        int warehouseId,
        decimal quantity,
        StockTransactionType type,
        string? reference = null)
    {
        if (productId <= 0)
            throw new ArgumentException(
                "Product is required.");

        if (warehouseId <= 0)
            throw new ArgumentException(
                "Warehouse is required.");

        if (quantity == 0)
            throw new ArgumentException(
                "Quantity cannot be zero.");

        ProductId = productId;
        WarehouseId = warehouseId;
        Quantity = quantity;
        Type = type;
        Reference = reference;
        CreatedAt = DateTime.UtcNow;
    }
}