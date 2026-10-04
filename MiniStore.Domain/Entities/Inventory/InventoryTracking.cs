namespace MiniStore.Domain.Entities;

public enum InventoryTrackingStatus { Available = 1, Depleted = 2, Quarantined = 3 }
public enum InventoryTrackingTransactionType { OpeningAllocation = 1, Receipt = 2, Issue = 3, Transfer = 4, Return = 5, Adjustment = 6 }

public sealed class InventoryTrackingBalance
{
    private InventoryTrackingBalance() { Identifier = SourceReference = string.Empty; }
    public long Id { get; private set; }
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public int? StorageLocationId { get; private set; }
    public ProductTrackingPolicy Policy { get; private set; }
    public string Identifier { get; private set; }
    public decimal Quantity { get; private set; }
    public DateOnly? ManufactureDate { get; private set; }
    public DateOnly? ExpirationDate { get; private set; }
    public InventoryTrackingStatus Status { get; private set; }
    public string SourceReference { get; private set; }
    public DateTime ReceivedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public InventoryTrackingBalance(int productId, int warehouseId, int? locationId,
        ProductTrackingPolicy policy, string identifier, decimal quantity,
        DateOnly? manufactureDate, DateOnly? expirationDate, string sourceReference)
    {
        if (productId <= 0 || warehouseId <= 0) throw new ArgumentException("Product and warehouse are required.");
        if (locationId <= 0) locationId = null;
        if (policy is not (ProductTrackingPolicy.Lot or ProductTrackingPolicy.Serial)) throw new ArgumentException("Lot or serial tracking is required.");
        var normalized = Normalize(identifier);
        if (normalized.Length > 100) throw new ArgumentException("Lot or serial number cannot exceed 100 characters.");
        if (quantity <= 0 || (policy == ProductTrackingPolicy.Serial && quantity != 1)) throw new ArgumentException("Serial quantity must equal one and lot quantity must be positive.");
        if (manufactureDate.HasValue && expirationDate.HasValue && expirationDate < manufactureDate) throw new ArgumentException("Expiration date cannot be before manufacture date.");
        if (string.IsNullOrWhiteSpace(sourceReference) || sourceReference.Trim().Length > 100) throw new ArgumentException("Tracking source reference is required.");
        ProductId=productId; WarehouseId=warehouseId; StorageLocationId=locationId; Policy=policy; Identifier=normalized;
        Quantity=quantity; ManufactureDate=manufactureDate; ExpirationDate=expirationDate; SourceReference=sourceReference.Trim();
        Status=InventoryTrackingStatus.Available; ReceivedAt=DateTime.UtcNow;
    }
    public void Add(decimal quantity) { if (Policy==ProductTrackingPolicy.Serial) throw new InvalidOperationException("Serial balances cannot be increased."); if(quantity<=0) throw new ArgumentException("Quantity must be positive."); Quantity+=quantity; Status=InventoryTrackingStatus.Available; }
    public void Remove(decimal quantity) { if(quantity<=0||quantity>Quantity) throw new InvalidOperationException("Insufficient tracked quantity."); Quantity-=quantity; if(Quantity==0) Status=InventoryTrackingStatus.Depleted; }
    public void Restore(decimal quantity)
    {
        if (quantity <= 0 || (Policy == ProductTrackingPolicy.Serial && (quantity != 1 || Quantity != 0)))
            throw new InvalidOperationException("Invalid tracked return quantity.");
        Quantity += quantity;
        Status = InventoryTrackingStatus.Available;
    }
    public void Relocate(int warehouseId, int? locationId)
    {
        if (Policy != ProductTrackingPolicy.Serial || Quantity != 1)
            throw new InvalidOperationException("Only an available serial can be relocated directly.");
        if (warehouseId <= 0)
            throw new ArgumentException("Warehouse is required.");
        if (locationId <= 0)
            locationId = null;

        WarehouseId = warehouseId;
        StorageLocationId = locationId;
    }
    private static string Normalize(string value) { if(string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Lot or serial number is required."); return value.Trim().ToUpperInvariant(); }
}

public sealed class InventoryTrackingTransaction
{
    private InventoryTrackingTransaction() { Identifier=SourceReference=CreatedByUserId=string.Empty; }
    public long Id { get; private set; }
    public long InventoryTrackingBalanceId { get; private set; }
    public InventoryTrackingBalance Balance { get; private set; } = null!;
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public int? StorageLocationId { get; private set; }
    public string Identifier { get; private set; }
    public decimal Quantity { get; private set; }
    public InventoryTrackingTransactionType Type { get; private set; }
    public string SourceReference { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public InventoryTrackingTransaction(long balanceId,int productId,int warehouseId,int? locationId,string identifier,decimal quantity,InventoryTrackingTransactionType type,string sourceReference,string userId)
    {
        if(balanceId<=0||productId<=0||warehouseId<=0) throw new ArgumentException("Tracking balance, product and warehouse are required.");
        if(quantity==0) throw new ArgumentException("Tracking transaction quantity cannot be zero.");
        if(string.IsNullOrWhiteSpace(identifier)||string.IsNullOrWhiteSpace(sourceReference)||string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Tracking transaction identity is required.");
        InventoryTrackingBalanceId=balanceId; ProductId=productId; WarehouseId=warehouseId; StorageLocationId=locationId;
        Identifier=identifier.Trim().ToUpperInvariant(); Quantity=quantity; Type=type; SourceReference=sourceReference.Trim(); CreatedByUserId=userId.Trim(); CreatedAt=DateTime.UtcNow;
    }
    public InventoryTrackingTransaction(InventoryTrackingBalance balance, decimal quantity, InventoryTrackingTransactionType type, string sourceReference, string userId)
        : this(1, balance.ProductId, balance.WarehouseId, balance.StorageLocationId, balance.Identifier, quantity, type, sourceReference, userId)
    { Balance = balance ?? throw new ArgumentNullException(nameof(balance)); InventoryTrackingBalanceId = 0; }
}
