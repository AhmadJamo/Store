using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Warehouses;

public class WarehouseInventorySearchDto
{
    public List<StorageLocation> Locations { get; set; } = [];
    public List<WarehouseProductSearchRowDto> Products { get; set; } = [];
}

public class WarehouseProductSearchRowDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Location { get; set; } = "Unassigned";
}
