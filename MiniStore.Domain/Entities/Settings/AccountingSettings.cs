namespace MiniStore.Domain.Entities;

public class AccountingSettings
{
    public int Id { get; private set; }
    public int SingletonKey { get; private set; } = 1;
    public int? PurchaseDiscountAccountId { get; private set; }
    public int? SalesDiscountAccountId { get; private set; }
    public int? SalesRevenueAccountId { get; private set; }
    public int? CostOfSalesAccountId { get; private set; }
    public int? GoodsReceivedNotInvoicedAccountId { get; private set; }
    public int? PurchasePriceVarianceAccountId { get; private set; }
    private AccountingSettings() { }
    public AccountingSettings(int? purchaseDiscountAccountId, int? salesDiscountAccountId, int? salesRevenueAccountId, int? costOfSalesAccountId, int? goodsReceivedNotInvoicedAccountId = null, int? purchasePriceVarianceAccountId = null)
    { PurchaseDiscountAccountId = purchaseDiscountAccountId; SalesDiscountAccountId = salesDiscountAccountId; SalesRevenueAccountId = salesRevenueAccountId; CostOfSalesAccountId = costOfSalesAccountId; GoodsReceivedNotInvoicedAccountId = goodsReceivedNotInvoicedAccountId; PurchasePriceVarianceAccountId = purchasePriceVarianceAccountId; }

    public void ConfigurePostingAccounts(
        int? purchaseDiscountAccountId,
        int? salesDiscountAccountId,
        int? salesRevenueAccountId,
        int? costOfSalesAccountId, int? goodsReceivedNotInvoicedAccountId, int? purchasePriceVarianceAccountId)
    {
        PurchaseDiscountAccountId = purchaseDiscountAccountId;
        SalesDiscountAccountId = salesDiscountAccountId;
        SalesRevenueAccountId = salesRevenueAccountId;
        CostOfSalesAccountId = costOfSalesAccountId;
        GoodsReceivedNotInvoicedAccountId = goodsReceivedNotInvoicedAccountId;
        PurchasePriceVarianceAccountId = purchasePriceVarianceAccountId;
    }
}
