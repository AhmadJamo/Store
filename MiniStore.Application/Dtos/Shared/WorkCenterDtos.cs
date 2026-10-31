namespace MiniStore.Application.Dtos.Shared;

public sealed record WorkCenterItemDto(
    string ItemKey, string Severity, string DocumentKind, int DocumentId, string DocumentNumber,
    string Party, string Status, DateOnly? DueDate, string TitleKey, string HelpKey,
    string RequiredPermission);

public sealed record WorkCenterDto(
    DateTime GeneratedAtUtc, IReadOnlyList<WorkCenterItemDto> Items);
