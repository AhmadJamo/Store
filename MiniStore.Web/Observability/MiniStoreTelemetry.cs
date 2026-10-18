using System.Diagnostics.Metrics;

namespace MiniStore.Web.Observability;

public static class MiniStoreTelemetry
{
    public const string MeterName = "MiniStore.Web";

    private static readonly Meter Meter = new(MeterName);

    public static readonly Counter<long> RequestCount =
        Meter.CreateCounter<long>("ministore.http.server.requests", unit: "{request}");

    public static readonly Counter<long> RequestFailureCount =
        Meter.CreateCounter<long>("ministore.http.server.failures", unit: "{request}");

    public static readonly Histogram<double> RequestDuration =
        Meter.CreateHistogram<double>("ministore.http.server.duration", unit: "ms");

    public static readonly UpDownCounter<long> ActiveRequests =
        Meter.CreateUpDownCounter<long>("ministore.http.server.active_requests", unit: "{request}");

    public static readonly Counter<long> DatabaseHealthProbeCount =
        Meter.CreateCounter<long>("ministore.database.health.probes", unit: "{probe}");

    public static readonly Counter<long> DatabaseHealthFailureCount =
        Meter.CreateCounter<long>("ministore.database.health.failures", unit: "{probe}");
}
