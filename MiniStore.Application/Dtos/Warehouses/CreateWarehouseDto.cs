namespace MiniStore.Application.DTOs.Warehouses;

public class CreateWarehouseDto
{
    public string Name { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public int InventoryAccountId { get; set; }
}
