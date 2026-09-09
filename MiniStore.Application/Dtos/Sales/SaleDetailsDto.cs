namespace MiniStore.Application.DTOs.Sale;

public class SaleDetailsDto
{
    public int Id { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public DateTime Date { get; set; }

    public string? Notes { get; set; }

    public decimal TotalAmount { get; set; }

    public List<SaleItemDetailsDto> Items { get; set; } = new();
}