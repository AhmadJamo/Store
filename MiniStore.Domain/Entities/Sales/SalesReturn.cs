namespace MiniStore.Domain.Entities;

public class SalesReturn
{
    public int Id { get; private set; }
    public string ReturnNumber { get; private set; } = string.Empty;
    public int SaleId { get; private set; }
    public int WarehouseId { get; private set; }
    public int PaymentMethodId { get; private set; }
    public DateTime Date { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public decimal RevenueAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal RefundAmount { get; private set; }
    public decimal RestockedCostAmount { get; private set; }
    public List<SalesReturnItem> Items { get; private set; } = [];

    private SalesReturn() { }

    public SalesReturn(
        string returnNumber,
        int saleId,
        int warehouseId,
        int paymentMethodId,
        DateTime date,
        string reason)
    {
        if (string.IsNullOrWhiteSpace(returnNumber))
            throw new ArgumentException("Return number is required.");
        if (saleId <= 0 || warehouseId <= 0 || paymentMethodId <= 0)
            throw new ArgumentException("Sale, warehouse and payment method are required for a sales return.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("A return reason is required.");

        ReturnNumber = returnNumber.Trim();
        SaleId = saleId;
        WarehouseId = warehouseId;
        PaymentMethodId = paymentMethodId;
        Date = date;
        Reason = reason.Trim();
    }

    public void AddItem(SalesReturnItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (Items.Any(x => x.SaleItemId == item.SaleItemId))
            throw new InvalidOperationException("The same sale item cannot be returned more than once in one document.");

        Items.Add(item);
        RevenueAmount += item.RevenueAmount;
        DiscountAmount += item.DiscountAmount;
        TaxAmount += item.TaxAmount;
        RefundAmount += item.RefundAmount;
        RestockedCostAmount += item.RestockedCostAmount;
    }
}
