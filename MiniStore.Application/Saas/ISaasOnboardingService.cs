namespace MiniStore.Application.Saas;

public record RegisterCompanyCommand(
    string OwnerUsername,
    string OwnerEmail,
    string Password,
    string CompanyName,
    string CompanySlug,
    int PlanId);

public record RegisterCompanyResult(string UserId, int TenantId, string TenantName);

public interface ISaasOnboardingService
{
    Task<RegisterCompanyResult> RegisterAsync(RegisterCompanyCommand command);
}
