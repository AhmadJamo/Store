using Microsoft.EntityFrameworkCore;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Web.Configuration;

public static class PersistenceExtensions
{
    public static IServiceCollection AddMiniStorePersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AuditSaveChangesInterceptor>();
        services.AddDbContext<AppDbContext>((provider, options) =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(provider.GetRequiredService<AuditSaveChangesInterceptor>()));
        return services;
    }
}
