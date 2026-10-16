using Microsoft.EntityFrameworkCore;
using MiniStore.Application.Services;
using MiniStore.Infrastructure.Persistence;
using MiniStore.Infrastructure.Services.Security;
using MiniStore.Infrastructure.Repositories;
using MiniStore.Web.Services;

namespace MiniStore.Web.Configuration;

public static class PersistenceExtensions
{
    public static IServiceCollection AddMiniStorePersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AuditEventOptions>(configuration.GetSection(AuditEventOptions.SectionName));
        services.Configure<AuditRetentionOptions>(configuration.GetSection(AuditRetentionOptions.SectionName));
        var retention = configuration.GetSection(AuditRetentionOptions.SectionName).Get<AuditRetentionOptions>() ?? new();
        var defaultConnection = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("DefaultConnection is required.");
        services.AddSingleton(new AuditDatabaseAccessOptions
        {
            ReadConnectionString = configuration.GetConnectionString("AuditReadConnection") ?? defaultConnection,
            MaintenanceConnectionString = configuration.GetConnectionString("AuditMaintenanceConnection") ?? defaultConnection
        });
        services.AddSingleton(new AuditRetentionPolicy(retention.Enabled,
            retention.ProtectedRetentionYears, retention.OperationalRetentionYears,
            retention.RequestEvidenceDays, Math.Clamp(retention.BatchSize, 1, 5000)));
        services.AddScoped<IAuditEventRedactor, AuditEventRedactor>();
        services.AddScoped<IAuditEventIntegrityService, AuditEventIntegrityService>();
        services.AddScoped<IAuditEventWriter, AuditEventWriter>();
        services.AddSingleton<ISecurityAuditEventWriter, SecurityAuditEventWriter>();
        services.AddScoped<IAuditRequestContext, HttpAuditRequestContext>();
        services.AddScoped<SemanticAuditEventService>();
        services.AddScoped<IAuditEventReadRepository, AuditEventReadRepository>();
        services.AddScoped<AuditEventReadService>();
        services.AddScoped<IAuditRetentionRepository, AuditRetentionRepository>();
        services.AddScoped<IAuditArchiveStore, FileAuditArchiveStore>();
        services.AddScoped<AuditRetentionService>();
        services.AddHostedService<AuditRetentionWorker>();
        services.AddScoped<AuditSaveChangesInterceptor>();
        services.AddDbContext<AppDbContext>((provider, options) =>
            options.UseSqlServer(defaultConnection)
                .AddInterceptors(provider.GetRequiredService<AuditSaveChangesInterceptor>()));
        return services;
    }
}
