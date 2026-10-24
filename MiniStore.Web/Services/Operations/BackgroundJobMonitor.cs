using System.Collections.Concurrent;
using System.Diagnostics;
using MiniStore.Application.Dtos.Shared;
using MiniStore.Application.Services;

namespace MiniStore.Web.Services;

public sealed class BackgroundJobMonitor : IBackgroundJobMonitor
{
    private sealed class State
    {
        public required string JobKey { get; init; }
        public required string TitleKey { get; init; }
        public required string Permission { get; init; }
        public required TimeSpan ExpectedInterval { get; init; }
        public bool Enabled { get; set; }
        public DateTime? LastStartedAtUtc { get; set; }
        public DateTime? LastCompletedAtUtc { get; set; }
        public long? LastDurationMilliseconds { get; set; }
        public string Status { get; set; } = "waiting";
        public long? StopwatchTimestamp { get; set; }
        public object SyncRoot { get; } = new();
    }

    private readonly ConcurrentDictionary<string, State> states = new(StringComparer.OrdinalIgnoreCase);

    public void Register(string jobKey, string titleKey, string permission, TimeSpan expectedInterval, bool enabled = true) =>
        states.AddOrUpdate(jobKey,
            _ => new State { JobKey = jobKey, TitleKey = titleKey, Permission = permission, ExpectedInterval = expectedInterval, Enabled = enabled, Status = enabled ? "waiting" : "disabled" },
            (_, current) => { lock (current.SyncRoot) { current.Enabled = enabled; current.Status = enabled ? current.Status : "disabled"; } return current; });

    public void Started(string jobKey) => Update(jobKey, state =>
    {
        state.Enabled = true;
        state.Status = "running";
        state.LastStartedAtUtc = DateTime.UtcNow;
        state.StopwatchTimestamp = Stopwatch.GetTimestamp();
    });

    public void Succeeded(string jobKey) => Complete(jobKey, "succeeded");
    public void Failed(string jobKey) => Complete(jobKey, "failed");
    public void Disabled(string jobKey) => Update(jobKey, state => { state.Enabled = false; state.Status = "disabled"; });

    public IReadOnlyList<BackgroundJobStatusDto> Snapshot(DateTime utcNow) => states.Values.Select(state =>
    {
        lock (state.SyncRoot)
        {
            var next = state.LastStartedAtUtc?.Add(state.ExpectedInterval);
            var sla = !state.Enabled ? "disabled"
                : state.Status == "failed" ? "breached"
                : next.HasValue && utcNow > next.Value.Add(state.ExpectedInterval) ? "breached"
                : next.HasValue && utcNow > next.Value ? "at-risk"
                : "healthy";
            return new BackgroundJobStatusDto(state.JobKey, state.TitleKey, state.Permission, state.Status, sla,
                Math.Max(1, (int)Math.Ceiling(state.ExpectedInterval.TotalMinutes)), state.LastStartedAtUtc,
                state.LastCompletedAtUtc, state.LastDurationMilliseconds, next);
        }
    }).ToList();

    private void Complete(string jobKey, string status) => Update(jobKey, state =>
    {
        state.Status = status;
        state.LastCompletedAtUtc = DateTime.UtcNow;
        state.LastDurationMilliseconds = state.StopwatchTimestamp.HasValue
            ? (long)Stopwatch.GetElapsedTime(state.StopwatchTimestamp.Value).TotalMilliseconds
            : null;
        state.StopwatchTimestamp = null;
    });

    private void Update(string jobKey, Action<State> update)
    {
        if (!states.TryGetValue(jobKey, out var state)) return;
        lock (state.SyncRoot) update(state);
    }
}
