namespace MiniStore.Domain.Entities;

public class StorageLocation
{
    public int Id { get; private set; }
    public int WarehouseId { get; private set; }
    public string Code { get; private set; }
    public string? Zone { get; private set; }
    public string? Aisle { get; private set; }
    public string? Rack { get; private set; }
    public string? Level { get; private set; }
    public string? Bin { get; private set; }
    public StorageLocationType Type { get; private set; }
    public StorageLocationStatus Status { get; private set; }
    public decimal? MaximumQuantity { get; private set; }

    private StorageLocation()
    {
        Code = string.Empty;
    }

    public StorageLocation(
        int warehouseId,
        string code,
        string? zone,
        string? aisle,
        string? rack,
        string? level,
        string? bin,
        StorageLocationType type,
        decimal? maximumQuantity)
    {
        if (warehouseId <= 0)
        {
            throw new ArgumentException("Warehouse is required.");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Location code is required.");
        }

        if (maximumQuantity < 0)
        {
            throw new ArgumentException("Maximum quantity cannot be negative.");
        }

        WarehouseId = warehouseId;
        Code = code.Trim().ToUpperInvariant();
        Zone = Normalize(zone);
        Aisle = Normalize(aisle);
        Rack = Normalize(rack);
        Level = Normalize(level);
        Bin = Normalize(bin);
        Type = type;
        Status = StorageLocationStatus.Active;
        MaximumQuantity = maximumQuantity;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToUpperInvariant();
    }
}
