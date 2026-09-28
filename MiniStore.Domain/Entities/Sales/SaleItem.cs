using MiniStore.Domain.Enums;

namespace MiniStore.Domain.Entities;

public class SaleItem
{
    public int Id { get; private set; }


public int SaleId { get; private set; }

    public int ProductId { get; private set; }

    public int? ProductRecipeId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal SalePrice { get; private set; }

    public decimal GrossTotal { get; private set; }

    public DiscountType DiscountType { get; private set; }

    public decimal DiscountValue { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal Total { get; private set; }
    public string? Notes { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal CostOfGoodsSold { get; private set; }

    private SaleItem()
    {
    }

    public void SetCostSnapshot(decimal unitCost, decimal costOfGoodsSold)
    {
        if (unitCost < 0 || costOfGoodsSold < 0)
            throw new ArgumentException("Sale cost cannot be negative.");

        UnitCost = Math.Round(unitCost, 8, MidpointRounding.AwayFromZero);
        CostOfGoodsSold = Math.Round(costOfGoodsSold, 8, MidpointRounding.AwayFromZero);
    }

    public SaleItem(
        int productId,
        decimal quantity,
        decimal salePrice,
        DiscountType discountType = DiscountType.Percentage,
        decimal discountValue = 0,
        string? notes = null,
        int? productRecipeId = null)
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
        if (notes?.Trim().Length > 200)
            throw new ArgumentException("Item notes cannot exceed 200 characters.");
        if (productRecipeId.HasValue && productRecipeId.Value <= 0)
            throw new ArgumentException("Recipe version is invalid.");

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
        ProductRecipeId = productRecipeId;
        Quantity = quantity;
        SalePrice = salePrice;

        GrossTotal = grossTotal;

        DiscountType = discountType;
        DiscountValue = discountValue;
        DiscountAmount = discountAmount;

        Total = GrossTotal - DiscountAmount;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }


}
