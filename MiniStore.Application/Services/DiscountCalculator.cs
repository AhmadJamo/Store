using MiniStore.Domain.Enums;

namespace MiniStore.Application.Services;

public class DiscountCalculator
{
    public DiscountCalculationResult Calculate(
    decimal grossAmount,
    DiscountType discountType,
    decimal discountValue)
    {
        if (grossAmount < 0)
            throw new ArgumentException(
            "Gross amount cannot be negative.");


    if (discountValue < 0)
            throw new ArgumentException(
                "Discount value cannot be negative.");

        decimal discountAmount;

        switch (discountType)
        {
            case DiscountType.Percentage:

                if (discountValue > 100)
                    throw new ArgumentException(
                        "Percentage discount cannot exceed 100%.");

                discountAmount =
                    grossAmount * discountValue / 100m;

                break;

            case DiscountType.FixedAmount:

                discountAmount = discountValue;

                break;

            default:

                throw new ArgumentException(
                    "Invalid discount type.");
        }

        // Discount can never exceed the gross amount.
        if (discountAmount > grossAmount)
            discountAmount = grossAmount;

        var netAmount = grossAmount - discountAmount;

        return new DiscountCalculationResult(
            grossAmount,
            discountType,
            discountValue,
            discountAmount,
            netAmount);
    }


}

public record DiscountCalculationResult(
decimal GrossAmount,
DiscountType DiscountType,
decimal DiscountValue,
decimal DiscountAmount,
decimal NetAmount);
