namespace MiniStore.Web.Observability;

public sealed class ObservabilityOptions
{
    public const string SectionName = "Observability";

    public bool Enabled { get; init; } = true;
    public bool LogServerErrors { get; init; } = true;
    public double SlowRequestThresholdMilliseconds { get; init; } = 1000;
}
