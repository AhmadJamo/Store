namespace MiniStore.Application.Dtos.Settings;
using MiniStore.Domain.Entities;

public class AccountingSettingsDto
{
    public int? PurchaseDiscountAccountId { get; set; }
    public int? SalesDiscountAccountId { get; set; }
    public int? SalesRevenueAccountId { get; set; }
    public int? CostOfSalesAccountId { get; set; }
    public int? GoodsReceivedNotInvoicedAccountId { get; set; }
    public int? PurchasePriceVarianceAccountId { get; set; }
    public int? LandedCostClearingAccountId { get; set; }
    public int? RealizedExchangeGainAccountId { get; set; }
    public int? RealizedExchangeLossAccountId { get; set; }
    public CustomerReceivableAccountMode? CustomerReceivableAccountMode { get; set; }
    public int? CustomerReceivableControlAccountId { get; set; }
    public int? CustomerReceivableParentAccountId { get; set; }
    public string CustomerAccountCodePrefix { get; set; } = "CUST-";
    public DateOnly CustomerPolicyEffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public SalesInvoiceQuantityPolicy SalesInvoiceQuantityPolicy { get; set; } = SalesInvoiceQuantityPolicy.DeliveredQuantity;
}
