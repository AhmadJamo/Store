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

    public DateTime Date { get; private set; }

    public string? Notes { get; private set; }

    public decimal Subtotal { get; private set; }

    public DiscountType InvoiceDiscountType { get; private set; }

    public decimal InvoiceDiscountValue { get; private set; }

    public decimal InvoiceDiscountAmount { get; private set; }

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
        string? notes = null)
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

        InvoiceNumber = invoiceNumber;
        WarehouseId = warehouseId;
        Date = date;
        Channel = channel;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
        Notes = notes;

        Items = new List<SaleItem>();

        Subtotal = 0;
        InvoiceDiscountType = DiscountType.Percentage;
        InvoiceDiscountValue = 0;
        InvoiceDiscountAmount = 0;
        TotalAmount = 0;
    }

    public void AddItem(SaleItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

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

        TotalAmount = Subtotal - InvoiceDiscountAmount;
    }

    public void RemoveInvoiceDiscount()
    {
        InvoiceDiscountType = DiscountType.Percentage;
        InvoiceDiscountValue = 0;
        InvoiceDiscountAmount = 0;

        TotalAmount = Subtotal;
    }

    private void RecalculateTotal()
    {
        Subtotal = Items.Sum(x => x.Total);

        InvoiceDiscountAmount = CalculateDiscountAmount(
            Subtotal,
            InvoiceDiscountType,
            InvoiceDiscountValue);

        TotalAmount = Subtotal - InvoiceDiscountAmount;
    }

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
