using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

var settings = Settings.Parse(args);
using var client = new HttpClient
{
    BaseAddress = new Uri(settings.BaseUrl, UriKind.Absolute),
    Timeout = TimeSpan.FromSeconds(15)
};

for (var index = 0; index < settings.WarmupRequests; index++)
{
    using var response = await client.GetAsync(settings.Path);
    response.EnsureSuccessStatusCode();
}

var durations = new ConcurrentBag<double>();
var failures = new ConcurrentBag<string>();
using var gate = new SemaphoreSlim(settings.Concurrency);
var total = Stopwatch.StartNew();

await Task.WhenAll(Enumerable.Range(0, settings.Requests).Select(async requestNumber =>
{
    await gate.WaitAsync();
    try
    {
        var started = Stopwatch.GetTimestamp();
        using var response = await client.GetAsync(
            settings.Path,
            HttpCompletionOption.ResponseHeadersRead);
        durations.Add(Stopwatch.GetElapsedTime(started).TotalMilliseconds);
        if (!response.IsSuccessStatusCode)
            failures.Add($"Request {requestNumber} returned HTTP {(int)response.StatusCode}.");
    }
    catch (Exception exception)
    {
        failures.Add($"Request {requestNumber} failed: {exception.GetType().Name}.");
    }
    finally
    {
        gate.Release();
    }
}));

total.Stop();
var ordered = durations.Order().ToArray();
var p50 = Percentile(ordered, 0.50);
var p95 = Percentile(ordered, 0.95);
var p99 = Percentile(ordered, 0.99);
var passed = failures.IsEmpty && ordered.Length == settings.Requests &&
             p95 <= settings.P95LimitMilliseconds &&
             p99 <= settings.P99LimitMilliseconds;

var result = new
{
    generatedAtUtc = DateTimeOffset.UtcNow,
    target = new Uri(client.BaseAddress!, settings.Path).ToString(),
    settings.Requests,
    settings.Concurrency,
    settings.WarmupRequests,
    completedRequests = ordered.Length,
    failedRequests = failures.Count,
    elapsedMilliseconds = Math.Round(total.Elapsed.TotalMilliseconds, 2),
    requestsPerSecond = Math.Round(ordered.Length / Math.Max(total.Elapsed.TotalSeconds, 0.001), 2),
    latencyMilliseconds = new
    {
        p50 = Math.Round(p50, 2),
        p95 = Math.Round(p95, 2),
        p99 = Math.Round(p99, 2),
        maximum = Math.Round(ordered.LastOrDefault(), 2)
    },
    limitsMilliseconds = new
    {
        p95 = settings.P95LimitMilliseconds,
        p99 = settings.P99LimitMilliseconds
    },
    passed,
    failures = failures.Take(10).ToArray()
};

var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(json);

if (!string.IsNullOrWhiteSpace(settings.OutputPath))
{
    var fullPath = Path.GetFullPath(settings.OutputPath);
    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
    await File.WriteAllTextAsync(fullPath, json + Environment.NewLine);
}

return passed ? 0 : 1;

static double Percentile(double[] orderedValues, double percentile)
{
    if (orderedValues.Length == 0)
        return double.PositiveInfinity;

    var index = (int)Math.Ceiling(percentile * orderedValues.Length) - 1;
    return orderedValues[Math.Clamp(index, 0, orderedValues.Length - 1)];
}

file sealed record Settings(
    string BaseUrl,
    string Path,
    int Requests,
    int Concurrency,
    int WarmupRequests,
    double P95LimitMilliseconds,
    double P99LimitMilliseconds,
    string? OutputPath)
{
    public static Settings Parse(string[] args)
    {
        var values = args
            .Select((value, index) => (value, index))
            .Where(item => item.value.StartsWith("--", StringComparison.Ordinal))
            .ToDictionary(
                item => item.value,
                item => item.index + 1 < args.Length ? args[item.index + 1] : string.Empty,
                StringComparer.OrdinalIgnoreCase);

        return new Settings(
            Read(values, "--url", "http://127.0.0.1:5087"),
            Read(values, "--path", "/health/live"),
            ReadInt(values, "--requests", 500, 1, 100_000),
            ReadInt(values, "--concurrency", 20, 1, 1000),
            ReadInt(values, "--warmup", 20, 0, 10_000),
            ReadDouble(values, "--p95-ms", 250),
            ReadDouble(values, "--p99-ms", 500),
            values.GetValueOrDefault("--output"));
    }

    private static string Read(IReadOnlyDictionary<string, string> values, string key, string fallback) =>
        values.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : fallback;

    private static int ReadInt(IReadOnlyDictionary<string, string> values, string key, int fallback,
        int minimum, int maximum) =>
        values.TryGetValue(key, out var value) &&
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? Math.Clamp(parsed, minimum, maximum)
            : fallback;

    private static double ReadDouble(IReadOnlyDictionary<string, string> values, string key, double fallback) =>
        values.TryGetValue(key, out var value) &&
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) && parsed > 0
            ? parsed
            : fallback;
}
