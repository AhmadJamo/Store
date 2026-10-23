namespace MiniStore.Application.DTOs.Settings;

public sealed class WorkflowCatalogPageDto
{
    public string ModuleName { get; init; } = string.Empty;
    public string Purpose { get; init; } = string.Empty;
    public List<WorkflowCatalogStepDto> Steps { get; init; } = [];
}

public sealed record WorkflowCatalogStepDto(
    string Module,
    int Number,
    string Phase,
    string Title,
    string ResponsibleRole,
    string ReviewerRole,
    string Instructions,
    string ControlPrinciple,
    string CompletionEvidence,
    string Controller,
    string Action,
    string RequiredPermission,
    bool Conditional = false);
