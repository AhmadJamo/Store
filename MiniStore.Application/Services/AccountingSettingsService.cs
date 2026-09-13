using MiniStore.Application.Dtos.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class AccountingSettingsService(
    IAccountingSettingsRepository settingsRepository,
    IAccountRepository accountRepository)
{
    public async Task<AccountingSettingsDto> GetAsync()
    {
        var settings = await settingsRepository.GetAsync();
        return settings is null ? new AccountingSettingsDto() : new AccountingSettingsDto
        {
            PurchaseDiscountAccountId = settings.PurchaseDiscountAccountId,
            SalesDiscountAccountId = settings.SalesDiscountAccountId,
            SalesRevenueAccountId = settings.SalesRevenueAccountId,
            CostOfSalesAccountId = settings.CostOfSalesAccountId
        };
    }

    public async Task UpdateAsync(AccountingSettingsDto dto)
    {
        await EnsureAccountsExist(dto);

        var settings = await settingsRepository.GetAsync();
        if (settings is null)
        {
            await settingsRepository.AddAsync(new AccountingSettings(
                dto.PurchaseDiscountAccountId,
                dto.SalesDiscountAccountId,
                dto.SalesRevenueAccountId,
                dto.CostOfSalesAccountId));
        }
        else
        {
            settings.ConfigurePostingAccounts(
                dto.PurchaseDiscountAccountId,
                dto.SalesDiscountAccountId,
                dto.SalesRevenueAccountId,
                dto.CostOfSalesAccountId);
        }

        await settingsRepository.SaveChangesAsync();
    }

    private async Task EnsureAccountsExist(AccountingSettingsDto dto)
    {
        var ids = new[]
        {
            dto.PurchaseDiscountAccountId, dto.SalesDiscountAccountId,
            dto.SalesRevenueAccountId, dto.CostOfSalesAccountId
        }.Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();

        foreach (var id in ids)
        {
            if (await accountRepository.GetByIdAsync(id) is null)
                throw new InvalidOperationException("A selected posting account was not found.");
        }
    }
}
