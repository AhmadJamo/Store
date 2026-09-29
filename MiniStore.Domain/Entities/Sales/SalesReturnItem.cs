namespace MiniStore.Domain.Entities;

public class SalesReturnItem
{
    public int Id { get; private set; }
    public int SalesReturnId { get; private set; }
    public int SaleItemId { get; private set; }
    public int ProductId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal RevenueAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal RefundAmount { get; private set; }
    public bool Restocked { get; private set; }
    public decimal RestockedCostAmount { get; private set; }

    private SalesReturnItem() { }

    public SalesReturnItem(
        int saleItemId,
        int productId,
        decimal quantity,
        decimal revenueAmount,
        decimal discountAmount,
        decimal taxAmount,
        decimal refundAmount,
        bool restocked,
        decimal restockedCostAmount)
    {
        if (saleItemId <= 0 || productId <= 0 || quantity <= 0)
            throw new ArgumentException("A valid sale item, product and quantity are required.");
        if (revenueAmount < 0 || discountAmount < 0 || taxAmount < 0 ||
            refundAmount < 0 || restockedCostAmount < 0)
            throw new ArgumentException("Sales return amounts cannot be negative.");
        if (!restocked && restockedCostAmount != 0)
            throw new ArgumentException("A non-restocked return cannot carry inventory cost.");

        SaleItemId = saleItemId;
        ProductId = productId;
        Quantity = quantity;
        RevenueAmount = Round(revenueAmount);
        DiscountAmount = Round(discountAmount);
        TaxAmount = Round(taxAmount);
        RefundAmount = Round(refundAmount);
        Restocked = restocked;
        RestockedCostAmount = Math.Round(restockedCostAmount, 8, MidpointRounding.AwayFromZero);
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
