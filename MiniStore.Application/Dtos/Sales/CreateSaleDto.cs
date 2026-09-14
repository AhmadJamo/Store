using MiniStore.Domain.Enums;

namespace MiniStore.Application.DTOs.Sales;

public class CreateSaleDto
{
    public string InvoiceNumber { get; set; } = string.Empty;

    public int WarehouseId { get; set; }
    public int? PosTerminalId { get; set; }
    public int? CustomerId { get; set; }
    public int PaymentMethodId { get; set; }

    public DateTime Date { get; set; }

    public string? Notes { get; set; }

    public DiscountType InvoiceDiscountType { get; set; } = DiscountType.Percentage;

    public decimal InvoiceDiscountValue { get; set; }

    public List<SaleItemDto> Items { get; set; } = new();
}
