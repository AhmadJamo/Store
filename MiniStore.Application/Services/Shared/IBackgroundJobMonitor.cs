using MiniStore.Application.Dtos.Shared;

namespace MiniStore.Application.Services;

public interface IBackgroundJobMonitor
{
    void Register(string jobKey, string titleKey, string permission, TimeSpan expectedInterval, bool enabled = true);
    void Started(string jobKey);
    void Succeeded(string jobKey);
    void Failed(string jobKey);
    void Disabled(string jobKey);
    IReadOnlyList<BackgroundJobStatusDto> Snapshot(DateTime utcNow);
}
