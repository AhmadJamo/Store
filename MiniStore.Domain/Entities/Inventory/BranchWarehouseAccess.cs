namespace MiniStore.Domain.Entities;

public class BranchWarehouseAccess
{
    public int BranchId { get; private set; }
    public int WarehouseId { get; private set; }
    public int Priority { get; private set; }
    public bool IsDefaultForPos { get; private set; }
    public bool AllowPosSales { get; private set; }
    public bool AllowPurchases { get; private set; }
    public bool AllowTransferOut { get; private set; }
    public bool AllowTransferIn { get; private set; }
    public bool AllowReplenishment { get; private set; }

    private BranchWarehouseAccess()
    {
    }

    public BranchWarehouseAccess(int branchId, int warehouseId) =>
        Configure(
            branchId,
            warehouseId,
            priority: 1,
            isDefaultForPos: false,
            allowPosSales: false,
            allowPurchases: false,
            allowTransferOut: true,
            allowTransferIn: true,
            allowReplenishment: true);

    public void Configure(
        int branchId,
        int warehouseId,
        int priority,
        bool isDefaultForPos,
        bool allowPosSales,
        bool allowPurchases,
        bool allowTransferOut,
        bool allowTransferIn,
        bool allowReplenishment)
    {
        if (branchId <= 0 || warehouseId <= 0)
            throw new ArgumentException("Branch and warehouse are required.");
        if (priority is < 1 or > 999)
            throw new ArgumentException("Warehouse priority must be between 1 and 999.");
        if (isDefaultForPos && !allowPosSales)
            throw new ArgumentException("The default POS warehouse must allow POS sales.");

        BranchId = branchId;
        WarehouseId = warehouseId;
        Priority = priority;
        IsDefaultForPos = isDefaultForPos;
        AllowPosSales = allowPosSales;
        AllowPurchases = allowPurchases;
        AllowTransferOut = allowTransferOut;
        AllowTransferIn = allowTransferIn;
        AllowReplenishment = allowReplenishment;
    }

    public void ClearDefaultForPos() => IsDefaultForPos = false;
}
