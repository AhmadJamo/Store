using MiniStore.Domain.Enums;

namespace MiniStore.Domain.Entities;

public class SaleItem
{
    public int Id { get; private set; }


public int SaleId { get; private set; }

    public int ProductId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal SalePrice { get; private set; }

    public decimal GrossTotal { get; private set; }

    public DiscountType DiscountType { get; private set; }

    public decimal DiscountValue { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal Total { get; private set; }

    private SaleItem()
    {
    }

    public SaleItem(
        int productId,
        decimal quantity,
        decimal salePrice,
        DiscountType discountType = DiscountType.Percentage,
        decimal discountValue = 0)
    {
        if (productId <= 0)
            throw new ArgumentException(
                "Product ID must be greater than zero.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (salePrice < 0)
            throw new ArgumentException(
                "Sale price cannot be negative.");

        if (discountValue < 0)
            throw new ArgumentException(
                "Discount value cannot be negative.");

        if (!Enum.IsDefined(discountType))
            throw new ArgumentException(
                "Invalid discount type.");

        var grossTotal = quantity * salePrice;

        decimal discountAmount;

        switch (discountType)
        {
            case DiscountType.Percentage:

                if (discountValue > 100)
                    throw new ArgumentException(
                        "Percentage discount cannot exceed 100%.");

                discountAmount =
                    grossTotal * discountValue / 100m;

                break;

            case DiscountType.FixedAmount:

                discountAmount = discountValue;

                break;

            default:

                throw new ArgumentException(
                    "Invalid discount type.");
        }

        if (discountAmount > grossTotal)
            discountAmount = grossTotal;

        ProductId = productId;
        Quantity = quantity;
        SalePrice = salePrice;

        GrossTotal = grossTotal;

        DiscountType = discountType;
        DiscountValue = discountValue;
        DiscountAmount = discountAmount;

        Total = GrossTotal - DiscountAmount;
    }


}
