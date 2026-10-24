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
