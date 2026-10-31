namespace MiniStore.Application.Dtos.Shared;

public sealed record BackgroundJobStatusDto(
    string JobKey,
    string TitleKey,
    string Permission,
    string State,
    string SlaState,
    int ExpectedIntervalMinutes,
    DateTime? LastStartedAtUtc,
    DateTime? LastCompletedAtUtc,
    long? LastDurationMilliseconds,
    DateTime? NextExpectedAtUtc);

public sealed record AnalyticsRefreshPlanDto(
    int WorkspaceId,
    string Name,
    string? NameArabic,
    string ModuleKey,
    int RefreshIntervalMinutes,
    bool IsAutomaticDelivery);

public sealed record OperationsCenterDto(
    DateTime GeneratedAtUtc,
    IReadOnlyList<BackgroundJobStatusDto> Jobs,
    IReadOnlyList<AnalyticsRefreshPlanDto> RefreshPlans);
