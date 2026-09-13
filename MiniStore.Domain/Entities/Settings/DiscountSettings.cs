using MiniStore.Domain.Enums;

namespace MiniStore.Domain.Entities;

public class DiscountSettings
{
    public int Id { get; private set; }

    public int SingletonKey { get; private set; } = 1;

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public bool Enabled { get; private set; }

    public bool AllowLineDiscount { get; private set; }

    public bool AllowInvoiceDiscount { get; private set; }

    public bool AllowPercentageDiscount { get; private set; }

    public bool AllowFixedAmountDiscount { get; private set; }

    public DiscountType DefaultDiscountType { get; private set; }

    public decimal MaxLineDiscountPercent { get; private set; }

    public decimal MaxLineDiscountAmount { get; private set; }

    public decimal MaxInvoiceDiscountPercent { get; private set; }

    public decimal MaxInvoiceDiscountAmount { get; private set; }

    public bool AllowDiscountAboveLimit { get; private set; }

    public string DiscountOverridePermission { get; private set; } = string.Empty;

    private DiscountSettings()
    {
        DiscountOverridePermission = "Sales.Discount.Override";
    }

    public DiscountSettings(
        bool enabled,
        bool allowLineDiscount,
        bool allowInvoiceDiscount,
        bool allowPercentageDiscount,
        bool allowFixedAmountDiscount,
        DiscountType defaultDiscountType,
        decimal maxLineDiscountPercent,
        decimal maxLineDiscountAmount,
        decimal maxInvoiceDiscountPercent,
        decimal maxInvoiceDiscountAmount,
        bool allowDiscountAboveLimit,
        string discountOverridePermission)
    {
        SetEnabled(enabled);
        SetAllowLineDiscount(allowLineDiscount);
        SetAllowInvoiceDiscount(allowInvoiceDiscount);
        SetAllowPercentageDiscount(allowPercentageDiscount);
        SetAllowFixedAmountDiscount(allowFixedAmountDiscount);
        SetDefaultDiscountType(defaultDiscountType);
        SetMaxLineDiscountPercent(maxLineDiscountPercent);
        SetMaxLineDiscountAmount(maxLineDiscountAmount);
        SetMaxInvoiceDiscountPercent(maxInvoiceDiscountPercent);
        SetMaxInvoiceDiscountAmount(maxInvoiceDiscountAmount);
        SetAllowDiscountAboveLimit(allowDiscountAboveLimit);
        SetDiscountOverridePermission(discountOverridePermission);
    }

    public void SetEnabled(bool value)
    {
        Enabled = value;
    }

    public void SetAllowLineDiscount(bool value)
    {
        AllowLineDiscount = value;
    }

    public void SetAllowInvoiceDiscount(bool value)
    {
        AllowInvoiceDiscount = value;
    }

    public void SetAllowPercentageDiscount(bool value)
    {
        AllowPercentageDiscount = value;
    }

    public void SetAllowFixedAmountDiscount(bool value)
    {
        AllowFixedAmountDiscount = value;
    }

    public void SetDefaultDiscountType(DiscountType value)
    {
        if (!Enum.IsDefined(value))
            throw new ArgumentException("Invalid discount type.");

        DefaultDiscountType = value;
    }

    public void SetMaxLineDiscountPercent(decimal value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentException(
                "Maximum line discount percentage must be between 0 and 100.");

        MaxLineDiscountPercent = value;
    }

    public void SetMaxLineDiscountAmount(decimal value)
    {
        if (value < 0)
            throw new ArgumentException(
                "Maximum line discount amount cannot be negative.");

        MaxLineDiscountAmount = value;
    }

    public void SetMaxInvoiceDiscountPercent(decimal value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentException(
                "Maximum invoice discount percentage must be between 0 and 100.");

        MaxInvoiceDiscountPercent = value;
    }

    public void SetMaxInvoiceDiscountAmount(decimal value)
    {
        if (value < 0)
            throw new ArgumentException(
                "Maximum invoice discount amount cannot be negative.");

        MaxInvoiceDiscountAmount = value;
    }

    public void SetAllowDiscountAboveLimit(bool value)
    {
        AllowDiscountAboveLimit = value;
    }

    public void SetDiscountOverridePermission(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "Discount override permission is required.");

        DiscountOverridePermission = value.Trim();
    }
}