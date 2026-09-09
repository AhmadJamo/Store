namespace MiniStore.Application.DTOs.Purchases;

public class PurchaseDto
{
    public int Id { get; set; }

    public int SupplierId { get; set; }

    public string SupplierName { get; set; }
        = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; }
        = string.Empty;

    public string InvoiceNumber { get; set; }
        = string.Empty;

    public DateTime Date { get; set; }

    public string? Notes { get; set; }

    public decimal TotalAmount { get; set; }

    public List<PurchaseItemDto> Items { get; set; }
        = new();
}