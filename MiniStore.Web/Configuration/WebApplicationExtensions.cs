using MiniStore.Infrastructure.Persistence;
using MiniStore.Web.Middleware;

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

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseMiddleware<TenantSessionMiddleware>();
        app.UseMiddleware<SubscriptionAccessMiddleware>();
        app.UseRequestLocalization();
        app.UseAuthorization();
        app.MapStaticAssets();
        app.MapControllers();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
        return app;
    }
}
