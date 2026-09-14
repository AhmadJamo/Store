using MiniStore.Application.Saas;
using MiniStore.Application.Tenancy;

namespace MiniStore.Web.Middleware;

public class SubscriptionAccessMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, EntitlementService entitlements)
    {
        var path = context.Request.Path;
        var bypass = path.StartsWithSegments("/subscription") || path.StartsWithSegments("/Account") ||
            path.StartsWithSegments("/platform") || path.StartsWithSegments("/pricing") || path == "/" ||
            path.StartsWithSegments("/css") || path.StartsWithSegments("/js") || path.StartsWithSegments("/lib");
        if (!bypass && context.User.Identity?.IsAuthenticated == true && context.User.HasClaim(x => x.Type == TenantClaimTypes.TenantId))
        {
            try { await entitlements.EnsureSubscriptionAsync(); }
            catch (InvalidOperationException) { context.Response.Redirect("/subscription"); return; }
        }
        await next(context);
    }
}
