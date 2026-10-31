using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Analytics;

public enum AnalyticsValueFormat { Number = 1, Money = 2, Percent = 3, Text = 4 }

public sealed class AnalyticsCenterQueryDto
{
    public int? WorkspaceId { get; set; }
    public string? ModuleKey { get; set; }
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
    public string? Dimension { get; set; }
    public AnalyticsComparisonMode? ComparisonMode { get; set; }
    public Dictionary<string, string> Filters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed record AnalyticsModuleDefinitionDto(string Key, string Title, string TitleArabic,
    string Description, string DescriptionArabic, string Permission,
    IReadOnlyList<AnalyticsDimensionDefinitionDto> Dimensions,
    IReadOnlyList<AnalyticsFilterDefinitionDto> Filters,
    IReadOnlyList<AnalyticsWidgetDefinitionDto> Widgets);
public sealed record AnalyticsDimensionDefinitionDto(string Key, string Label, string LabelArabic);
public sealed record AnalyticsFilterDefinitionDto(string Key, string Label, string LabelArabic,
    string Type, IReadOnlyList<AnalyticsFilterOptionDto> Options);
public sealed record AnalyticsFilterOptionDto(string Value, string Label);
public sealed record AnalyticsWidgetDefinitionDto(string Key, string Title, string TitleArabic,
    AnalyticsVisualizationType DefaultVisualization, AnalyticsValueFormat ValueFormat,
    int DefaultColumnSpan, string Description, string DescriptionArabic);

public sealed class AnalyticsDatasetDto
{
    public string ModuleKey { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string TitleArabic { get; init; } = string.Empty;
    public List<AnalyticsWidgetDataDto> Widgets { get; init; } = [];
}

public sealed class AnalyticsWidgetDataDto
{
    public string Key { get; init; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TitleArabic { get; set; } = string.Empty;
    public string? Definition { get; init; }
    public string? DefinitionArabic { get; init; }
    public AnalyticsVisualizationType Visualization { get; set; }
    public AnalyticsValueFormat ValueFormat { get; init; }
    public decimal? Value { get; init; }
    public string? TextValue { get; init; }
    public string? SecondaryText { get; init; }
    public string? SecondaryTextArabic { get; init; }
    public decimal? ComparisonValue { get; set; }
    public decimal? ComparisonDeltaPercent { get; set; }
    public DateOnly? ComparisonFrom { get; set; }
    public DateOnly? ComparisonTo { get; set; }
    public int Sequence { get; set; }
    public int ColumnSpan { get; set; } = 1;
    public int RowSpan { get; set; } = 1;
    public int TopN { get; set; } = 10;
    public bool ShowLegend { get; set; } = true;
    public bool ShowComparison { get; set; }
    public decimal? TargetValue { get; set; }
    public string? AccentColor { get; set; }
    public List<AnalyticsPointDto> Points { get; init; } = [];
    public List<string> Columns { get; init; } = [];
    public List<AnalyticsRowDto> Rows { get; init; } = [];
}
public sealed record AnalyticsPointDto(string Label, decimal Value, string? LinkController = null, int? LinkId = null);
public sealed record AnalyticsRowDto(IReadOnlyList<string> Cells, string? LinkController = null, int? LinkId = null);

public sealed class AnalyticsCenterPageDto
{
    public AnalyticsCenterQueryDto Query { get; init; } = new();
    public AnalyticsDatasetDto Dataset { get; init; } = new();
    public AnalyticsWorkspaceDto? Workspace { get; init; }
    public List<AnalyticsWorkspaceDto> Workspaces { get; init; } = [];
    public List<AnalyticsModuleDefinitionDto> Modules { get; init; } = [];
    public AnalyticsModuleDefinitionDto Module { get; init; } = null!;
    public int LayoutColumns { get; init; } = 4;
    public int RefreshIntervalMinutes { get; init; }
    public bool ShowDefinitions { get; init; } = true;
    public AnalyticsFinancialPreviewDto FinancialPreview { get; init; } = new();
}

public sealed class AnalyticsFinancialPreviewDto
{
    public string ProviderKey { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public string SourceArabic { get; init; } = string.Empty;
    public string Currency { get; init; } = string.Empty;
    public int DecimalPlaces { get; init; }
    public DateOnly From { get; init; }
    public DateOnly To { get; init; }
    public DateTime GeneratedAtUtc { get; init; }
    public bool ValuesAreServerAuthored { get; init; } = true;
    public List<AnalyticsFinancialPreviewItemDto> Items { get; init; } = [];
}

public sealed record AnalyticsFinancialPreviewItemDto(string Key, string Title, string TitleArabic,
    decimal Value, string Definition, string DefinitionArabic);

public sealed record AnalyticsWorkspaceDto(int Id, string Name, string? NameArabic, string ModuleKey,
    AnalyticsWorkspaceVisibility Visibility, bool IsDefault, int LayoutColumns, int DefaultPeriodDays,
    AnalyticsComparisonMode ComparisonMode, int RefreshIntervalMinutes, bool ShowDefinitions, byte[] RowVersion);

public sealed class CreateAnalyticsWorkspaceDto
{
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [StringLength(150)] public string? NameArabic { get; set; }
    [StringLength(1000)] public string? Description { get; set; }
    [Required, StringLength(50)] public string ModuleKey { get; set; } = string.Empty;
    public AnalyticsWorkspaceVisibility Visibility { get; set; } = AnalyticsWorkspaceVisibility.Personal;
    public bool IsDefault { get; set; }
    [Range(1, 4)] public int LayoutColumns { get; set; } = 4;
    [Range(1, 731)] public int DefaultPeriodDays { get; set; } = 90;
    public AnalyticsComparisonMode ComparisonMode { get; set; } = AnalyticsComparisonMode.PreviousPeriod;
    public int RefreshIntervalMinutes { get; set; }
    public bool ShowDefinitions { get; set; } = true;
    public List<CreateAnalyticsWidgetDto> Widgets { get; set; } = [];
}

public sealed class CreateAnalyticsWidgetDto
{
    public string ModuleKey { get; set; } = string.Empty;
    public string WidgetKey { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
    [Range(1, 1000)] public int Sequence { get; set; }
    public AnalyticsVisualizationType Visualization { get; set; }
    [StringLength(150)] public string? TitleOverride { get; set; }
    [StringLength(150)] public string? TitleArabicOverride { get; set; }
    [Range(1, 4)] public int ColumnSpan { get; set; } = 1;
    [Range(1, 4)] public int RowSpan { get; set; } = 1;
    [Range(1, 100)] public int TopN { get; set; } = 10;
    public bool ShowLegend { get; set; } = true;
    public bool ShowComparison { get; set; }
    public decimal? TargetValue { get; set; }
    [RegularExpression("^#[0-9A-Fa-f]{6}$")] public string? AccentColor { get; set; }
}

public sealed class AnalyticsWorkspaceCreatePageDto
{
    public CreateAnalyticsWorkspaceDto Form { get; init; } = new();
    public List<AnalyticsModuleDefinitionDto> Modules { get; init; } = [];
}

public sealed class AnalyticsWorkspaceAccessDto
{
    public int WorkspaceId { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool CanManage { get; init; }
    public byte[] RowVersion { get; init; } = [];
    public List<AnalyticsDelegationDto> Delegations { get; init; } = [];
    public List<AnalyticsPrincipalOptionDto> Users { get; init; } = [];
    public List<AnalyticsPrincipalOptionDto> Roles { get; init; } = [];
}

public sealed record AnalyticsPrincipalOptionDto(string Id, string DisplayName);

public sealed record AnalyticsDelegationDto(int Id, AnalyticsDelegationTargetType TargetType,
    string? TargetUserId, int? TargetRoleId, DateTime StartsAtUtc, DateTime? ExpiresAtUtc,
    string CreatedByUserId, DateTime CreatedAtUtc, string? RevokedByUserId, DateTime? RevokedAtUtc,
    bool IsActive);

public sealed class CreateAnalyticsDelegationDto
{
    public AnalyticsDelegationTargetType TargetType { get; set; }
    [StringLength(450)] public string? TargetUserId { get; set; }
    public int? TargetRoleId { get; set; }
    public DateTime? StartsAtUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    [Required] public byte[] RowVersion { get; set; } = [];
}

public sealed class RevokeAnalyticsDelegationDto
{
    [Range(1, int.MaxValue)] public int DelegationId { get; set; }
    [Required] public byte[] RowVersion { get; set; } = [];
}
