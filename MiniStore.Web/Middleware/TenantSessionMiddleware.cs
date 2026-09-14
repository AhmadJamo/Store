using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MiniStore.Application.Tenancy;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Web.Middleware;

public class TenantSessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        ITenantMembershipRepository memberships,
        SignInManager<IdentityUser> signInManager)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = signInManager.UserManager.GetUserId(context.User);
            var tenantClaim = context.User.FindFirst(TenantClaimTypes.TenantId);
            var hasValidTenantClaim =
                int.TryParse(tenantClaim?.Value, out var tenantId) &&
                tenantId > 0 &&
                !string.IsNullOrWhiteSpace(userId) &&
                await memberships.ExistsAsync(tenantId, userId);

            if (hasValidTenantClaim)
            {
                await next(context);
                return;
            }

            var membership = string.IsNullOrWhiteSpace(userId)
                ? null
                : await memberships.GetDefaultActiveAsync(userId);

            if (membership is null)
            {
                await signInManager.SignOutAsync();
                context.Response.Redirect("/Account/Login");
                return;
            }

            var replacementTenantClaim = new Claim(
                TenantClaimTypes.TenantId,
                membership.TenantId.ToString(System.Globalization.CultureInfo.InvariantCulture));
            if (tenantClaim is not null)
                context.User.Identities.First().RemoveClaim(tenantClaim);
            context.User.Identities.First().AddClaim(replacementTenantClaim);
            await signInManager.SignInWithClaimsAsync(
                await signInManager.UserManager.FindByIdAsync(userId!)
                    ?? throw new InvalidOperationException("Signed-in user no longer exists."),
                isPersistent: false,
                [replacementTenantClaim]);
        }

        await next(context);
    }
}
