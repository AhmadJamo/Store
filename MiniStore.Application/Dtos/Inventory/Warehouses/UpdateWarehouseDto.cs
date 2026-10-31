using MiniStore.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace MiniStore.Application.DTOs.Warehouses;

public class UpdateWarehouseDto
{
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int BranchId { get; set; }
    [Range(1, int.MaxValue)]
    public int InventoryAccountId { get; set; }
    public WarehouseType Type { get; set; }
    public InventoryControlMode ControlMode { get; set; }
    public InventoryPickingStrategy PickingStrategy { get; set; }
    public bool AllowPosSales { get; set; }
    public bool EnforceLocationCapacity { get; set; }
    public bool RequireSourceLocationForTransfers { get; set; }
    public bool RequireDestinationLocationForTransfers { get; set; }
    public PurchaseReceivingRoute PurchaseReceivingRoute { get; set; }
}
