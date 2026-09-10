using MiniStore.Domain.Enums;

namespace MiniStore.Application.DTOs.Sales;

public class SaleItemDto
{
    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal SalePrice { get; set; }

    public DiscountType DiscountType { get; set; } = DiscountType.Percentage;

    public decimal DiscountValue { get; set; }
}