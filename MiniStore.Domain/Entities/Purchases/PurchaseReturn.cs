namespace MiniStore.Domain.Entities;

public class PurchaseReturn
{
    public int Id { get; private set; }
    public string ReturnNumber { get; private set; } = string.Empty;
    public int PurchaseId { get; private set; }
    public int SupplierId { get; private set; }
    public DateTime Date { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public decimal PayableAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal OriginalInventoryAmount { get; private set; }
    public decimal RemovedInventoryCost { get; private set; }
    public List<PurchaseReturnItem> Items { get; private set; } = [];

    private PurchaseReturn() { }

    public PurchaseReturn(string returnNumber, int purchaseId, int supplierId, DateTime date, string reason)
    {
        if (string.IsNullOrWhiteSpace(returnNumber)) throw new ArgumentException("Return number is required.");
        if (purchaseId <= 0 || supplierId <= 0) throw new ArgumentException("Purchase and supplier are required for a purchase return.");
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("A return reason is required.");
        ReturnNumber = returnNumber.Trim();
        PurchaseId = purchaseId;
        SupplierId = supplierId;
        Date = date;
        Reason = reason.Trim();
    }

    public void AddItem(PurchaseReturnItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (Items.Any(x => x.PurchaseItemId == item.PurchaseItemId))
            throw new InvalidOperationException("The same purchase item cannot be returned more than once in one document.");
        Items.Add(item);
        PayableAmount += item.PayableAmount;
        DiscountAmount += item.DiscountAmount;
        TaxAmount += item.TaxAmount;
        OriginalInventoryAmount += item.OriginalInventoryAmount;
        RemovedInventoryCost += item.RemovedInventoryCost;
    }
}
