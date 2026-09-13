namespace MiniStore.Application.DTOs.Warehouses;

public class WarehouseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public int? BranchId { get; set; }
    public int? InventoryAccountId { get; set; }
}
