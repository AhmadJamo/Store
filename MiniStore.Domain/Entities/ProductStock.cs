namespace MiniStore.Domain.Entities;

public class ProductStock
{
    public int Id { get; private set; }

    public int ProductId { get; private set; }

    public int WarehouseId { get; private set; }

    public decimal Quantity { get; private set; }
    //State
    public ProductStock(
        int productId,
        int warehouseId)
    {
        if (productId <= 0)
            throw new ArgumentException(
                "Product is required.");

        if (warehouseId <= 0)
            throw new ArgumentException(
                "Warehouse is required.");

        ProductId = productId;
        WarehouseId = warehouseId;
        Quantity = 0;
    }
    //Behavior
    public void AddQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        Quantity += quantity;
    }

    public void RemoveQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (quantity > Quantity)
            throw new InvalidOperationException(
                "Insufficient stock.");

        Quantity -= quantity;
    }
}