using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Settings;

public class InventorySettingsDto
{
    public WarehouseType DefaultWarehouseType { get; set; } = WarehouseType.General;
    public InventoryControlMode DefaultControlMode { get; set; } = InventoryControlMode.Hybrid;
    public InventoryPickingStrategy DefaultPickingStrategy { get; set; } = InventoryPickingStrategy.LocationPriority;
    public bool DefaultAllowPosSales { get; set; }
    public bool DefaultEnforceLocationCapacity { get; set; } = true;
    public bool DefaultRequireSourceLocationForTransfers { get; set; }
    public bool DefaultRequireDestinationLocationForTransfers { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
