using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Services;

public record PlatformAuthenticationAttempt(ClaimsPrincipal? Principal, bool IsLockedOut)
{
    public bool Succeeded => Principal is not null;
}

public sealed class PlatformAccountAuthenticationService(
    UserManager<IdentityUser> users,
    SignInManager<IdentityUser> signInManager,
    ISaasRepository saas,
    ISecurityAuditEventWriter securityAudit,
    IAuditRequestContext auditRequest)
{
    public async Task<PlatformAuthenticationAttempt> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        var user = string.IsNullOrWhiteSpace(username)
            ? null
            : await users.FindByNameAsync(username.Trim());
        var op = user is null ? null : await saas.GetOperatorAsync(user.Id);
        var result = user is null || op is null
            ? Microsoft.AspNetCore.Identity.SignInResult.Failed
            : await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            await RecordFailureAsync(result.IsLockedOut, cancellationToken);
            return new PlatformAuthenticationAttempt(null, result.IsLockedOut);
        }

        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, user!.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Role, op!.Role.ToString())
        ], PlatformAuthentication.Scheme);
        return new PlatformAuthenticationAttempt(new ClaimsPrincipal(identity), false);
    }

    private Task RecordFailureAsync(bool lockedOut, CancellationToken cancellationToken) =>
        securityAudit.RecordAsync(new IndependentAuditEventWriteRequest
        {
            Scope = AuditEventScope.Platform,
            Category = AuditEventCategory.Security,
            Severity = lockedOut ? AuditEventSeverity.Critical : AuditEventSeverity.Warning,
            Outcome = AuditEventOutcome.Denied,
            EventType = SemanticAuditEventTypes.PlatformLoginFailed,
            Action = "Login",
            ActorType = AuditActorType.External,
            ActorId = "anonymous",
            SourceChannel = AuditSourceChannel.Platform,
            CorrelationId = auditRequest.CorrelationId,
            ResourceType = "PlatformSession",
            ResourceId = "login",
            ReasonCode = lockedOut ? "AccountLocked" : "InvalidCredentials",
            RequestPathTemplate = auditRequest.RequestPathTemplate,
            SourceIpMasked = auditRequest.SourceIpMasked,
            UserAgentHash = auditRequest.UserAgentHash
        }, cancellationToken);
}
