using System.ComponentModel.DataAnnotations;
using MiniStore.Application.Saas;

namespace MiniStore.Application.DTOs.Api;

public sealed record CompanyRegistrationPageDto(
    IReadOnlyList<PlanCardDto> Plans);

public sealed record RegisterCompanyRequestDto(
    [property: Required, StringLength(200)] string CompanyName,
    [property: Required, StringLength(63, MinimumLength = 3), RegularExpression("^[a-z0-9][a-z0-9-]{1,61}[a-z0-9]$")] string CompanySlug,
    [property: Required, StringLength(100)] string OwnerUsername,
    [property: Required, EmailAddress, StringLength(256)] string OwnerEmail,
    [property: Required, StringLength(1024, MinimumLength = 6)] string Password,
    [property: Range(1, int.MaxValue)] int PlanId);

public sealed record CompanyRegistrationResultDto(bool RequiresCompanyOnboarding);
