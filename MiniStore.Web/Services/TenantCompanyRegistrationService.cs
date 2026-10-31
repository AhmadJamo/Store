using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MiniStore.Application.Saas;
using MiniStore.Application.Tenancy;

namespace MiniStore.Web.Services;

public sealed class TenantCompanyRegistrationService(
    ISaasOnboardingService onboarding,
    UserManager<IdentityUser> users,
    SignInManager<IdentityUser> signInManager)
{
    public async Task RegisterAndSignInAsync(RegisterCompanyCommand command)
    {
        var result = await onboarding.RegisterAsync(command);
        var user = await users.FindByIdAsync(result.UserId)
            ?? throw new InvalidOperationException("The registered user was not found.");

        await signInManager.SignInWithClaimsAsync(
            user,
            false,
            [new Claim(
                TenantClaimTypes.TenantId,
                result.TenantId.ToString(System.Globalization.CultureInfo.InvariantCulture))]);
    }
}
