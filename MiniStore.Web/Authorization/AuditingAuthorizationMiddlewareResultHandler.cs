using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;

namespace MiniStore.Web.Authorization;

public sealed class AuditingAuthorizationMiddlewareResultHandler(
    ISecurityAuditEventWriter audit,
    IAuditRequestContext requestContext) : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler fallback = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden && context.User.Identity?.IsAuthenticated == true)
        {
            var permission = policy.Requirements.OfType<PermissionRequirement>().FirstOrDefault()?.Permission;
            var platform = policy.AuthenticationSchemes.Contains(PlatformAuthentication.Scheme, StringComparer.Ordinal);
            var tenantId = platform
                ? null
                : context.RequestServices.GetRequiredService<MiniStore.Domain.Interfaces.ITenantContext>().TenantId;
            if (!platform && !tenantId.HasValue)
            {
                await fallback.HandleAsync(next, context, policy, authorizeResult);
                return;
            }
            await audit.RecordAsync(new IndependentAuditEventWriteRequest
            {
                TenantId = tenantId,
                Scope = platform ? AuditEventScope.Platform : AuditEventScope.Tenant,
                Category = AuditEventCategory.Security,
                Severity = AuditEventSeverity.Warning,
                Outcome = AuditEventOutcome.Denied,
                EventType = SemanticAuditEventTypes.PrivilegedActionDenied,
                Action = "Authorize",
                ActorType = AuditActorType.User,
                ActorId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "authenticated",
                SourceChannel = platform ? AuditSourceChannel.Platform : requestContext.SourceChannel,
                CorrelationId = requestContext.CorrelationId,
                ResourceType = permission is null ? "AuthorizationPolicy" : "Permission",
                ResourceId = permission ?? "privileged-endpoint",
                ReasonCode = "PermissionDenied",
                RequestPathTemplate = requestContext.RequestPathTemplate,
                SourceIpMasked = requestContext.SourceIpMasked,
                UserAgentHash = requestContext.UserAgentHash
            }, CancellationToken.None);
        }

        await fallback.HandleAsync(next, context, policy, authorizeResult);
    }
}
