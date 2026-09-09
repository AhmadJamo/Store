using MiniStore.Application.DTOs.Sales;

namespace MiniStore.Application.DTOs.Sale;

public class CreateSaleDto
{
    public string InvoiceNumber { get; set; } = string.Empty;

    public int WarehouseId { get; set; }

    public DateTime Date { get; set; }

    public string? Notes { get; set; }

    public List<SaleItemDto> Items { get; set; } = new();
}