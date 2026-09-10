
using MiniStore.Domain.Enums;

namespace MiniStore.Application.DTOs.Sales;

public class SaleItemDetailsDto
{
    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal SalePrice { get; set; }

    public decimal GrossTotal { get; set; }

    public DiscountType DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal Total { get; set; }
}

