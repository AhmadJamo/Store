using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Saas;

public record PlanCardDto(int Id, string Code, string NameEnglish, string NameArabic, string DescriptionEnglish, string DescriptionArabic, decimal MonthlyPrice, decimal AnnualPrice, string Currency, IReadOnlyDictionary<string, int?> Limits, IReadOnlyCollection<string> Features);
public record CreatePlanDto(string Code, string NameEnglish, string NameArabic, string DescriptionEnglish, string DescriptionArabic, decimal MonthlyPrice, decimal AnnualPrice, string Currency, int MaxUsers, int MaxWarehouses, int MaxBranches, int MaxPosTerminals, int MaxProducts);
public record CreatePromotionCodeDto(string Code, decimal DiscountPercentage, DateTime StartsAt, DateTime EndsAt, int? MaximumRedemptions, int? PlanId);
public record CompanySubscriptionDto(int TenantId, string Name, string Slug, bool IsActive, int? PlanId, string PlanName, SubscriptionStatus? Status, DateTime? PeriodEnd, int UserCount);
public record UpdateCompanySubscriptionDto(int TenantId, int PlanId, SubscriptionStatus Status, DateTime PeriodEnd, bool CompanyIsActive);

public class PlatformSaasService(ISaasRepository repository)
{
    public async Task<IReadOnlyList<PlanCardDto>> GetPublicPlansAsync() => Map(await repository.GetPublicPlansAsync());
    public async Task<IReadOnlyList<PlanCardDto>> GetPlansAsync() => Map(await repository.GetPlansAsync());
    public Task<IReadOnlyList<PromotionCode>> GetPromotionCodesAsync() => repository.GetPromotionCodesAsync();
    public async Task<IReadOnlyList<CompanySubscriptionDto>> GetCompaniesAsync()
    {
        var plans = await repository.GetPlansAsync();
        var result = new List<CompanySubscriptionDto>();
        foreach (var tenant in await repository.GetTenantsAsync())
        {
            var subscription = await repository.GetSubscriptionAsync(tenant.Id);
            var plan = subscription is null ? null : plans.FirstOrDefault(x => x.Id == subscription.PlanId);
            result.Add(new(tenant.Id, tenant.Name, tenant.Slug, tenant.IsActive, subscription?.PlanId, plan?.NameEnglish ?? "No plan", subscription?.Status, subscription?.CurrentPeriodEnd, await repository.CountUsersAsync(tenant.Id)));
        }
        return result;
    }
    public async Task UpdateCompanySubscriptionAsync(UpdateCompanySubscriptionDto dto)
    {
        var tenant = await repository.GetTenantAsync(dto.TenantId) ?? throw new InvalidOperationException("Company not found.");
        var subscription = await repository.GetSubscriptionAsync(dto.TenantId) ?? throw new InvalidOperationException("Subscription not found.");
        _ = await repository.GetPlanAsync(dto.PlanId) ?? throw new InvalidOperationException("Plan not found.");
        tenant.SetActive(dto.CompanyIsActive);
        subscription.ChangePlan(dto.PlanId, subscription.BillingCycle, DateTime.UtcNow, dto.PeriodEnd.ToUniversalTime());
        subscription.SetStatus(dto.Status);
        await repository.SaveChangesAsync();
    }
    public async Task CreatePlanAsync(CreatePlanDto dto)
    {
        var plan = new Plan(dto.Code, dto.NameEnglish, dto.NameArabic, dto.MonthlyPrice, dto.AnnualPrice, dto.Currency, 100);
        plan.Update(dto.Code, dto.NameEnglish, dto.NameArabic, dto.MonthlyPrice, dto.AnnualPrice, dto.Currency, 100, true, true, dto.DescriptionEnglish, dto.DescriptionArabic);
        plan.SetLimit(SaasLimitKeys.Users, dto.MaxUsers); plan.SetLimit(SaasLimitKeys.Warehouses, dto.MaxWarehouses);
        plan.SetLimit(SaasLimitKeys.Branches, dto.MaxBranches); plan.SetLimit(SaasLimitKeys.PosTerminals, dto.MaxPosTerminals); plan.SetLimit(SaasLimitKeys.Products, dto.MaxProducts);
        await repository.AddPlanAsync(plan); await repository.SaveChangesAsync();
    }
    public async Task CreatePromotionCodeAsync(CreatePromotionCodeDto dto) { await repository.AddPromotionCodeAsync(new PromotionCode(dto.Code, dto.DiscountPercentage, dto.StartsAt.ToUniversalTime(), dto.EndsAt.ToUniversalTime(), dto.MaximumRedemptions, dto.PlanId)); await repository.SaveChangesAsync(); }
    private static IReadOnlyList<PlanCardDto> Map(IReadOnlyList<Plan> plans) => plans.Select(x => new PlanCardDto(x.Id, x.Code, x.NameEnglish, x.NameArabic, x.DescriptionEnglish, x.DescriptionArabic, x.MonthlyPrice, x.AnnualPrice, x.Currency, x.Limits.ToDictionary(l => l.Key, l => l.Value), x.Features.Where(f => f.IsEnabled).Select(f => f.Key).ToArray())).ToList();
}
