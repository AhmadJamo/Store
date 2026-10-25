using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Web.Localization;
using MiniStore.Web.Guidance;
using MiniStore.Web.Observability;
using Microsoft.Extensions.Localization;
using MiniStore.Web.Middleware.Api;
using MiniStore.Web.Presentation.Cutover;

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
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.Name = "MiniStore.Antiforgery";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        });
        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Extensions["correlationId"] = context.HttpContext.TraceIdentifier;
        });
        services.AddOpenApi("internal-v1", options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                foreach (var path in document.Paths.Keys
                             .Where(path => !path.StartsWith("/api/v1", StringComparison.OrdinalIgnoreCase))
                             .ToList())
                    document.Paths.Remove(path);
                document.Info.Title = "MiniStore Internal API";
                document.Info.Version = "v1";
                return Task.CompletedTask;
            });
        });
        services.Configure<ObservabilityOptions>(
            configuration.GetSection(ObservabilityOptions.SectionName));
        services.AddOptions<PresentationCutoverOptions>()
            .Bind(configuration.GetSection(PresentationCutoverOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.AngularBasePath) &&
                           options.AngularBasePath.StartsWith('/') &&
                           !options.AngularBasePath.StartsWith("//", StringComparison.Ordinal) &&
                           options.AngularBasePath.All(character =>
                               char.IsLetterOrDigit(character) || character is '/' or '-' or '_'),
                "PresentationCutover:AngularBasePath must be a local absolute path.")
            .ValidateOnStart();
        services.AddHealthChecks()
            .AddCheck("application", () =>
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(),
                tags: ["live"])
            .AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);
        services.AddScoped<WorkflowCompletionFilter>();
        services.AddScoped<PresentationCutoverResolver>();
        services.AddScoped<PresentationCutoverFilter>();
        services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            options.Filters.Add<WorkflowCompletionFilter>();
            options.Filters.Add<PresentationCutoverFilter>();
        })
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
                return InternalApiProblemFactory.Validation(
                    context.HttpContext,
                    context.ModelState,
                    localizer["One or more validation errors occurred."]);
            };
        });

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
            options.OnRejected = async (context, _) =>
            {
                var retryAfterSeconds = context.Lease.TryGetMetadata(
                    System.Threading.RateLimiting.MetadataName.RetryAfter, out var retryAfter)
                    ? Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds))
                    : 60;

                context.HttpContext.Response.Headers.RetryAfter = retryAfterSeconds.ToString(
                    System.Globalization.CultureInfo.InvariantCulture);
                var path = context.HttpContext.Request.Path;
                if (path.StartsWithSegments("/api/supplier"))
                {
                    context.HttpContext.Response.StatusCode=StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsJsonAsync(new{error="Supplier API rate limit exceeded.",retryAfterSeconds});
                    return;
                }
                if (path.StartsWithSegments("/supplier-portal"))
                {
                    context.HttpContext.Response.StatusCode=StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsync("Supplier portal rate limit exceeded. Please retry later.");
                    return;
                }
                var target = path.StartsWithSegments("/Account/Register")
                    ? $"/Account/Register?rateLimited=true&retryAfterSeconds={retryAfterSeconds}"
                    : path.StartsWithSegments("/platform/account/login")
                        ? $"/platform/account/login?rateLimited=true&retryAfterSeconds={retryAfterSeconds}"
                        : $"/Account/Login?rateLimited=true&retryAfterSeconds={retryAfterSeconds}";
                context.HttpContext.Response.Redirect(target);
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
            options.AddPolicy("supplier-portal", context =>
                System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
                    Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(context.Request.RouteValues["token"]?.ToString()??context.Connection.RemoteIpAddress?.ToString()??"unknown"))),
                    _=>new System.Threading.RateLimiting.FixedWindowRateLimiterOptions{PermitLimit=30,Window=TimeSpan.FromMinutes(1),QueueLimit=0}));
            options.AddPolicy("supplier-api", context =>
                System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
                    Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(context.Request.Headers["X-Supplier-Api-Key"].ToString() is {Length:>0} key?key:context.Connection.RemoteIpAddress?.ToString()??"unknown"))),
                    _=>new System.Threading.RateLimiting.FixedWindowRateLimiterOptions{PermitLimit=60,Window=TimeSpan.FromMinutes(1),QueueLimit=0}));
        });
    }
}
