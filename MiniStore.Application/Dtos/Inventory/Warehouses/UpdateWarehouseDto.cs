using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Warehouses;

public class UpdateWarehouseDto
{
    public string Name { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public int InventoryAccountId { get; set; }
    public WarehouseType Type { get; set; }
    public InventoryControlMode ControlMode { get; set; }
    public InventoryPickingStrategy PickingStrategy { get; set; }
    public bool AllowPosSales { get; set; }
    public bool EnforceLocationCapacity { get; set; }
    public bool RequireSourceLocationForTransfers { get; set; }
    public bool RequireDestinationLocationForTransfers { get; set; }
}
