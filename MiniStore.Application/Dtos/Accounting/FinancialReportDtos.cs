using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Accounting;

public sealed class FinancialReportQueryDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public sealed record FinancialReportRowDto(int AccountId, string Code, string Name, AccountType Type, int Depth,
    bool HasChildren, decimal PeriodDebit, decimal PeriodCredit, decimal ClosingDebit, decimal ClosingCredit,
    decimal PeriodAmount, decimal ClosingAmount);

public sealed record TrialBalanceDto(List<FinancialReportRowDto> Rows, decimal TotalPeriodDebit,
    decimal TotalPeriodCredit, decimal TotalClosingDebit, decimal TotalClosingCredit, decimal Difference);

public sealed record FinancialStatementSectionDto(List<FinancialReportRowDto> Rows, decimal Total);

public sealed record BalanceSheetDto(FinancialStatementSectionDto Assets, FinancialStatementSectionDto Liabilities,
    FinancialStatementSectionDto Equity, decimal CurrentEarnings, decimal TotalLiabilitiesAndEquity, decimal Difference);

public sealed record IncomeStatementDto(FinancialStatementSectionDto Revenue, FinancialStatementSectionDto Expenses,
    decimal NetIncome);

public sealed record FinancialReportPageDto(DateTime FromDate, DateTime ToDate, TrialBalanceDto TrialBalance,
    BalanceSheetDto BalanceSheet, IncomeStatementDto IncomeStatement);
