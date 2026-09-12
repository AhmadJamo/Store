using MiniStore.Domain.Enums;

namespace MiniStore.Application.Dtos.Settings;

public class DiscountSettingsDto
{
    public bool Enabled { get; set; }

    public bool AllowLineDiscount { get; set; }

    public bool AllowInvoiceDiscount { get; set; }

    public bool AllowPercentageDiscount { get; set; }

    public bool AllowFixedAmountDiscount { get; set; }

    public DiscountType DefaultDiscountType { get; set; }
        = DiscountType.Percentage;

    public decimal MaxLineDiscountPercent { get; set; } = 10;

    public decimal MaxLineDiscountAmount { get; set; } = 100;

    public decimal MaxInvoiceDiscountPercent { get; set; } = 10;

    public decimal MaxInvoiceDiscountAmount { get; set; } = 100;

    public bool AllowDiscountAboveLimit { get; set; }

    public string DiscountOverridePermission { get; set; }
        = "Sales.Discount.Override";

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}