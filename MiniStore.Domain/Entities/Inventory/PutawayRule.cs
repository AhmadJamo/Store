namespace MiniStore.Domain.Entities;

public sealed class PutawayRule
{
    private PutawayRule() { }
    public int Id { get; private set; }
    public int WarehouseId { get; private set; }
    public int StorageLocationId { get; private set; }
    public int? ProductId { get; private set; }
    public int? ProductCategoryId { get; private set; }
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;

    public PutawayRule(int warehouseId, int storageLocationId, int? productId, int? productCategoryId, int priority)
    {
        if (warehouseId <= 0 || storageLocationId <= 0) throw new ArgumentException("Warehouse and storage location are required.");
        if (productId.HasValue && productCategoryId.HasValue) throw new ArgumentException("A putaway rule can target either a product or a category, not both.");
        if (productId <= 0 || productCategoryId <= 0) throw new ArgumentException("Putaway rule target is invalid.");
        if (priority is < 1 or > 9999) throw new ArgumentException("Putaway priority must be between 1 and 9999.");
        WarehouseId=warehouseId;StorageLocationId=storageLocationId;ProductId=productId;ProductCategoryId=productCategoryId;Priority=priority;
    }
    public void SetActive(bool active)=>IsActive=active;
}
