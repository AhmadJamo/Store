namespace MiniStore.Application.Dtos.Shared;

public sealed record DocumentEvidenceListItemDto(
    string Kind, int Id, string Number, string Party, string Status,
    DateOnly DocumentDate, string CurrencyCode, decimal TotalAmount);

public sealed record DocumentJourneyStepDto(
    string EventKey, string FromStatus, string ToStatus, string ActorUserId,
    DateTime OccurredAtUtc, string? Reason);

public sealed record RelatedDocumentDto(
    string Kind, int Id, string Number, string RelationshipKey, bool CanOpen);

public sealed record ExplainNumberPartDto(
    string PartKey, decimal Amount, string Operation, string SourceKey);

public sealed record ExplainNumberDto(
    string NumberKey, decimal Value, string CurrencyCode, int DecimalPlaces,
    string FormulaKey, IReadOnlyList<ExplainNumberPartDto> Parts);

public sealed record DocumentEvidenceDto(
    string Kind, int Id, string Number, string Party, string Status,
    DateOnly DocumentDate, string CurrencyCode, decimal TotalAmount,
    IReadOnlyList<DocumentJourneyStepDto> Journey,
    IReadOnlyList<RelatedDocumentDto> RelatedDocuments,
    IReadOnlyList<ExplainNumberDto> ExplainedNumbers);
