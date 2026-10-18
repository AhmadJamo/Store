using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Web.Middleware;
using MiniStore.Web.Observability;

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

        app.UseMiddleware<RequestTelemetryMiddleware>();
        app.UseHttpsRedirection();
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
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
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
