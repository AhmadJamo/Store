namespace MiniStore.Domain.Entities;

public class InventorySettings
{
    public int Id { get; private set; }

    public int SingletonKey { get; private set; } = 1;

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public WarehouseType DefaultWarehouseType { get; private set; } = WarehouseType.General;

    public InventoryControlMode DefaultControlMode { get; private set; } = InventoryControlMode.Hybrid;

    public InventoryPickingStrategy DefaultPickingStrategy { get; private set; } = InventoryPickingStrategy.LocationPriority;

    public bool DefaultAllowPosSales { get; private set; }

    public bool DefaultEnforceLocationCapacity { get; private set; } = true;

    public bool DefaultRequireSourceLocationForTransfers { get; private set; }

    public bool DefaultRequireDestinationLocationForTransfers { get; private set; }

    private InventorySettings()
    {
    }

    public InventorySettings(
        WarehouseType defaultWarehouseType,
        InventoryControlMode defaultControlMode,
        InventoryPickingStrategy defaultPickingStrategy,
        bool defaultAllowPosSales,
        bool defaultEnforceLocationCapacity,
        bool defaultRequireSourceLocationForTransfers,
        bool defaultRequireDestinationLocationForTransfers)
    {
        Update(
            defaultWarehouseType,
            defaultControlMode,
            defaultPickingStrategy,
            defaultAllowPosSales,
            defaultEnforceLocationCapacity,
            defaultRequireSourceLocationForTransfers,
            defaultRequireDestinationLocationForTransfers);
    }

    public void Update(
        WarehouseType defaultWarehouseType,
        InventoryControlMode defaultControlMode,
        InventoryPickingStrategy defaultPickingStrategy,
        bool defaultAllowPosSales,
        bool defaultEnforceLocationCapacity,
        bool defaultRequireSourceLocationForTransfers,
        bool defaultRequireDestinationLocationForTransfers)
    {
        if (!Enum.IsDefined(defaultWarehouseType) ||
            !Enum.IsDefined(defaultControlMode) ||
            !Enum.IsDefined(defaultPickingStrategy))
        {
            throw new ArgumentException("Invalid default inventory policy.");
        }

        if (defaultControlMode == InventoryControlMode.Simple &&
            (defaultRequireSourceLocationForTransfers ||
             defaultRequireDestinationLocationForTransfers))
        {
            throw new ArgumentException(
                "Simple warehouses cannot require exact transfer locations.");
        }

        DefaultWarehouseType = defaultWarehouseType;
        DefaultControlMode = defaultControlMode;
        DefaultPickingStrategy = defaultPickingStrategy;
        DefaultAllowPosSales = defaultAllowPosSales;
        DefaultEnforceLocationCapacity = defaultEnforceLocationCapacity;
        DefaultRequireSourceLocationForTransfers = defaultRequireSourceLocationForTransfers;
        DefaultRequireDestinationLocationForTransfers = defaultRequireDestinationLocationForTransfers;
    }
}
