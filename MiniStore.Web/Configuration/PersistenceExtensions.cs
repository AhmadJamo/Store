using Microsoft.EntityFrameworkCore;
using MiniStore.Application.Services;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Infrastructure.Services.Security;

namespace MiniStore.Web.Configuration;

public static class PersistenceExtensions
{
    public static IServiceCollection AddMiniStorePersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AuditEventOptions>(configuration.GetSection(AuditEventOptions.SectionName));
        services.AddScoped<IAuditEventRedactor, AuditEventRedactor>();
        services.AddScoped<IAuditEventIntegrityService, AuditEventIntegrityService>();
        services.AddScoped<IAuditEventWriter, AuditEventWriter>();
        services.AddScoped<AuditSaveChangesInterceptor>();
        services.AddDbContext<AppDbContext>((provider, options) =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(provider.GetRequiredService<AuditSaveChangesInterceptor>()));
        return services;
    }
}
