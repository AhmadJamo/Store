
using MiniStore.Domain.Enums;

namespace MiniStore.Application.DTOs.Sales;

public class SaleDetailsDto
{
    public int Id { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public int WarehouseId { get; set; }
    public int? PosTerminalId { get; set; }
    public MiniStore.Domain.Entities.PosOrderType? PosOrderType { get; set; }
    public string? ServiceReference { get; set; }
    public int? GuestCount { get; set; }

    public DateTime Date { get; set; }

    public string? Notes { get; set; }

    public decimal Subtotal { get; set; }

    public DiscountType InvoiceDiscountType { get; set; }

    public decimal InvoiceDiscountValue { get; set; }

    public decimal InvoiceDiscountAmount { get; set; }

    public int? TaxRateId { get; set; }
    public decimal TaxRatePercent { get; set; }
    public bool IsTaxInclusive { get; set; }
    public decimal TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public List<SaleItemDetailsDto> Items { get; set; } = new();
}

