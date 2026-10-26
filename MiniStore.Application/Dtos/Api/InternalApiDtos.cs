namespace MiniStore.Application.DTOs.Api;

public sealed record InternalApiTenantDto(int Id, string Name, string Code);

public sealed record InternalApiUserDto(string Id, string? UserName, string? Email);

public sealed record InternalApiSessionDto(
    InternalApiUserDto User,
    InternalApiTenantDto Tenant,
    string Culture,
    string Direction,
    IReadOnlyList<string> Permissions);

public sealed record AntiforgeryTokenDto(string Token, string HeaderName);

public sealed record MutationProbeDto(bool Accepted, string CorrelationId);

public sealed class TenantLoginRequestDto
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(256)]
    public string UserName { get; init; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(1024)]
    public string Password { get; init; } = string.Empty;

    [System.ComponentModel.DataAnnotations.StringLength(128)]
    public string? CompanyCode { get; init; }
}

public sealed record TenantLoginResultDto(bool RequiresCompanyOnboarding);
