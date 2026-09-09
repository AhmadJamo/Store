namespace MiniStore.Application.DTOs.Purchases;

public class CreatePurchaseDto
{
    public int SupplierId { get; set; }

    public int WarehouseId { get; set; }

    public string InvoiceNumber { get; set; }
        = string.Empty;

    public DateTime Date { get; set; }

    public string? Notes { get; set; }

    public List<CreatePurchaseItemDto> Items { get; set; }
        = new();
}