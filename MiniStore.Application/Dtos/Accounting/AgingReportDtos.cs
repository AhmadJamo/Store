namespace MiniStore.Application.Dtos.Accounting;

public sealed class AgingReportQueryDto
{
    public DateTime? AsOfDate { get; init; }
    public string? Search { get; init; }
}

public sealed record AgingBucketsDto(decimal Current, decimal Days1To30, decimal Days31To60,
    decimal Days61To90, decimal Over90)
{
    public decimal Total => Current + Days1To30 + Days31To60 + Days61To90 + Over90;
}

public sealed record AgingApplicationDto(string Type, int Id, string Number, DateOnly Date,
    decimal CompanyAmount, string Route);

public sealed record AgingDocumentDto(int Id, string Number, DateOnly DocumentDate, DateOnly DueDate,
    string CurrencyCode, decimal DocumentAmount, decimal AppliedAmount, decimal OutstandingAmount,
    string Bucket, string Route, string JournalSearch, IReadOnlyList<AgingApplicationDto> Applications);

public sealed record AgingPartyDto(int Id, string Code, string Name, int? ControlAccountId,
    AgingBucketsDto Buckets, IReadOnlyList<AgingDocumentDto> Documents);

public sealed record AgingReconciliationRowDto(int AccountId, string AccountCode, string AccountName,
    decimal SubledgerAmount, decimal GeneralLedgerAmount, decimal Difference, string JournalRoute);

public sealed record AgingSideDto(AgingBucketsDto Buckets, IReadOnlyList<AgingPartyDto> Parties,
    IReadOnlyList<AgingReconciliationRowDto> Reconciliation);

public sealed record AgingReportPageDto(DateOnly AsOfDate, string CompanyCurrencyCode,
    AgingSideDto Receivables, AgingSideDto Payables);
