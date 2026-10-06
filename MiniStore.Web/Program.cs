using MiniStore.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

if (OperatingSystem.IsWindows())
{
    builder.Logging.AddFilter<Microsoft.Extensions.Logging.EventLog.EventLogLoggerProvider>(
        category: null,
        LogLevel.None);
}

builder.Services
    .AddMiniStorePresentation(builder.Configuration)
    .AddMiniStorePersistence(builder.Configuration)
    .AddMiniStoreSecurity()
    .AddMiniStoreBusinessServices();

var app = builder.Build();

if (!await app.InitializeMiniStoreAsync(builder.Configuration))
{
    return;
}

app.UseMiniStorePipeline();
app.Run();
