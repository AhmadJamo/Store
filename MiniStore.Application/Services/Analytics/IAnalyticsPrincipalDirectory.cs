namespace MiniStore.Application.Services;

public interface IAnalyticsPrincipalDirectory
{
    Task<IReadOnlyList<AnalyticsPrincipalOption>> GetUsersAsync(int tenantId);
    Task<IReadOnlyList<AnalyticsPrincipalOption>> GetRolesAsync(int tenantId);
}

public sealed record AnalyticsPrincipalOption(string Id, string DisplayName);
