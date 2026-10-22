using MiniStore.Application.Dtos.Analytics;

namespace MiniStore.Application.Services;

public interface IAnalyticsModuleProvider
{
    AnalyticsModuleDefinitionDto Definition { get; }
    Task<AnalyticsDatasetDto> GetAsync(AnalyticsCenterQueryDto query);
}
