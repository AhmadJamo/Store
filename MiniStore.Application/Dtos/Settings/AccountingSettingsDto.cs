namespace MiniStore.Application.Dtos.Settings;
using MiniStore.Application.Dtos.Accounting;
using MiniStore.Domain.Entities;

public sealed class AccountingSettingsPageDto
{
    public AccountingSettingsDto Settings { get; init; } = new();
    public List<AccountDto> Accounts { get; init; } = [];
}

public sealed class BranchSalesAccountsPageDto
{
    public List<BranchSalesAccountDto> Branches { get; init; } = [];
    public List<AccountDto> Accounts { get; init; } = [];
}

public sealed class BranchSalesAccountDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public int? SalesRevenueAccountId { get; init; }
}

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
