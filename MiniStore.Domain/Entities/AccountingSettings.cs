namespace MiniStore.Domain.Entities;

public class AccountingSettings
{
    public int Id { get; private set; }
    public int SingletonKey { get; private set; } = 1;
    public int? PurchaseDiscountAccountId { get; private set; }
    public int? SalesDiscountAccountId { get; private set; }
    public int? SalesRevenueAccountId { get; private set; }
    public int? CostOfSalesAccountId { get; private set; }
    private AccountingSettings() { }
    public AccountingSettings(int? purchaseDiscountAccountId, int? salesDiscountAccountId, int? salesRevenueAccountId, int? costOfSalesAccountId)
    { PurchaseDiscountAccountId = purchaseDiscountAccountId; SalesDiscountAccountId = salesDiscountAccountId; SalesRevenueAccountId = salesRevenueAccountId; CostOfSalesAccountId = costOfSalesAccountId; }

    public void ConfigurePostingAccounts(
        int? purchaseDiscountAccountId,
        int? salesDiscountAccountId,
        int? salesRevenueAccountId,
        int? costOfSalesAccountId)
    {
        PurchaseDiscountAccountId = purchaseDiscountAccountId;
        SalesDiscountAccountId = salesDiscountAccountId;
        SalesRevenueAccountId = salesRevenueAccountId;
        CostOfSalesAccountId = costOfSalesAccountId;
    }
}
