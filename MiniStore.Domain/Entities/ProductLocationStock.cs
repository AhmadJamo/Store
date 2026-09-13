namespace MiniStore.Domain.Entities;

public class ProductLocationStock
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public int StorageLocationId { get; private set; }
    public decimal Quantity { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    private ProductLocationStock() { }
    public ProductLocationStock(int productId, int warehouseId, int storageLocationId)
    {
        if (productId <= 0 || warehouseId <= 0 || storageLocationId <= 0) throw new ArgumentException("Product, warehouse and storage location are required.");
        ProductId = productId; WarehouseId = warehouseId; StorageLocationId = storageLocationId;
    }
    public void AddQuantity(decimal quantity) { if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero."); Quantity += quantity; }
    public void RemoveQuantity(decimal quantity) { if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero."); if (quantity > Quantity) throw new InvalidOperationException("Insufficient stock in the selected location."); Quantity -= quantity; }
}
