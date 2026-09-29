namespace MiniStore.Domain.Entities;

public class PurchaseReturnItem
{
    public int Id { get; private set; }
    public int PurchaseReturnId { get; private set; }
    public int PurchaseItemId { get; private set; }
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal OriginalInventoryAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal PayableAmount { get; private set; }
    public decimal RemovedInventoryCost { get; private set; }

    private PurchaseReturnItem() { }

    public PurchaseReturnItem(int purchaseItemId, int productId, int warehouseId, decimal quantity,
        decimal originalInventoryAmount, decimal discountAmount, decimal taxAmount,
        decimal payableAmount, decimal removedInventoryCost)
    {
        if (purchaseItemId <= 0 || productId <= 0 || warehouseId <= 0 || quantity <= 0)
            throw new ArgumentException("A valid purchase item, product, warehouse and quantity are required.");
        if (originalInventoryAmount < 0 || discountAmount < 0 || taxAmount < 0 || payableAmount < 0 || removedInventoryCost < 0)
            throw new ArgumentException("Purchase return amounts cannot be negative.");
        PurchaseItemId = purchaseItemId;
        ProductId = productId;
        WarehouseId = warehouseId;
        Quantity = quantity;
        OriginalInventoryAmount = RoundMoney(originalInventoryAmount);
        DiscountAmount = RoundMoney(discountAmount);
        TaxAmount = RoundMoney(taxAmount);
        PayableAmount = RoundMoney(payableAmount);
        RemovedInventoryCost = Math.Round(removedInventoryCost, 8, MidpointRounding.AwayFromZero);
    }

    private static decimal RoundMoney(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
