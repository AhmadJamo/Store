namespace MiniStore.Application.Dtos.Accounting;

public sealed class CashFlowCloseQueryDto
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

public sealed record CashFlowLineDto(int EntryId, string EntryNumber, DateTime Date, string Description,
    string SourceType, string SourceReference, string Classification, decimal Amount, string EvidenceRoute);
public sealed record CashFlowSummaryDto(decimal OpeningCash, decimal Operating, decimal Investing,
    decimal Financing, decimal Unclassified, decimal NetChange, decimal ClosingCash);
public sealed record CloseReadinessCheckDto(string Code, string Status, int Count, decimal Difference,
    string EvidenceRoute);
public sealed record CashFlowClosePageDto(DateTime FromDate, DateTime ToDate, string CompanyCurrencyCode,
    int CashAccountCount, CashFlowSummaryDto Summary, IReadOnlyList<CashFlowLineDto> Movements,
    IReadOnlyList<CloseReadinessCheckDto> Readiness);
