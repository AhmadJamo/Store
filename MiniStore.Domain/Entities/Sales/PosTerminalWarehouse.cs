namespace MiniStore.Domain.Entities;

public class PosTerminalWarehouse
{
    public int PosTerminalId { get; private set; }
    public int WarehouseId { get; private set; }
    public int Priority { get; private set; }

    private PosTerminalWarehouse()
    {
    }

    public PosTerminalWarehouse(int warehouseId, int priority)
    {
        if (warehouseId <= 0)
            throw new ArgumentException("Warehouse is required.");
        if (priority is < 1 or > 999)
            throw new ArgumentException("Warehouse priority must be between 1 and 999.");

        WarehouseId = warehouseId;
        Priority = priority;
    }

    public void ChangePriority(int priority)
    {
        if (priority is < 1 or > 999)
            throw new ArgumentException("Warehouse priority must be between 1 and 999.");
        Priority = priority;
    }
}
