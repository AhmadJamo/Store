using MiniStore.Application.Dtos.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class AccountingSettingsService(
    IAccountingSettingsRepository settingsRepository,
    IAccountRepository accountRepository,
    ISaleRepository sales,
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
            RealizedExchangeLossAccountId = settings.RealizedExchangeLossAccountId,
            CustomerReceivableAccountMode=settings.CustomerReceivableMode, CustomerReceivableControlAccountId=settings.CustomerReceivableControlAccountId,
            CustomerReceivableParentAccountId=settings.CustomerReceivableParentAccountId,CustomerAccountCodePrefix=settings.CustomerAccountCodePrefix,
            CustomerPolicyEffectiveFrom=settings.CustomerPolicyEffectiveFrom??DateOnly.FromDateTime(DateTime.Today)
        };
    }

    public async Task UpdateAsync(AccountingSettingsDto dto)
    {
        await EnsureAccountsExist(dto);

        var settings = await settingsRepository.GetAsync();
        if(settings?.CustomerReceivableMode is { } current&&dto.CustomerReceivableAccountMode is { } requested&&current!=requested&&await sales.GetAllAsync() is {Count:>0})
            throw new InvalidOperationException("Customer receivable policy has dependent sales and requires the controlled transition workflow.");
        var before = settings is null ? null : AuditValues(settings);
        if (settings is null)
        {
            settings=new AccountingSettings(
                dto.PurchaseDiscountAccountId,
                dto.SalesDiscountAccountId,
                dto.SalesRevenueAccountId,
                dto.CostOfSalesAccountId, dto.GoodsReceivedNotInvoicedAccountId, dto.PurchasePriceVarianceAccountId, dto.LandedCostClearingAccountId,
                dto.RealizedExchangeGainAccountId, dto.RealizedExchangeLossAccountId);
            await settingsRepository.AddAsync(settings);
        }
        if(dto.CustomerReceivableAccountMode is { } mode) settings.ConfigureCustomerReceivables(mode,dto.CustomerReceivableControlAccountId,dto.CustomerReceivableParentAccountId,dto.CustomerAccountCodePrefix,dto.CustomerPolicyEffectiveFrom);
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
            RealizedExchangeLossAccountId = settings.RealizedExchangeLossAccountId,
            CustomerReceivableAccountMode=settings.CustomerReceivableMode,CustomerReceivableControlAccountId=settings.CustomerReceivableControlAccountId,
            CustomerReceivableParentAccountId=settings.CustomerReceivableParentAccountId,CustomerAccountCodePrefix=settings.CustomerAccountCodePrefix,
            CustomerPolicyEffectiveFrom=settings.CustomerPolicyEffectiveFrom??DateOnly.FromDateTime(DateTime.Today)
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
            ["RealizedExchangeLossAccountId"] = value.RealizedExchangeLossAccountId,
            ["CustomerReceivableAccountMode"]=value.CustomerReceivableAccountMode,["CustomerReceivableControlAccountId"]=value.CustomerReceivableControlAccountId,
            ["CustomerReceivableParentAccountId"]=value.CustomerReceivableParentAccountId,["CustomerAccountCodePrefix"]=value.CustomerAccountCodePrefix,["CustomerPolicyEffectiveFrom"]=value.CustomerPolicyEffectiveFrom
        };

    private async Task EnsureAccountsExist(AccountingSettingsDto dto)
    {
        var ids = new[]
        {
            dto.PurchaseDiscountAccountId, dto.SalesDiscountAccountId,
            dto.SalesRevenueAccountId, dto.CostOfSalesAccountId, dto.GoodsReceivedNotInvoicedAccountId,
            dto.PurchasePriceVarianceAccountId, dto.LandedCostClearingAccountId, dto.RealizedExchangeGainAccountId, dto.RealizedExchangeLossAccountId,
            dto.CustomerReceivableControlAccountId,dto.CustomerReceivableParentAccountId
        }.Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();

        foreach (var id in ids)
        {
            if (await accountRepository.GetByIdAsync(id) is null)
                throw new InvalidOperationException("A selected posting account was not found.");
        }
        if(dto.CustomerReceivableAccountMode==CustomerReceivableAccountMode.SharedControlAccount&&dto.CustomerReceivableControlAccountId is { } controlId&&await accountRepository.GetByIdAsync(controlId) is not {IsActive:true,Type:AccountType.Asset,ParentAccountId:not null})
            throw new InvalidOperationException("The shared customer receivable control account must be an active asset subaccount.");
        if(dto.CustomerReceivableAccountMode==CustomerReceivableAccountMode.AutomaticIndividualAccount&&dto.CustomerReceivableParentAccountId is { } parentId&&await accountRepository.GetByIdAsync(parentId) is not {IsActive:true,Type:AccountType.Asset})
            throw new InvalidOperationException("The automatic customer account parent must be an active asset account.");
    }
}
