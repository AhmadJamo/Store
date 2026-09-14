namespace MiniStore.Domain.Entities;

public class Warehouse
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public int? BranchId { get; private set; }

    public int? InventoryAccountId { get; private set; }

    public WarehouseType Type { get; private set; } = WarehouseType.General;

    public InventoryControlMode ControlMode { get; private set; } = InventoryControlMode.Hybrid;

    public InventoryPickingStrategy PickingStrategy { get; private set; } = InventoryPickingStrategy.LocationPriority;

    public bool AllowPosSales { get; private set; }

    public bool EnforceLocationCapacity { get; private set; } = true;

    public bool RequireSourceLocationForTransfers { get; private set; }

    public bool RequireDestinationLocationForTransfers { get; private set; }

    public Warehouse(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Warehouse name is required.");

        Name = name;
    }

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Warehouse name is required.");

        Name = name;
    }

    public void AssignAccounting(int branchId, int inventoryAccountId)
    {
        if (branchId <= 0 || inventoryAccountId <= 0)
            throw new ArgumentException("A branch and inventory account are required.");
        BranchId = branchId;
        InventoryAccountId = inventoryAccountId;
    }

    public void ConfigureInventoryOperations(
        WarehouseType type,
        InventoryControlMode controlMode,
        InventoryPickingStrategy pickingStrategy,
        bool allowPosSales,
        bool enforceLocationCapacity,
        bool requireSourceLocationForTransfers,
        bool requireDestinationLocationForTransfers)
    {
        if (!Enum.IsDefined(type) ||
            !Enum.IsDefined(controlMode) ||
            !Enum.IsDefined(pickingStrategy))
        {
            throw new ArgumentException("Invalid warehouse inventory policy.");
        }

        if (controlMode == InventoryControlMode.Simple &&
            (requireSourceLocationForTransfers || requireDestinationLocationForTransfers))
        {
            throw new ArgumentException(
                "A simple warehouse cannot require exact transfer locations.");
        }

        Type = type;
        ControlMode = controlMode;
        PickingStrategy = pickingStrategy;
        AllowPosSales = allowPosSales;
        EnforceLocationCapacity = enforceLocationCapacity;
        RequireSourceLocationForTransfers = requireSourceLocationForTransfers;
        RequireDestinationLocationForTransfers = requireDestinationLocationForTransfers;
    }
}
