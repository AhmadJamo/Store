using MiniStore.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace MiniStore.Application.DTOs.Warehouses;

public class CreateWarehouseDto
{
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int BranchId { get; set; }
    [Range(1, int.MaxValue)]
    public int InventoryAccountId { get; set; }
    public WarehouseType Type { get; set; } = WarehouseType.General;
    public InventoryControlMode ControlMode { get; set; } = InventoryControlMode.Hybrid;
    public InventoryPickingStrategy PickingStrategy { get; set; } = InventoryPickingStrategy.LocationPriority;
    public bool AllowPosSales { get; set; }
    public bool EnforceLocationCapacity { get; set; } = true;
    public bool RequireSourceLocationForTransfers { get; set; }
    public bool RequireDestinationLocationForTransfers { get; set; }
    public PurchaseReceivingRoute PurchaseReceivingRoute { get; set; } = PurchaseReceivingRoute.OneStep;
}
