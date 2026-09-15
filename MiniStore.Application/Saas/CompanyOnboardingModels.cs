using MiniStore.Domain.Entities;

namespace MiniStore.Application.Saas;

public record CompanyOnboardingDto(
    int TenantId,
    string TenantName,
    CompanyOnboardingStatus Status,
    IReadOnlyList<BusinessTypeOptionDto> BusinessTypes);

public record BusinessTypeOptionDto(
    BusinessType Value,
    string NameEnglish,
    string NameArabic,
    string DescriptionEnglish,
    string DescriptionArabic,
    bool RecommendedPos);

public record CompleteCompanyOnboardingCommand(
    BusinessType BusinessType,
    string CountryCode,
    string Currency,
    UiLanguage Language,
    int FiscalYearStartMonth,
    bool IsTaxRegistered,
    decimal DefaultTaxRate,
    bool UsePos,
    InventoryControlMode InventoryControlMode);

public interface ICompanyOnboardingService
{
    Task<CompanyOnboardingDto> GetAsync(int tenantId);
    Task<bool> IsResolvedAsync(int tenantId);
    Task CompleteAsync(int tenantId, string userId, CompleteCompanyOnboardingCommand command);
    Task SkipAsync(int tenantId, string userId);
}
