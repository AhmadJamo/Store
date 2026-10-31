using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Web.Middleware;
using MiniStore.Web.Observability;
using MiniStore.Web.Middleware.Api;

namespace MiniStore.Web.Configuration;

public static class WebApplicationExtensions
{
    public static async Task<bool> InitializeMiniStoreAsync(
        this WebApplication app,
        IConfiguration configuration)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            await IdentitySeeder.SeedAsync(scope.ServiceProvider, configuration);
            await PermissionSeeder.SeedAsync(scope.ServiceProvider);
            await PlatformOperatorSeeder.SeedAsync(scope.ServiceProvider, configuration);
            return true;
        }
        catch (Exception exception)
        {
            app.Logger.LogCritical(
                exception,
                "MiniStore could not complete startup database initialization.");
            Environment.ExitCode = 1;
            return false;
        }
    }

    public static WebApplication UseMiniStorePipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseMiddleware<InternalApiExceptionMiddleware>();
        app.UseMiddleware<RequestTelemetryMiddleware>();
        app.UseHttpsRedirection();
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = context =>
            {
                var fileName = context.File.Name;
                context.Context.Response.Headers.CacheControl =
                    string.Equals(fileName, "index.html", StringComparison.OrdinalIgnoreCase)
                        ? "no-store, no-cache"
                        : "public,max-age=31536000,immutable";
            }
        });
        app.UseRouting();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseMiddleware<TenantSessionMiddleware>();
        app.UseMiddleware<SubscriptionAccessMiddleware>();
        app.UseRequestLocalization();
        app.UseAuthorization();
        app.MapStaticAssets();
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("live"),
            AllowCachingResponses = false,
            ResponseWriter = WriteHealthResponseAsync
        }).AllowAnonymous();
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("ready"),
            AllowCachingResponses = false,
            ResponseWriter = WriteHealthResponseAsync
        }).AllowAnonymous();
        app.MapControllers();
        app.MapOpenApi("/openapi/{documentName}.json").RequireAuthorization();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
        app.MapFallbackToFile("app/{*path:nonfile}", "app/index.html",
            new StaticFileOptions
            {
                OnPrepareResponse = context =>
                    context.Context.Response.Headers.CacheControl = "no-store, no-cache"
            }).AllowAnonymous();
        return app;
    }

    private static Task WriteHealthResponseAsync(HttpContext context,
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.Headers.CacheControl = "no-store, no-cache";
        return context.Response.WriteAsJsonAsync(new
        {
            status = report.Status.ToString()
        });
    }
}
