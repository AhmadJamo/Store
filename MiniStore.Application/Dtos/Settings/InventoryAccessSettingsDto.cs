namespace MiniStore.Application.Dtos.Settings;

public class InventoryAccessSettingsDto
{
    public List<InventoryAccessBranchDto> Branches { get; set; } = [];
    public List<InventoryAccessWarehouseDto> Warehouses { get; set; } = [];
    public List<BranchWarehouseAccessDto> BranchAccesses { get; set; } = [];
    public List<PosTerminalAccessDto> PosTerminals { get; set; } = [];
}

public class InventoryAccessBranchDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}

public class InventoryAccessWarehouseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool AllowPosSales { get; set; }
}

public class BranchWarehouseAccessDto
{
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool IsDefaultForPos { get; set; }
    public bool AllowPosSales { get; set; }
    public bool AllowPurchases { get; set; }
    public bool AllowTransferOut { get; set; }
    public bool AllowTransferIn { get; set; }
    public bool AllowReplenishment { get; set; }
}

public class PosTerminalAccessDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public int DefaultWarehouseId { get; set; }
    public string DefaultWarehouseName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<PosTerminalWarehouseDto> Warehouses { get; set; } = [];
}

public class PosTerminalWarehouseDto
{
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int Priority { get; set; }
}

public class ConfigureBranchWarehouseDto
{
    public int BranchId { get; set; }
    public int WarehouseId { get; set; }
    public int Priority { get; set; } = 1;
    public bool IsDefaultForPos { get; set; }
    public bool AllowPosSales { get; set; }
    public bool AllowPurchases { get; set; }
    public bool AllowTransferOut { get; set; } = true;
    public bool AllowTransferIn { get; set; } = true;
    public bool AllowReplenishment { get; set; } = true;
}

public class CreatePosTerminalDto
{
    public string Name { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public int DefaultWarehouseId { get; set; }
}

public class ConfigurePosTerminalWarehouseDto
{
    public int PosTerminalId { get; set; }
    public int WarehouseId { get; set; }
    public int Priority { get; set; } = 1;
    public bool MakeDefault { get; set; }
}
