using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Web.Localization;

namespace MiniStore.Web.Configuration;

public static class WebPresentationExtensions
{
    public static IServiceCollection AddMiniStorePresentation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        services.AddControllersWithViews(options =>
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(SupportedUiCultures.English);
            options.SupportedCultures = SupportedUiCultures.All.ToList();
            options.SupportedUICultures = SupportedUiCultures.All.ToList();
            options.RequestCultureProviders =
            [
                new CookieRequestCultureProvider(),
                new DatabaseRequestCultureProvider()
            ];
        });

        AddRateLimiting(services, configuration);
        return services;
    }

    private static void AddRateLimiting(IServiceCollection services, IConfiguration configuration)
    {
        var registrationPermitLimit = Math.Clamp(
            configuration.GetValue("RateLimiting:Registration:PermitLimit", 20), 5, 1000);
        var registrationWindowMinutes = Math.Clamp(
            configuration.GetValue("RateLimiting:Registration:WindowMinutes", 60), 1, 1440);

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = (context, _) =>
            {
                var retryAfterSeconds = context.Lease.TryGetMetadata(
                    System.Threading.RateLimiting.MetadataName.RetryAfter, out var retryAfter)
                    ? Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds))
                    : 60;

                context.HttpContext.Response.Headers.RetryAfter = retryAfterSeconds.ToString(
                    System.Globalization.CultureInfo.InvariantCulture);
                var path = context.HttpContext.Request.Path;
                var target = path.StartsWithSegments("/Account/Register")
                    ? $"/Account/Register?rateLimited=true&retryAfterSeconds={retryAfterSeconds}"
                    : path.StartsWithSegments("/platform/account/login")
                        ? $"/platform/account/login?rateLimited=true&retryAfterSeconds={retryAfterSeconds}"
                        : $"/Account/Login?rateLimited=true&retryAfterSeconds={retryAfterSeconds}";
                context.HttpContext.Response.Redirect(target);
                return ValueTask.CompletedTask;
            };

            options.AddPolicy("login", context =>
                System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));
            options.AddPolicy("registration", context =>
                System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
                    {
                        PermitLimit = registrationPermitLimit,
                        Window = TimeSpan.FromMinutes(registrationWindowMinutes),
                        QueueLimit = 0
                    }));
        });
    }
}
