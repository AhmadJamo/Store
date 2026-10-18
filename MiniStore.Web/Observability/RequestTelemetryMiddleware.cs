using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace MiniStore.Web.Observability;

public sealed class RequestTelemetryMiddleware(
    RequestDelegate next,
    ILogger<RequestTelemetryMiddleware> logger,
    IOptions<ObservabilityOptions> options)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!options.Value.Enabled)
        {
            await next(context);
            return;
        }

        var started = Stopwatch.GetTimestamp();
        MiniStoreTelemetry.ActiveRequests.Add(1);

        try
        {
            await next(context);

            if (context.Response.StatusCode >= StatusCodes.Status500InternalServerError)
            {
                MiniStoreTelemetry.RequestFailureCount.Add(1, CreateTags(context));
                if (options.Value.LogServerErrors)
                    logger.LogError(
                        "Request failed with status {StatusCode} for {Method} {Path}.",
                        context.Response.StatusCode,
                        context.Request.Method,
                        context.Request.Path);
            }
        }
        catch
        {
            MiniStoreTelemetry.RequestFailureCount.Add(1, CreateTags(context));
            throw;
        }
        finally
        {
            var elapsedMilliseconds = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
            var tags = CreateTags(context);
            MiniStoreTelemetry.RequestCount.Add(1, tags);
            MiniStoreTelemetry.RequestDuration.Record(elapsedMilliseconds, tags);
            MiniStoreTelemetry.ActiveRequests.Add(-1);

            if (elapsedMilliseconds >= Math.Max(1, options.Value.SlowRequestThresholdMilliseconds))
            {
                logger.LogWarning(
                    "Slow request detected: {ElapsedMilliseconds:F0} ms for {Method} {Path}.",
                    elapsedMilliseconds,
                    context.Request.Method,
                    context.Request.Path);
            }
        }
    }

    private static TagList CreateTags(HttpContext context) => new()
    {
        { "http.request.method", context.Request.Method },
        { "http.response.status_code", context.Response.StatusCode }
    };
}
