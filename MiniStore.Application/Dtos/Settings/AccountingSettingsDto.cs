namespace MiniStore.Application.Dtos.Settings;

public class AccountingSettingsDto
{
    public int? PurchaseDiscountAccountId { get; set; }
    public int? SalesDiscountAccountId { get; set; }
    public int? SalesRevenueAccountId { get; set; }
    public int? CostOfSalesAccountId { get; set; }
}
