using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MiniStore.Infrastructure.Authorization;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Web.Authorization;

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
        });

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        return services;
    }
}
