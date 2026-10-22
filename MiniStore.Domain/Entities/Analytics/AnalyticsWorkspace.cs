namespace MiniStore.Domain.Entities;

public enum AnalyticsWorkspaceVisibility { Personal = 1, Shared = 2 }
public enum AnalyticsVisualizationType { Metric = 1, Funnel = 2, Table = 3, Bar = 4, Line = 5, Donut = 6, Comparison = 7 }
public enum AnalyticsComparisonMode { None = 1, PreviousPeriod = 2, PreviousYear = 3 }

public sealed class AnalyticsWorkspace
{
    private AnalyticsWorkspace() { Name = ModuleKey = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? NameArabic { get; private set; }
    public string? Description { get; private set; }
    public string ModuleKey { get; private set; } = string.Empty;
    public AnalyticsWorkspaceVisibility Visibility { get; private set; }
    public string? OwnerUserId { get; private set; }
    public bool IsDefault { get; private set; }
    public int LayoutColumns { get; private set; }
    public int DefaultPeriodDays { get; private set; }
    public AnalyticsComparisonMode ComparisonMode { get; private set; }
    public int RefreshIntervalMinutes { get; private set; }
    public bool ShowDefinitions { get; private set; }
    public string CreatedByUserId { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<AnalyticsWorkspaceWidget> Widgets { get; private set; } = [];

    public AnalyticsWorkspace(string name, string? nameArabic, string? description, string moduleKey,
        AnalyticsWorkspaceVisibility visibility, string userId, bool isDefault, int layoutColumns,
        int defaultPeriodDays, AnalyticsComparisonMode comparisonMode, int refreshIntervalMinutes,
        bool showDefinitions)
    {
        Name = Required(name, 150, "Analytics workspace name");
        NameArabic = Optional(nameArabic, 150, "Arabic analytics workspace name");
        Description = Optional(description, 1000, "Analytics workspace description");
        ModuleKey = Required(moduleKey, 50, "Analytics module key").ToLowerInvariant();
        if (!Enum.IsDefined(visibility) || !Enum.IsDefined(comparisonMode)) throw new ArgumentException("Analytics workspace option is invalid.");
        if (layoutColumns is < 1 or > 4) throw new ArgumentException("Analytics workspace columns must be between 1 and 4.");
        if (defaultPeriodDays is < 1 or > 731) throw new ArgumentException("Analytics workspace default period must be between 1 and 731 days.");
        if (refreshIntervalMinutes is not (0 or 5 or 15 or 30 or 60)) throw new ArgumentException("Analytics workspace refresh interval is invalid.");
        CreatedByUserId = Required(userId, 450, "Analytics workspace creator");
        Visibility = visibility; OwnerUserId = visibility == AnalyticsWorkspaceVisibility.Personal ? CreatedByUserId : null;
        IsDefault = isDefault; LayoutColumns = layoutColumns; DefaultPeriodDays = defaultPeriodDays;
        ComparisonMode = comparisonMode; RefreshIntervalMinutes = refreshIntervalMinutes;
        ShowDefinitions = showDefinitions; CreatedAtUtc = DateTime.UtcNow;
    }

    public void AddWidget(AnalyticsWorkspaceWidget widget)
    {
        if (Widgets.Count >= 30) throw new InvalidOperationException("An analytics workspace cannot contain more than 30 widgets.");
        if (Widgets.Any(x => x.WidgetKey.Equals(widget.WidgetKey, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("An analytics widget can be added only once per workspace.");
        if (Widgets.Any(x => x.Sequence == widget.Sequence)) throw new InvalidOperationException("Analytics widget order must be unique.");
        Widgets.Add(widget);
    }

    public void ClearDefault() => IsDefault = false;
    private static string Required(string? value, int max, string label) { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{label} is required."); var x=value.Trim(); if(x.Length>max)throw new ArgumentException($"{label} cannot exceed {max} characters."); return x; }
    private static string? Optional(string? value, int max, string label) { if(string.IsNullOrWhiteSpace(value))return null;var x=value.Trim();if(x.Length>max)throw new ArgumentException($"{label} cannot exceed {max} characters.");return x; }
}

public sealed class AnalyticsWorkspaceWidget
{
    private AnalyticsWorkspaceWidget() { WidgetKey = string.Empty; }
    public int Id { get; private set; }
    public int AnalyticsWorkspaceId { get; private set; }
    public string WidgetKey { get; private set; } = string.Empty;
    public int Sequence { get; private set; }
    public AnalyticsVisualizationType Visualization { get; private set; }
    public string? TitleOverride { get; private set; }
    public string? TitleArabicOverride { get; private set; }
    public int ColumnSpan { get; private set; }
    public int RowSpan { get; private set; }
    public int TopN { get; private set; }
    public bool ShowLegend { get; private set; }
    public bool ShowComparison { get; private set; }
    public decimal? TargetValue { get; private set; }
    public string? AccentColor { get; private set; }

    public AnalyticsWorkspaceWidget(string widgetKey, int sequence, AnalyticsVisualizationType visualization,
        string? titleOverride, string? titleArabicOverride, int columnSpan, int rowSpan, int topN,
        bool showLegend, bool showComparison, decimal? targetValue, string? accentColor)
    {
        if (string.IsNullOrWhiteSpace(widgetKey) || widgetKey.Trim().Length > 100) throw new ArgumentException("Analytics widget key is required and cannot exceed 100 characters.");
        if (sequence is < 1 or > 1000) throw new ArgumentException("Analytics widget order must be between 1 and 1000.");
        if (!Enum.IsDefined(visualization)) throw new ArgumentException("Analytics widget visualization is invalid.");
        if (columnSpan is < 1 or > 4 || rowSpan is < 1 or > 4) throw new ArgumentException("Analytics widget span must be between 1 and 4.");
        if (topN is < 1 or > 100) throw new ArgumentException("Analytics widget row limit must be between 1 and 100.");
        WidgetKey = widgetKey.Trim().ToLowerInvariant(); Sequence = sequence; Visualization = visualization;
        TitleOverride = Optional(titleOverride, 150); TitleArabicOverride = Optional(titleArabicOverride, 150);
        ColumnSpan = columnSpan; RowSpan = rowSpan; TopN = topN; ShowLegend = showLegend;
        ShowComparison = showComparison; TargetValue = targetValue;
        AccentColor = string.IsNullOrWhiteSpace(accentColor) ? null : ValidateColor(accentColor);
    }

    private static string? Optional(string? value, int max) { if(string.IsNullOrWhiteSpace(value))return null;var x=value.Trim();if(x.Length>max)throw new ArgumentException($"Analytics widget title cannot exceed {max} characters.");return x; }
    private static string ValidateColor(string value) { var x=value.Trim().ToUpperInvariant();if(x.Length!=7||x[0]!='#'||x.Skip(1).Any(c=>!Uri.IsHexDigit(c)))throw new ArgumentException("Analytics widget accent color must use #RRGGBB format.");return x; }
}
