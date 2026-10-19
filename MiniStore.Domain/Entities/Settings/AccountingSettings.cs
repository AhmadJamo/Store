namespace MiniStore.Domain.Entities;

public enum CustomerReceivableAccountMode { SharedControlAccount = 1, AutomaticIndividualAccount = 2, ManualSelection = 3 }

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
    public CustomerReceivableAccountMode? CustomerReceivableMode { get; private set; }
    public int? CustomerReceivableControlAccountId { get; private set; }
    public int? CustomerReceivableParentAccountId { get; private set; }
    public string CustomerAccountCodePrefix { get; private set; } = "CUST-";
    public DateOnly? CustomerPolicyEffectiveFrom { get; private set; }
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

    public void ConfigureCustomerReceivables(CustomerReceivableAccountMode mode,int? controlAccountId,int? parentAccountId,string codePrefix,DateOnly effectiveFrom)
    {
        if(!Enum.IsDefined(mode)) throw new ArgumentException("Customer receivable account mode is invalid.");
        codePrefix=(codePrefix??"").Trim().ToUpperInvariant();
        if(mode==CustomerReceivableAccountMode.SharedControlAccount&&controlAccountId is null) throw new ArgumentException("A shared customer receivable control account is required.");
        if(mode==CustomerReceivableAccountMode.AutomaticIndividualAccount&&parentAccountId is null) throw new ArgumentException("A parent receivable account is required for automatic customer accounts.");
        if(mode==CustomerReceivableAccountMode.AutomaticIndividualAccount&&(codePrefix.Length is < 1 or > 20||codePrefix.Any(char.IsWhiteSpace))) throw new ArgumentException("Customer account code prefix must contain 1 to 20 characters without spaces.");
        CustomerReceivableMode=mode; CustomerReceivableControlAccountId=controlAccountId; CustomerReceivableParentAccountId=parentAccountId; CustomerAccountCodePrefix=codePrefix; CustomerPolicyEffectiveFrom=effectiveFrom;
    }
}
