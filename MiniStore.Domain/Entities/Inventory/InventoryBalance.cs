namespace MiniStore.Domain.Entities;

public sealed class InventoryBalance
{
    private InventoryBalance() { }
    public long Id { get; private set; }
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public int? StorageLocationId { get; private set; }
    public decimal OnHand { get; private set; }
    public decimal Reserved { get; private set; }
    public decimal Available => OnHand - Reserved;
    public DateTime UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public InventoryBalance(int productId, int warehouseId, int? storageLocationId, decimal onHand)
    {
        if (productId <= 0 || warehouseId <= 0) throw new ArgumentException("Product and warehouse are required.");
        if (storageLocationId <= 0) storageLocationId = null;
        ProductId = productId; WarehouseId = warehouseId; StorageLocationId = storageLocationId;
        SetOnHand(onHand);
    }

    public void SetOnHand(decimal quantity)
    {
        if (Reserved > 0 && quantity < Reserved) throw new InvalidOperationException("On-hand quantity cannot be lower than reserved quantity.");
        OnHand = quantity; UpdatedAt = DateTime.UtcNow;
    }

    public void SetReserved(decimal quantity)
    {
        if (quantity < 0) throw new ArgumentException("Reserved quantity cannot be negative.");
        if (quantity > 0 && (OnHand < 0 || quantity > OnHand)) throw new InvalidOperationException("Reserved quantity cannot exceed on-hand quantity.");
        Reserved = quantity; UpdatedAt = DateTime.UtcNow;
    }
}
