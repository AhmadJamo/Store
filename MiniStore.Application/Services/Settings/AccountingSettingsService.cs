using MiniStore.Application.Dtos.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class AccountingSettingsService(
    IAccountingSettingsRepository settingsRepository,
    IAccountRepository accountRepository,
    SemanticAuditEventService audit)
{
    public async Task<AccountingSettingsDto> GetAsync()
    {
        var settings = await settingsRepository.GetAsync();
        return settings is null ? new AccountingSettingsDto() : new AccountingSettingsDto
        {
            PurchaseDiscountAccountId = settings.PurchaseDiscountAccountId,
            SalesDiscountAccountId = settings.SalesDiscountAccountId,
            SalesRevenueAccountId = settings.SalesRevenueAccountId,
            CostOfSalesAccountId = settings.CostOfSalesAccountId,
            GoodsReceivedNotInvoicedAccountId = settings.GoodsReceivedNotInvoicedAccountId,
            PurchasePriceVarianceAccountId = settings.PurchasePriceVarianceAccountId,
            LandedCostClearingAccountId = settings.LandedCostClearingAccountId,
            RealizedExchangeGainAccountId = settings.RealizedExchangeGainAccountId,
            RealizedExchangeLossAccountId = settings.RealizedExchangeLossAccountId
        };
    }

    public async Task UpdateAsync(AccountingSettingsDto dto)
    {
        await EnsureAccountsExist(dto);

        var settings = await settingsRepository.GetAsync();
        var before = settings is null ? null : AuditValues(settings);
        if (settings is null)
        {
            await settingsRepository.AddAsync(new AccountingSettings(
                dto.PurchaseDiscountAccountId,
                dto.SalesDiscountAccountId,
                dto.SalesRevenueAccountId,
                dto.CostOfSalesAccountId, dto.GoodsReceivedNotInvoicedAccountId, dto.PurchasePriceVarianceAccountId, dto.LandedCostClearingAccountId,
                dto.RealizedExchangeGainAccountId, dto.RealizedExchangeLossAccountId));
        }
        else
        {
            settings.ConfigurePostingAccounts(
                dto.PurchaseDiscountAccountId,
                dto.SalesDiscountAccountId,
                dto.SalesRevenueAccountId,
                dto.CostOfSalesAccountId, dto.GoodsReceivedNotInvoicedAccountId, dto.PurchasePriceVarianceAccountId, dto.LandedCostClearingAccountId,
                dto.RealizedExchangeGainAccountId, dto.RealizedExchangeLossAccountId);
        }

        await audit.RecordTenantSuccessAsync(
            AuditEventCategory.Configuration,
            SemanticAuditEventTypes.AccountingSettingsChanged,
            "Update",
            "AccountingSettings",
            "Current",
            before,
            AuditValues(dto));
        await settingsRepository.SaveChangesAsync();
    }

    private static IReadOnlyDictionary<string, object?> AuditValues(AccountingSettings settings) =>
        AuditValues(new AccountingSettingsDto
        {
            PurchaseDiscountAccountId = settings.PurchaseDiscountAccountId,
            SalesDiscountAccountId = settings.SalesDiscountAccountId,
            SalesRevenueAccountId = settings.SalesRevenueAccountId,
            CostOfSalesAccountId = settings.CostOfSalesAccountId,
            GoodsReceivedNotInvoicedAccountId = settings.GoodsReceivedNotInvoicedAccountId,
            PurchasePriceVarianceAccountId = settings.PurchasePriceVarianceAccountId,
            LandedCostClearingAccountId = settings.LandedCostClearingAccountId,
            RealizedExchangeGainAccountId = settings.RealizedExchangeGainAccountId,
            RealizedExchangeLossAccountId = settings.RealizedExchangeLossAccountId
        });

    private static IReadOnlyDictionary<string, object?> AuditValues(AccountingSettingsDto value) =>
        new Dictionary<string, object?>
        {
            ["PurchaseDiscountAccountId"] = value.PurchaseDiscountAccountId,
            ["SalesDiscountAccountId"] = value.SalesDiscountAccountId,
            ["SalesRevenueAccountId"] = value.SalesRevenueAccountId,
            ["CostOfSalesAccountId"] = value.CostOfSalesAccountId,
            ["GoodsReceivedNotInvoicedAccountId"] = value.GoodsReceivedNotInvoicedAccountId,
            ["PurchasePriceVarianceAccountId"] = value.PurchasePriceVarianceAccountId,
            ["LandedCostClearingAccountId"] = value.LandedCostClearingAccountId,
            ["RealizedExchangeGainAccountId"] = value.RealizedExchangeGainAccountId,
            ["RealizedExchangeLossAccountId"] = value.RealizedExchangeLossAccountId
        };

    private async Task EnsureAccountsExist(AccountingSettingsDto dto)
    {
        var ids = new[]
        {
            dto.PurchaseDiscountAccountId, dto.SalesDiscountAccountId,
            dto.SalesRevenueAccountId, dto.CostOfSalesAccountId, dto.GoodsReceivedNotInvoicedAccountId,
            dto.PurchasePriceVarianceAccountId, dto.LandedCostClearingAccountId, dto.RealizedExchangeGainAccountId, dto.RealizedExchangeLossAccountId
        }.Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();

        foreach (var id in ids)
        {
            if (await accountRepository.GetByIdAsync(id) is null)
                throw new InvalidOperationException("A selected posting account was not found.");
        }
    }
}
