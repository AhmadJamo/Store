using MiniStore.Application.Dtos.Accounting;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class FinancialReportService(IJournalEntryRepository entries, IAccountRepository accounts)
{
    public async Task<FinancialReportPageDto> GetAsync(FinancialReportQueryDto query)
    {
        var toDate = (query.ToDate ?? DateTime.Today).Date;
        var fromDate = (query.FromDate ?? new DateTime(toDate.Year, 1, 1)).Date;
        if (fromDate > toDate) throw new ArgumentException("The start date cannot be after the end date.");

        var accountList = await accounts.GetAllAsync();
        var movements = (await entries.GetFinancialMovementsAsync(fromDate, toDate)).ToDictionary(x => x.AccountId);
        var rows = BuildRows(accountList, movements);
        var direct = accountList.Select(account => (account, movement: movements.GetValueOrDefault(account.Id))).ToList();
        decimal Net(AccountType type) => direct.Where(x => x.account.Type == type)
            .Sum(x => x.movement?.ClosingNet ?? 0m);
        decimal PeriodNet(AccountType type) => direct.Where(x => x.account.Type == type)
            .Sum(x => (x.movement?.PeriodDebit ?? 0m) - (x.movement?.PeriodCredit ?? 0m));

        var periodDebit = movements.Values.Sum(x => x.PeriodDebit);
        var periodCredit = movements.Values.Sum(x => x.PeriodCredit);
        var closingDebit = movements.Values.Sum(x => Math.Max(x.ClosingNet, 0m));
        var closingCredit = movements.Values.Sum(x => Math.Max(-x.ClosingNet, 0m));
        var assetTotal = Net(AccountType.Asset);
        var liabilityTotal = -Net(AccountType.Liability);
        var equityTotal = -Net(AccountType.Equity);
        var currentEarnings = -Net(AccountType.Revenue) - Net(AccountType.Expense);
        var revenueTotal = -PeriodNet(AccountType.Revenue);
        var expenseTotal = PeriodNet(AccountType.Expense);

        FinancialStatementSectionDto Section(AccountType type, decimal total) =>
            new(rows.Where(x => x.Type == type).ToList(), total);
        return new FinancialReportPageDto(fromDate, toDate,
            new TrialBalanceDto(rows, periodDebit, periodCredit, closingDebit, closingCredit, closingDebit - closingCredit),
            new BalanceSheetDto(Section(AccountType.Asset, assetTotal), Section(AccountType.Liability, liabilityTotal),
                Section(AccountType.Equity, equityTotal), currentEarnings,
                liabilityTotal + equityTotal + currentEarnings,
                assetTotal - liabilityTotal - equityTotal - currentEarnings),
            new IncomeStatementDto(Section(AccountType.Revenue, revenueTotal),
                Section(AccountType.Expense, expenseTotal), revenueTotal - expenseTotal));
    }

    private static List<FinancialReportRowDto> BuildRows(List<Account> accounts,
        Dictionary<int, FinancialAccountMovement> movements)
    {
        var rows = new List<FinancialReportRowDto>();
        foreach (var type in Enum.GetValues<AccountType>())
        {
            var typed = accounts.Where(x => x.Type == type).ToDictionary(x => x.Id);
            var pending = typed.Keys.ToHashSet();
            (decimal Debit, decimal Credit, decimal Closing) Build(Account account, int depth)
            {
                pending.Remove(account.Id);
                var children = typed.Values.Where(x => x.ParentAccountId == account.Id && pending.Contains(x.Id))
                    .OrderBy(x => x.Code).ToList();
                var rowIndex = rows.Count;
                var movement = movements.GetValueOrDefault(account.Id);
                var debit = movement?.PeriodDebit ?? 0m;
                var credit = movement?.PeriodCredit ?? 0m;
                var closing = movement?.ClosingNet ?? 0m;
                var childResults = children.Select(child => Build(child, depth + 1)).ToList();
                debit += childResults.Sum(x => x.Debit);
                credit += childResults.Sum(x => x.Credit);
                closing += childResults.Sum(x => x.Closing);
                var normalPeriod = type is AccountType.Asset or AccountType.Expense ? debit - credit : credit - debit;
                var normalClosing = type is AccountType.Asset or AccountType.Expense ? closing : -closing;
                rows.Insert(rowIndex, new FinancialReportRowDto(account.Id, account.Code, account.Name, type, depth,
                    children.Count > 0, debit, credit, Math.Max(closing, 0m), Math.Max(-closing, 0m),
                    normalPeriod, normalClosing));
                return (debit, credit, closing);
            }
            var roots = typed.Values.Where(x => !x.ParentAccountId.HasValue || !typed.ContainsKey(x.ParentAccountId.Value))
                .OrderBy(x => x.Code).ToList();
            foreach (var root in roots) Build(root, 0);
            while (pending.Count > 0) Build(typed[pending.OrderBy(id => typed[id].Code).First()], 0);
        }
        return rows;
    }
}
