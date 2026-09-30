namespace MiniStore.Domain.Entities;

public class StorageLocation
{
    public int Id { get; private set; }
    public int WarehouseId { get; private set; }
    public int? ParentLocationId { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? Barcode { get; private set; }
    public int Sequence { get; private set; }
    public string? Zone { get; private set; }
    public string? Aisle { get; private set; }
    public string? Rack { get; private set; }
    public string? Level { get; private set; }
    public string? Bin { get; private set; }
    public StorageLocationType Type { get; private set; }
    public StorageLocationStatus Status { get; private set; }
    public decimal? MaximumQuantity { get; private set; }
    public bool IsReceivable { get; private set; }
    public bool IsPickable { get; private set; }
    public bool IsReservable { get; private set; }
    public bool IsShippable { get; private set; }
    public bool IsCountable { get; private set; }

    private StorageLocation()
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public StorageLocation(
        int warehouseId,
        string code,
        string name,
        string? barcode,
        int? parentLocationId,
        int sequence,
        string? zone,
        string? aisle,
        string? rack,
        string? level,
        string? bin,
        StorageLocationType type,
        decimal? maximumQuantity,
        bool isReceivable,
        bool isPickable,
        bool isReservable,
        bool isShippable,
        bool isCountable)
    {
        if (warehouseId <= 0)
        {
            throw new ArgumentException("Warehouse is required.");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Location code is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Location name is required.");
        }

        if (sequence < 0)
        {
            throw new ArgumentException("Location sequence cannot be negative.");
        }

        if (maximumQuantity < 0)
        {
            throw new ArgumentException("Maximum quantity cannot be negative.");
        }

        WarehouseId = warehouseId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Barcode = Normalize(barcode);
        ParentLocationId = parentLocationId;
        Sequence = sequence;
        Zone = Normalize(zone);
        Aisle = Normalize(aisle);
        Rack = Normalize(rack);
        Level = Normalize(level);
        Bin = Normalize(bin);
        Type = type;
        Status = StorageLocationStatus.Active;
        MaximumQuantity = maximumQuantity;
        IsReceivable = isReceivable;
        IsPickable = isPickable;
        IsReservable = isReservable;
        IsShippable = isShippable;
        IsCountable = isCountable;
    }

    public void UpdateHierarchyAndCapabilities(
        string name,
        string? barcode,
        int? parentLocationId,
        int sequence,
        bool isReceivable,
        bool isPickable,
        bool isReservable,
        bool isShippable,
        bool isCountable)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Location name is required.");
        }

        if (sequence < 0)
        {
            throw new ArgumentException("Location sequence cannot be negative.");
        }

        Name = name.Trim();
        Barcode = Normalize(barcode);
        ParentLocationId = parentLocationId;
        Sequence = sequence;
        IsReceivable = isReceivable;
        IsPickable = isPickable;
        IsReservable = isReservable;
        IsShippable = isShippable;
        IsCountable = isCountable;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToUpperInvariant();
    }
}
