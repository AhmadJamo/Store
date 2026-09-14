using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Warehouses;

public class CreateWarehouseDto
{
    public string Name { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public int InventoryAccountId { get; set; }
    public WarehouseType Type { get; set; } = WarehouseType.General;
    public InventoryControlMode ControlMode { get; set; } = InventoryControlMode.Hybrid;
    public InventoryPickingStrategy PickingStrategy { get; set; } = InventoryPickingStrategy.LocationPriority;
    public bool AllowPosSales { get; set; }
    public bool EnforceLocationCapacity { get; set; } = true;
    public bool RequireSourceLocationForTransfers { get; set; }
    public bool RequireDestinationLocationForTransfers { get; set; }
}
