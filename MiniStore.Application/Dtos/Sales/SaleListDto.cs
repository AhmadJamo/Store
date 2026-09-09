namespace MiniStore.Application.DTOs.Sale;

public class SaleListDto
{
    public int Id { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public DateTime Date { get; set; }

    public decimal TotalAmount { get; set; }
}