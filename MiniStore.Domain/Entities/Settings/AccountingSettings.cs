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
    public int? LandedCostClearingAccountId { get; private set; }
    public int? RealizedExchangeGainAccountId { get; private set; }
    public int? RealizedExchangeLossAccountId { get; private set; }
    private AccountingSettings() { }
    public AccountingSettings(int? purchaseDiscountAccountId, int? salesDiscountAccountId, int? salesRevenueAccountId, int? costOfSalesAccountId, int? goodsReceivedNotInvoicedAccountId = null, int? purchasePriceVarianceAccountId = null, int? landedCostClearingAccountId = null, int? realizedExchangeGainAccountId = null, int? realizedExchangeLossAccountId = null)
    { PurchaseDiscountAccountId = purchaseDiscountAccountId; SalesDiscountAccountId = salesDiscountAccountId; SalesRevenueAccountId = salesRevenueAccountId; CostOfSalesAccountId = costOfSalesAccountId; GoodsReceivedNotInvoicedAccountId = goodsReceivedNotInvoicedAccountId; PurchasePriceVarianceAccountId = purchasePriceVarianceAccountId; LandedCostClearingAccountId = landedCostClearingAccountId; RealizedExchangeGainAccountId = realizedExchangeGainAccountId; RealizedExchangeLossAccountId = realizedExchangeLossAccountId; }

    public void ConfigurePostingAccounts(
        int? purchaseDiscountAccountId,
        int? salesDiscountAccountId,
        int? salesRevenueAccountId,
        int? costOfSalesAccountId, int? goodsReceivedNotInvoicedAccountId, int? purchasePriceVarianceAccountId, int? landedCostClearingAccountId = null,
        int? realizedExchangeGainAccountId = null, int? realizedExchangeLossAccountId = null)
    {
        PurchaseDiscountAccountId = purchaseDiscountAccountId;
        SalesDiscountAccountId = salesDiscountAccountId;
        SalesRevenueAccountId = salesRevenueAccountId;
        CostOfSalesAccountId = costOfSalesAccountId;
        GoodsReceivedNotInvoicedAccountId = goodsReceivedNotInvoicedAccountId;
        PurchasePriceVarianceAccountId = purchasePriceVarianceAccountId;
        LandedCostClearingAccountId = landedCostClearingAccountId;
        RealizedExchangeGainAccountId = realizedExchangeGainAccountId;
        RealizedExchangeLossAccountId = realizedExchangeLossAccountId;
    }
}
