using MiniStore.Application.Dtos.Accounting;
using MiniStore.Application.DTOs.Inventory.Reconciliation;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class CashFlowCloseService(
    IPaymentMethodRepository paymentMethods,
    IJournalEntryRepository journals,
    IFiscalPeriodRepository fiscalPeriods,
    FinancialReportService financialReports,
    AgingReportService agingReports,
    InventoryReconciliationService inventoryReconciliation,
    IGeneralSettingsRepository generalSettings)
{
    public async Task<CashFlowClosePageDto> GetAsync(CashFlowCloseQueryDto query,
        CancellationToken cancellationToken = default)
    {
        var toDate = (query.ToDate ?? DateTime.Today).Date;
        var fromDate = (query.FromDate ?? new DateTime(toDate.Year, 1, 1)).Date;
        if (fromDate > toDate) throw new ArgumentException("The start date cannot be after the end date.");
        if (toDate > DateTime.Today) throw new ArgumentException("The report end date cannot be in the future.");

        var cashAccountIds = (await paymentMethods.GetAllAsync()).Where(method => method.IsActive)
            .Select(method => method.AccountId).Distinct().ToArray();
        var movements = (await journals.GetCashFlowMovementsAsync(cashAccountIds, fromDate, toDate))
            .Select(Map).Where(row => row.Amount != 0).ToList();
        var openingBalances = fromDate == DateTime.MinValue.Date ? [] :
            await journals.GetAccountBalancesAsOfAsync(cashAccountIds, fromDate.AddDays(-1));
        var opening = Round(openingBalances.Sum(balance => balance.Debit - balance.Credit));
        decimal Total(string classification) => Round(movements.Where(row => row.Classification == classification)
            .Sum(row => row.Amount));
        var operating = Total("Operating");
        var investing = Total("Investing");
        var financing = Total("Financing");
        var unclassified = Total("Unclassified");
        var netChange = Round(operating + investing + financing + unclassified);

        var financial = await financialReports.GetAsync(new FinancialReportQueryDto { FromDate = fromDate, ToDate = toDate });
        var aging = await agingReports.GetAsync(new AgingReportQueryDto { AsOfDate = toDate });
        var inventory = await inventoryReconciliation.SearchAsync(new InventoryReconciliationQueryDto
        {
            ExceptionsOnly = true, Page = 1, PageSize = 10
        }, cancellationToken);
        var targetPeriod = (await fiscalPeriods.GetAllAsync()).SingleOrDefault(period => period.Contains(toDate));
        var draftCount = await journals.CountDraftEntriesAsync(toDate);
        var arDifference = Round(aging.Receivables.Reconciliation.Sum(row => Math.Abs(row.Difference)));
        var apDifference = Round(aging.Payables.Reconciliation.Sum(row => Math.Abs(row.Difference)));
        var periodIssueCount = targetPeriod is null || targetPeriod.Status == FiscalPeriodStatus.Closed ? 1 : 0;

        var checks = new List<CloseReadinessCheckDto>
        {
            Check("CashAccounts", cashAccountIds.Length == 0 ? "Blocked" : "Ready", cashAccountIds.Length == 0 ? 1 : 0, 0, "/settings/payment-methods"),
            Check("UnclassifiedCash", unclassified == 0 ? "Ready" : "Warning", movements.Count(row => row.Classification == "Unclassified"), Math.Abs(unclassified), "/accounting/journal-entries"),
            Check("TrialBalance", financial.TrialBalance.Difference == 0 ? "Ready" : "Blocked", financial.TrialBalance.Difference == 0 ? 0 : 1, Math.Abs(financial.TrialBalance.Difference), "/accounting/financial-reports?view=trial-balance"),
            Check("BalanceSheet", financial.BalanceSheet.Difference == 0 ? "Ready" : "Blocked", financial.BalanceSheet.Difference == 0 ? 0 : 1, Math.Abs(financial.BalanceSheet.Difference), "/accounting/financial-reports?view=balance-sheet"),
            Check("Receivables", arDifference == 0 ? "Ready" : "Blocked", aging.Receivables.Reconciliation.Count(row => row.Difference != 0), arDifference, $"/accounting/aging?asOfDate={toDate:yyyy-MM-dd}&view=receivables"),
            Check("Payables", apDifference == 0 ? "Ready" : "Blocked", aging.Payables.Reconciliation.Count(row => row.Difference != 0), apDifference, $"/accounting/aging?asOfDate={toDate:yyyy-MM-dd}&view=payables"),
            Check("Inventory", inventory.ExceptionCount == 0 ? "Ready" : "Blocked", inventory.ExceptionCount, 0, "/inventory/reconciliation?exceptionsOnly=true"),
            Check("DraftJournals", draftCount == 0 ? "Ready" : "Blocked", draftCount, 0, $"/accounting/journal-entries?toDate={toDate:yyyy-MM-dd}&status=Draft"),
            Check("FiscalPeriod", periodIssueCount == 0 ? "Ready" : "Blocked", periodIssueCount, 0, "/settings/fiscal-periods")
        };
        var currency = (await generalSettings.GetAsync())?.Currency ?? "JOD";
        return new CashFlowClosePageDto(fromDate, toDate, currency, cashAccountIds.Length,
            new CashFlowSummaryDto(opening, operating, investing, financing, unclassified, netChange,
                Round(opening + netChange)), movements, checks);
    }

    public static string Classify(string? sourceType, IReadOnlyList<AccountType> counterpartTypes)
    {
        var source = sourceType ?? string.Empty;
        if (source.StartsWith("Customer", StringComparison.OrdinalIgnoreCase) ||
            source.StartsWith("Supplier", StringComparison.OrdinalIgnoreCase) ||
            source.StartsWith("Sale", StringComparison.OrdinalIgnoreCase) ||
            source.StartsWith("Purchase", StringComparison.OrdinalIgnoreCase)) return "Operating";
        if (counterpartTypes.Any(type => type is AccountType.Revenue or AccountType.Expense)) return "Operating";
        if (counterpartTypes.Count > 0 && counterpartTypes.All(type => type == AccountType.Asset)) return "Investing";
        if (counterpartTypes.Count > 0 && counterpartTypes.All(type => type is AccountType.Liability or AccountType.Equity)) return "Financing";
        return "Unclassified";
    }

    private static CashFlowLineDto Map(CashFlowJournalMovement movement) => new(movement.EntryId,
        movement.EntryNumber, movement.Date, movement.Description ?? string.Empty, movement.SourceType ?? string.Empty,
        movement.SourceReference ?? string.Empty, Classify(movement.SourceType, movement.CounterpartTypes),
        Round(movement.Amount), $"/accounting/journal-entries/{movement.EntryId}");
    private static CloseReadinessCheckDto Check(string code, string status, int count, decimal difference, string route) =>
        new(code, status, count, Round(difference), route);
    private static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
