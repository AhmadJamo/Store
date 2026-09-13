namespace MiniStore.Application.DTOs.Warehouses;
public class UnassignedStockDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public decimal WarehouseQuantity { get; set; }
    public decimal AssignedQuantity { get; set; }
    public decimal UnassignedQuantity => WarehouseQuantity - AssignedQuantity;
}
