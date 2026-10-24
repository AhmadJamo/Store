using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MiniStore.Infrastructure.Authorization;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Web.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Configuration;

public static class SecurityExtensions
{
    public static IServiceCollection AddMiniStoreSecurity(this IServiceCollection services)
    {
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddAuthentication().AddCookie(PlatformAuthentication.Scheme, options =>
        {
            options.Cookie.Name = "MiniStore.Platform";
            options.LoginPath = "/platform/account/login";
            options.AccessDeniedPath = "/platform/account/login";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.SlidingExpiration = false;
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.Events.OnRedirectToLogin = context =>
                WriteApiStatusOrRedirectAsync(context, StatusCodes.Status401Unauthorized);
            options.Events.OnRedirectToAccessDenied = context =>
                WriteApiStatusOrRedirectAsync(context, StatusCodes.Status403Forbidden);
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(PlatformAuthentication.Policy, policy =>
            {
                policy.AddAuthenticationSchemes(PlatformAuthentication.Scheme);
                policy.RequireAuthenticatedUser();
            });
            options.AddPolicy(PlatformAuthentication.ManagementPolicy, policy =>
            {
                policy.AddAuthenticationSchemes(PlatformAuthentication.Scheme);
                policy.RequireRole("Owner", "Administrator");
            });
            options.AddPolicy(PlatformAuthentication.BillingPolicy, policy =>
            {
                policy.AddAuthenticationSchemes(PlatformAuthentication.Scheme);
                policy.RequireRole("Owner", "Administrator", "Billing");
            });
            options.AddPolicy(PlatformAuthentication.AuditViewPolicy, policy =>
            {
                policy.AddAuthenticationSchemes(PlatformAuthentication.Scheme);
                policy.RequireRole("Owner", "Administrator", "Billing");
            });
            options.AddPolicy(PlatformAuthentication.AuditExportPolicy, policy =>
            {
                policy.AddAuthenticationSchemes(PlatformAuthentication.Scheme);
                policy.RequireRole("Owner", "Administrator");
            });
            options.AddPolicy(PlatformAuthentication.AuditSensitivePolicy, policy =>
            {
                policy.AddAuthenticationSchemes(PlatformAuthentication.Scheme);
                policy.RequireRole("Owner");
            });
            options.AddPolicy(PlatformAuthentication.AuditRetentionPolicy, policy =>
            {
                policy.AddAuthenticationSchemes(PlatformAuthentication.Scheme);
                policy.RequireRole("Owner");
            });
        });

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IAuthorizationMiddlewareResultHandler, AuditingAuthorizationMiddlewareResultHandler>();
        return services;
    }

    private static Task WriteApiStatusOrRedirectAsync(
        Microsoft.AspNetCore.Authentication.RedirectContext<Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions> context,
        int apiStatusCode)
    {
        if (!context.Request.Path.StartsWithSegments("/api/v1") &&
            !context.Request.Path.StartsWithSegments("/openapi"))
        {
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }

        return InternalApiProblemWriter.WriteAsync(
            context.HttpContext,
            apiStatusCode,
            apiStatusCode == StatusCodes.Status401Unauthorized
                ? "Authentication is required."
                : "Access is denied.");
    }
}
