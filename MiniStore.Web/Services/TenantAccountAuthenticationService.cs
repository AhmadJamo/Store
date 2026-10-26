using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MiniStore.Application.Saas;
using MiniStore.Application.Services;
using MiniStore.Application.Tenancy;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Web.Services;

public enum TenantLoginStatus
{
    Succeeded,
    InvalidCredentials,
    CompanyAccessDenied
}

public sealed record TenantLoginOutcome(
    TenantLoginStatus Status,
    bool RequiresCompanyOnboarding = false);

public sealed class TenantAccountAuthenticationService(
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager,
    ITenantMembershipRepository memberships,
    ICompanyOnboardingService companyOnboarding,
    ISecurityAuditEventWriter securityAudit,
    IAuditRequestContext auditRequest)
{
    public async Task<TenantLoginOutcome> SignInAsync(
        string userName,
        string password,
        string? companyCode)
    {
        var user = await userManager.FindByNameAsync(userName.Trim());
        var result = user is null
            ? Microsoft.AspNetCore.Identity.SignInResult.Failed
            : await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            await RecordDeniedAsync(result.IsLockedOut ? "AccountLocked" : "InvalidCredentials",
                result.IsLockedOut ? AuditEventSeverity.Critical : AuditEventSeverity.Warning);
            return new TenantLoginOutcome(TenantLoginStatus.InvalidCredentials);
        }

        var membership = string.IsNullOrWhiteSpace(companyCode)
            ? await memberships.GetDefaultActiveAsync(user!.Id)
            : await memberships.GetActiveByCompanyCodeAsync(user!.Id, companyCode);
        if (membership is null)
        {
            await RecordDeniedAsync("CompanyAccessDenied", AuditEventSeverity.Warning);
            return new TenantLoginOutcome(TenantLoginStatus.CompanyAccessDenied);
        }

        await signInManager.SignInWithClaimsAsync(
            user,
            isPersistent: false,
            [new Claim(
                TenantClaimTypes.TenantId,
                membership.TenantId.ToString(System.Globalization.CultureInfo.InvariantCulture))]);

        return new TenantLoginOutcome(
            TenantLoginStatus.Succeeded,
            !await companyOnboarding.IsResolvedAsync(membership.TenantId));
    }

    public Task SignOutAsync() => signInManager.SignOutAsync();

    private Task RecordDeniedAsync(string reasonCode, AuditEventSeverity severity) =>
        securityAudit.RecordAsync(new IndependentAuditEventWriteRequest
        {
            Scope = AuditEventScope.System,
            Category = AuditEventCategory.Security,
            Severity = severity,
            Outcome = AuditEventOutcome.Denied,
            EventType = SemanticAuditEventTypes.TenantLoginFailed,
            Action = "Login",
            ActorType = AuditActorType.External,
            ActorId = "anonymous",
            SourceChannel = AuditSourceChannel.Web,
            CorrelationId = auditRequest.CorrelationId,
            ResourceType = "TenantSession",
            ResourceId = "login",
            ReasonCode = reasonCode,
            RequestPathTemplate = auditRequest.RequestPathTemplate,
            SourceIpMasked = auditRequest.SourceIpMasked,
            UserAgentHash = auditRequest.UserAgentHash
        }, CancellationToken.None);
}
