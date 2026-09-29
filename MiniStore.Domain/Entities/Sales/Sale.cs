using MiniStore.Domain.Enums;

namespace MiniStore.Domain.Entities;

public class Sale
{
    public int Id { get; private set; }


public string InvoiceNumber { get; private set; }

    public SaleChannel Channel { get; private set; }

    public string CreatedByUserId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public int WarehouseId { get; private set; }
    public int? CustomerId { get; private set; }
    public int? PaymentMethodId { get; private set; }
    public int? PosTerminalId { get; private set; }
    public PosOrderType? PosOrderType { get; private set; }
    public string? ServiceReference { get; private set; }
    public int? GuestCount { get; private set; }

    public DateTime Date { get; private set; }

    public string? Notes { get; private set; }

    public decimal Subtotal { get; private set; }

    public DiscountType InvoiceDiscountType { get; private set; }

    public decimal InvoiceDiscountValue { get; private set; }

    public decimal InvoiceDiscountAmount { get; private set; }

    public int? TaxRateId { get; private set; }
    public decimal TaxRatePercent { get; private set; }
    public int? TaxOutputAccountId { get; private set; }
    public bool IsTaxInclusive { get; private set; }
    public decimal TaxAmount { get; private set; }

    public decimal TotalAmount { get; private set; }

    public List<SaleItem> Items { get; private set; }

    private Sale()
    {
        InvoiceNumber = string.Empty;
        CreatedByUserId = string.Empty;
        Items = new List<SaleItem>();
    }

    public Sale(
        string invoiceNumber,
        int warehouseId,
        DateTime date,
        SaleChannel channel,
        string createdByUserId,
        int? customerId,
        int paymentMethodId,
        string? notes = null,
        int? posTerminalId = null,
        PosOrderType? posOrderType = null,
        string? serviceReference = null,
        int? guestCount = null)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new ArgumentException(
                "Invoice number is required.");

        if (warehouseId <= 0)
            throw new ArgumentException(
                "Warehouse ID must be greater than zero.");

        if (string.IsNullOrWhiteSpace(createdByUserId))
            throw new ArgumentException(
                "The user who created the sale is required.");
        if (customerId.HasValue && customerId.Value <= 0) throw new ArgumentException("Customer is invalid.");
        if (paymentMethodId <= 0) throw new ArgumentException("A payment method is required.");
        if (posTerminalId.HasValue && posTerminalId.Value <= 0)
            throw new ArgumentException("POS terminal is invalid.");
        if (posOrderType.HasValue && !Enum.IsDefined(posOrderType.Value))
            throw new ArgumentException("POS order type is invalid.");
        if (serviceReference?.Trim().Length > 80)
            throw new ArgumentException("Service reference cannot exceed 80 characters.");
        if (guestCount is <= 0 or > 999)
            throw new ArgumentException("Guest count must be between 1 and 999.");

        InvoiceNumber = invoiceNumber;
        WarehouseId = warehouseId;
        Date = date;
        Channel = channel;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
        CustomerId = customerId;
        PaymentMethodId = paymentMethodId;
        PosTerminalId = posTerminalId;
        PosOrderType = posOrderType;
        ServiceReference = string.IsNullOrWhiteSpace(serviceReference)
            ? null
            : serviceReference.Trim();
        GuestCount = guestCount;
        Notes = notes;

        Items = new List<SaleItem>();

        Subtotal = 0;
        InvoiceDiscountType = DiscountType.Percentage;
        InvoiceDiscountValue = 0;
        InvoiceDiscountAmount = 0;
        TaxRatePercent = 0;
        IsTaxInclusive = false;
        TaxAmount = 0;
        TotalAmount = 0;
    }

    public void ApplyTax(
        int taxRateId,
        decimal rate,
        int outputAccountId,
        bool isPriceInclusive)
    {
        if (taxRateId <= 0 || rate < 0 || rate > 100 || outputAccountId <= 0)
            throw new ArgumentException("The selected sales tax is invalid.");

        TaxRateId = taxRateId;
        TaxRatePercent = rate;
        TaxOutputAccountId = outputAccountId;
        IsTaxInclusive = isPriceInclusive;
        RecalculateTotal();
    }

    public void AddItem(SaleItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (Items.Any(x => x.ProductId == item.ProductId))
            throw new InvalidOperationException(
                "The same product cannot be added more than once.");

        Items.Add(item);

        RecalculateTotal();
    }

    public void RemoveItem(SaleItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        Items.Remove(item);

        RecalculateTotal();
    }

    public void ApplyInvoiceDiscount(
        DiscountType discountType,
        decimal discountValue)
    {
        if (!Enum.IsDefined(discountType))
            throw new ArgumentException(
                "Invalid discount type.");

        if (discountValue < 0)
            throw new ArgumentException(
                "Discount value cannot be negative.");

        if (discountType == DiscountType.Percentage &&
            discountValue > 100)
        {
            throw new ArgumentException(
                "Percentage discount cannot exceed 100%.");
        }

        InvoiceDiscountType = discountType;
        InvoiceDiscountValue = discountValue;

        InvoiceDiscountAmount = CalculateDiscountAmount(
            Subtotal,
            discountType,
            discountValue);

        RecalculateTaxAndTotal();
    }

    public void RemoveInvoiceDiscount()
    {
        InvoiceDiscountType = DiscountType.Percentage;
        InvoiceDiscountValue = 0;
        InvoiceDiscountAmount = 0;

        RecalculateTaxAndTotal();
    }

    private void RecalculateTotal()
    {
        Subtotal = Items.Sum(x => x.Total);

        InvoiceDiscountAmount = CalculateDiscountAmount(
            Subtotal,
            InvoiceDiscountType,
            InvoiceDiscountValue);

        RecalculateTaxAndTotal();
    }

    private void RecalculateTaxAndTotal()
    {
        var discountedAmount = Subtotal - InvoiceDiscountAmount;
        TaxAmount = !TaxRateId.HasValue || TaxRatePercent <= 0
            ? 0
            : IsTaxInclusive
                ? RoundMoney(discountedAmount * TaxRatePercent / (100m + TaxRatePercent))
                : RoundMoney(discountedAmount * TaxRatePercent / 100m);
        TotalAmount = IsTaxInclusive
            ? discountedAmount
            : discountedAmount + TaxAmount;
    }

    private static decimal RoundMoney(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);

    private static decimal CalculateDiscountAmount(
        decimal amount,
        DiscountType discountType,
        decimal discountValue)
    {
        if (amount <= 0 || discountValue <= 0)
            return 0;

        decimal discountAmount;

        switch (discountType)
        {
            case DiscountType.Percentage:

                discountAmount =
                    amount * discountValue / 100m;

                break;

            case DiscountType.FixedAmount:

                discountAmount = discountValue;

                break;

            default:

                throw new ArgumentException(
                    "Invalid discount type.");
        }

        return Math.Min(discountAmount, amount);
    }


}
