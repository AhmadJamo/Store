using MiniStore.Application.Dtos.Analytics;
using MiniStore.Application.Permissions;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class AnalyticsCenterService(IEnumerable<IAnalyticsModuleProvider> moduleProviders,
    IAnalyticsWorkspaceRepository workspaces, IUnitOfWork unitOfWork, ICurrentUserService currentUser,
    IPermissionService permissionService)
{
    private Dictionary<string, IAnalyticsModuleProvider> Providers => moduleProviders.ToDictionary(x => x.Definition.Key, StringComparer.OrdinalIgnoreCase);

    public async Task<AnalyticsCenterPageDto> GetAsync(AnalyticsCenterQueryDto query)
    {
        var providers = await GetAuthorizedProvidersAsync();
        if (providers.Count == 0) throw new InvalidOperationException("No analytics modules are registered.");
        var available = await workspaces.GetAvailableAsync(currentUser.UserId);
        AnalyticsWorkspace? workspace = null;
        if (query.WorkspaceId.HasValue)
            workspace = available.SingleOrDefault(x => x.Id == query.WorkspaceId.Value) ?? throw new InvalidOperationException("Analytics workspace was not found or is not available.");
        else if (string.IsNullOrWhiteSpace(query.ModuleKey))
            workspace = available.FirstOrDefault(x => x.IsDefault) ?? available.FirstOrDefault();
        var moduleKey = workspace?.ModuleKey ?? query.ModuleKey ?? providers.Keys.Order().First();
        if (!providers.TryGetValue(moduleKey, out var provider)) throw new InvalidOperationException("Analytics module is not registered.");
        query.ModuleKey = provider.Definition.Key;
        var today = DateOnly.FromDateTime(DateTime.Today);
        query.To ??= today; query.From ??= today.AddDays(-(workspace?.DefaultPeriodDays ?? 90) + 1);
        ValidateDates(query);
        query.Dimension = provider.Definition.Dimensions.Any(x => x.Key.Equals(query.Dimension, StringComparison.OrdinalIgnoreCase))
            ? query.Dimension : provider.Definition.Dimensions.FirstOrDefault()?.Key;
        var dataset = await provider.GetAsync(query);
        if (workspace is not null && workspace.ComparisonMode != AnalyticsComparisonMode.None)
        {
            var comparisonQuery = BuildComparisonQuery(query, workspace.ComparisonMode);
            var comparisonDataset = await provider.GetAsync(comparisonQuery);
            ApplyComparisons(dataset, comparisonDataset, comparisonQuery);
        }
        ApplyWorkspace(dataset, workspace, provider.Definition);
        return new AnalyticsCenterPageDto
        {
            Query = query, Dataset = dataset, Workspace = workspace is null ? null : Map(workspace),
            Workspaces = available.Select(Map).ToList(), Modules = providers.Values.Select(x => x.Definition).OrderBy(x => x.Title).ToList(),
            Module = provider.Definition, LayoutColumns = workspace?.LayoutColumns ?? 4,
            RefreshIntervalMinutes = workspace?.RefreshIntervalMinutes ?? 0,
            ShowDefinitions = workspace?.ShowDefinitions ?? true
        };
    }

    public async Task<AnalyticsWorkspaceCreatePageDto> GetCreatePageAsync(CreateAnalyticsWorkspaceDto? form = null)
    {
        var modules = (await GetAuthorizedProvidersAsync()).Values.Select(x => x.Definition).OrderBy(x => x.Title).ToList();
        form ??= new CreateAnalyticsWorkspaceDto { ModuleKey = modules.FirstOrDefault()?.Key ?? string.Empty };
        HydrateWidgets(form, modules);
        return new AnalyticsWorkspaceCreatePageDto { Form = form, Modules = modules };
    }

    public async Task<int> CreateAsync(CreateAnalyticsWorkspaceDto dto, bool canCreateShared)
    {
        var providers = Providers;
        if (!providers.TryGetValue(dto.ModuleKey ?? string.Empty, out var provider)) throw new ArgumentException("Analytics module is not registered.");
        if (!await permissionService.CanAsync(currentUser.UserId, provider.Definition.Permission))
            throw new InvalidOperationException("You are not authorized to use this analytics module.");
        if (dto.Visibility == AnalyticsWorkspaceVisibility.Shared && !canCreateShared)
            throw new InvalidOperationException("You are not authorized to create shared analytics workspaces.");
        var selected = dto.Widgets.Where(x => x.IsSelected && x.ModuleKey.Equals(provider.Definition.Key, StringComparison.OrdinalIgnoreCase)).ToList();
        if (selected.Count == 0) throw new ArgumentException("Select at least one analytics widget.");
        if (selected.Select(x => x.WidgetKey).Distinct(StringComparer.OrdinalIgnoreCase).Count() != selected.Count)
            throw new ArgumentException("Analytics widgets cannot be duplicated.");
        var catalog = provider.Definition.Widgets.ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);
        if (selected.Any(x => !catalog.ContainsKey(x.WidgetKey))) throw new ArgumentException("Selected analytics widget does not belong to the module.");
        AnalyticsWorkspace? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (dto.IsDefault)
                foreach (var existing in (await workspaces.GetAvailableForUpdateAsync(currentUser.UserId)).Where(x => x.IsDefault && x.ModuleKey == provider.Definition.Key && x.Visibility == dto.Visibility)) existing.ClearDefault();
            created = new AnalyticsWorkspace(dto.Name, dto.NameArabic, dto.Description, provider.Definition.Key,
                dto.Visibility, currentUser.UserId, dto.IsDefault, dto.LayoutColumns, dto.DefaultPeriodDays,
                dto.ComparisonMode, dto.RefreshIntervalMinutes, dto.ShowDefinitions);
            foreach (var row in selected.OrderBy(x => x.Sequence))
                created.AddWidget(new AnalyticsWorkspaceWidget(row.WidgetKey, row.Sequence, row.Visualization,
                    row.TitleOverride, row.TitleArabicOverride, row.ColumnSpan, row.RowSpan, row.TopN,
                    row.ShowLegend, row.ShowComparison, row.TargetValue, row.AccentColor));
            await workspaces.AddAsync(created);
        });
        return created!.Id;
    }

    private static void ApplyWorkspace(AnalyticsDatasetDto data, AnalyticsWorkspace? workspace, AnalyticsModuleDefinitionDto module)
    {
        var definitions = module.Widgets.ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);
        if (workspace is null)
        {
            for (var i = 0; i < data.Widgets.Count; i++) { data.Widgets[i].Sequence = (i + 1) * 10; data.Widgets[i].ColumnSpan = definitions.GetValueOrDefault(data.Widgets[i].Key)?.DefaultColumnSpan ?? 1; }
            return;
        }
        var configured = workspace.Widgets.ToDictionary(x => x.WidgetKey, StringComparer.OrdinalIgnoreCase);
        data.Widgets.RemoveAll(x => !configured.ContainsKey(x.Key));
        foreach (var widget in data.Widgets)
        {
            var c = configured[widget.Key]; widget.Sequence = c.Sequence; widget.Visualization = c.Visualization;
            widget.Title = c.TitleOverride ?? widget.Title; widget.TitleArabic = c.TitleArabicOverride ?? widget.TitleArabic;
            widget.ColumnSpan = Math.Min(c.ColumnSpan, workspace.LayoutColumns); widget.RowSpan = c.RowSpan;
            widget.TopN = c.TopN; widget.ShowLegend = c.ShowLegend; widget.ShowComparison = c.ShowComparison;
            widget.TargetValue = c.TargetValue; widget.AccentColor = c.AccentColor;
            if (widget.Rows.Count > widget.TopN) widget.Rows.RemoveRange(widget.TopN, widget.Rows.Count - widget.TopN);
            if (widget.Points.Count > widget.TopN) widget.Points.RemoveRange(widget.TopN, widget.Points.Count - widget.TopN);
        }
        data.Widgets.Sort((a, b) => a.Sequence.CompareTo(b.Sequence));
    }

    private static void HydrateWidgets(CreateAnalyticsWorkspaceDto form, List<AnalyticsModuleDefinitionDto> modules)
    {
        var submitted = form.Widgets.GroupBy(x => $"{x.ModuleKey}:{x.WidgetKey}", StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
        form.Widgets = modules.SelectMany(module => module.Widgets.Select((widget, i) => submitted.GetValueOrDefault($"{module.Key}:{widget.Key}") ?? new CreateAnalyticsWidgetDto
        {
            ModuleKey = module.Key, WidgetKey = widget.Key, IsSelected = module.Key.Equals(form.ModuleKey, StringComparison.OrdinalIgnoreCase),
            Sequence = (i + 1) * 10, Visualization = widget.DefaultVisualization,
            ColumnSpan = widget.DefaultColumnSpan, RowSpan = 1, TopN = 10, ShowLegend = true
        })).ToList();
    }
    private static AnalyticsCenterQueryDto BuildComparisonQuery(AnalyticsCenterQueryDto query, AnalyticsComparisonMode mode)
    {
        var from = query.From!.Value;
        var to = query.To!.Value;
        var comparisonTo = mode == AnalyticsComparisonMode.PreviousYear
            ? to.AddYears(-1)
            : from.AddDays(-1);
        var comparisonFrom = mode == AnalyticsComparisonMode.PreviousYear
            ? from.AddYears(-1)
            : comparisonTo.AddDays(-(to.DayNumber - from.DayNumber));
        return new AnalyticsCenterQueryDto
        {
            ModuleKey = query.ModuleKey,
            From = comparisonFrom,
            To = comparisonTo,
            Dimension = query.Dimension,
            Filters = new Dictionary<string, string>(query.Filters, StringComparer.OrdinalIgnoreCase)
        };
    }
    private static void ApplyComparisons(AnalyticsDatasetDto current, AnalyticsDatasetDto comparison,
        AnalyticsCenterQueryDto comparisonQuery)
    {
        var previous = comparison.Widgets.Where(x => x.Value.HasValue)
            .ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);
        foreach (var widget in current.Widgets.Where(x => x.Value.HasValue))
        {
            if (!previous.TryGetValue(widget.Key, out var prior)) continue;
            var currentValue = widget.Value.GetValueOrDefault();
            var priorValue = prior.Value.GetValueOrDefault();
            widget.ComparisonValue = priorValue;
            widget.ComparisonDeltaPercent = priorValue == 0
                ? currentValue == 0 ? 0 : null
                : decimal.Round((currentValue - priorValue) / Math.Abs(priorValue) * 100,
                    2, MidpointRounding.AwayFromZero);
            widget.ComparisonFrom = comparisonQuery.From;
            widget.ComparisonTo = comparisonQuery.To;
        }
    }
    private static AnalyticsWorkspaceDto Map(AnalyticsWorkspace x) => new(x.Id, x.Name, x.NameArabic, x.ModuleKey,
        x.Visibility, x.IsDefault, x.LayoutColumns, x.DefaultPeriodDays, x.ComparisonMode,
        x.RefreshIntervalMinutes, x.ShowDefinitions, x.RowVersion);
    private async Task<Dictionary<string, IAnalyticsModuleProvider>> GetAuthorizedProvidersAsync()
    {
        var authorized = new Dictionary<string, IAnalyticsModuleProvider>(StringComparer.OrdinalIgnoreCase);
        foreach (var provider in Providers.Values)
            if (await permissionService.CanAsync(currentUser.UserId, provider.Definition.Permission))
                authorized.Add(provider.Definition.Key, provider);
        return authorized;
    }
    private static void ValidateDates(AnalyticsCenterQueryDto query)
    {
        if (query.From > query.To) throw new ArgumentException("Analytics start date cannot be after the end date.");
        if (query.To!.Value.DayNumber - query.From!.Value.DayNumber > 731) throw new ArgumentException("Analytics period cannot exceed two years.");
    }
}
