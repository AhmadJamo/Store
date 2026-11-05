using MiniStore.Application.Dtos.Accounting;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class ManualJournalService(IJournalEntryRepository entries, IAccountRepository accounts,
    IBranchRepository branches, IWarehouseRepository warehouses, DocumentNumberService numbers,
    JournalPostingService posting, JournalEntryQueryService queries, IUnitOfWork unitOfWork)
{
    private const string SourceType = "ManualJournal";

    public Task<JournalEntryListPageDto> GetPageAsync(JournalEntryListQueryDto query) =>
        queries.GetPageAsync(query, SourceType);

    public async Task<JournalEntryDetailsDto?> GetAsync(int id)
    {
        var entry = await entries.GetByIdAsync(id);
        return entry?.SourceType?.StartsWith(SourceType, StringComparison.Ordinal) == true
            ? await queries.GetAsync(id) : null;
    }

    public async Task<ManualJournalCreatePageDto> GetCreatePageAsync() => new()
    {
        Accounts = (await accounts.GetAllAsync()).Where(x => x.IsActive).OrderBy(x => x.Code)
            .Select(x => new ManualJournalOptionDto(x.Id, $"{x.Code} — {x.Name}")).ToList(),
        Branches = (await branches.GetAllAsync()).Where(x => x.IsActive).OrderBy(x => x.Code)
            .Select(x => new ManualJournalOptionDto(x.Id, $"{x.Code} — {x.Name}")).ToList(),
        Warehouses = (await warehouses.GetAllAsync()).OrderBy(x => x.Name)
            .Select(x => new ManualJournalOptionDto(x.Id, x.Name)).ToList()
    };

    public async Task<int> CreateDraftAsync(CreateManualJournalDto input)
    {
        var key = NormalizeKey(input.IdempotencyKey);
        var description = Require(input.Description, "A manual journal description is required.");
        if (input.Date == default) throw new ArgumentException("A manual journal date is required.");
        if (input.Lines.Count < 2) throw new ArgumentException("A manual journal requires at least two lines.");
        var accountMap = (await accounts.GetAllAsync()).Where(x => x.IsActive).ToDictionary(x => x.Id);
        var branchMap = (await branches.GetAllAsync()).Where(x => x.IsActive).ToDictionary(x => x.Id);
        var warehouseMap = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id);
        var lines = input.Lines.Select(line => BuildLine(line, accountMap, branchMap, warehouseMap)).ToList();
        EnsureBalanced(lines);

        var id = 0;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var existing = await entries.GetBySourceAsync(SourceType, key);
            if (existing is not null)
            {
                if (!Matches(existing, input.Date.Date, description, lines))
                    throw new InvalidOperationException("The manual journal request key was already used with different content.");
                id = existing.Id; return;
            }
            var entry = new JournalEntry(await numbers.GenerateAsync(DocumentNumberType.JournalEntry, input.Date.Date),
                input.Date.Date, description, SourceType, key);
            foreach (var line in lines) entry.AddLine(line);
            await entries.AddAsync(entry);
            await unitOfWork.FlushAsync();
            id = entry.Id;
        });
        return id;
    }

    public async Task<int> PostAsync(int id)
    {
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var entry = await RequireManualForUpdate(id);
            if (entry.Status == JournalEntryStatus.Posted) return;
            if (entry.Status != JournalEntryStatus.Draft)
                throw new InvalidOperationException("Only a draft manual journal can be posted.");
            await posting.PostDraftAsync(entry);
        });
        return id;
    }

    public async Task<int> ReverseAsync(int id, ReverseManualJournalDto input)
    {
        var key = NormalizeKey(input.IdempotencyKey);
        var reason = Require(input.Reason, "A manual journal reversal reason is required.");
        if (input.Date == default) throw new ArgumentException("A manual journal reversal date is required.");
        var reversalId = 0;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var existing = await entries.GetBySourceAsync($"{SourceType}Reversal", key);
            if (existing is not null)
            {
                if (existing.ReversalOfEntryId != id)
                    throw new InvalidOperationException("The manual journal request key was already used for another reversal.");
                reversalId = existing.Id; return;
            }
            var original = await RequireManualForUpdate(id);
            if (original.Status != JournalEntryStatus.Posted)
                throw new InvalidOperationException("Only a posted manual journal can be reversed.");
            var reversal = await posting.ReverseAsync(SourceType, original.SourceReference!, key, input.Date.Date, reason);
            await unitOfWork.FlushAsync();
            reversalId = reversal.Id;
        });
        return reversalId;
    }

    private async Task<JournalEntry> RequireManualForUpdate(int id)
    {
        var entry = await entries.GetByIdForUpdateAsync(id)
            ?? throw new InvalidOperationException("The manual journal was not found.");
        if (!string.Equals(entry.SourceType, SourceType, StringComparison.Ordinal))
            throw new InvalidOperationException("The journal entry is not a manual journal.");
        return entry;
    }

    private static JournalEntryLine BuildLine(ManualJournalLineInputDto input, Dictionary<int, Account> accountMap,
        Dictionary<int, Branch> branchMap, Dictionary<int, Warehouse> warehouseMap)
    {
        if (!accountMap.ContainsKey(input.AccountId)) throw new ArgumentException("Select an active account for every manual journal line.");
        if (input.BranchId.HasValue && !branchMap.ContainsKey(input.BranchId.Value)) throw new ArgumentException("The selected branch is unavailable.");
        if (input.WarehouseId.HasValue && !warehouseMap.TryGetValue(input.WarehouseId.Value, out var warehouse)) throw new ArgumentException("The selected warehouse is unavailable.");
        if (input.BranchId.HasValue && input.WarehouseId.HasValue && warehouseMap[input.WarehouseId.Value].BranchId.HasValue &&
            warehouseMap[input.WarehouseId.Value].BranchId != input.BranchId)
            throw new ArgumentException("The selected warehouse does not belong to the selected branch.");
        if (decimal.Round(input.Debit, 2) != input.Debit || decimal.Round(input.Credit, 2) != input.Credit)
            throw new ArgumentException("Manual journal amounts cannot exceed two decimal places.");
        if (input.Debit > 9999999999999999.99m || input.Credit > 9999999999999999.99m)
            throw new ArgumentException("Manual journal amounts exceed the supported limit.");
        return new JournalEntryLine(input.AccountId, input.Debit, input.Credit, input.BranchId, input.WarehouseId,
            input.Description);
    }

    private static void EnsureBalanced(List<JournalEntryLine> lines)
    {
        if (lines.Sum(x => x.Debit) != lines.Sum(x => x.Credit))
            throw new ArgumentException("Manual journal debit and credit totals must be equal.");
    }

    private static bool Matches(JournalEntry entry, DateTime date, string description, List<JournalEntryLine> lines) =>
        entry.Status == JournalEntryStatus.Draft && entry.Date.Date == date && entry.Description == description &&
        entry.Lines.Count == lines.Count && entry.Lines.Zip(lines).All(pair =>
            pair.First.AccountId == pair.Second.AccountId && pair.First.BranchId == pair.Second.BranchId &&
            pair.First.WarehouseId == pair.Second.WarehouseId && pair.First.Debit == pair.Second.Debit &&
            pair.First.Credit == pair.Second.Credit && pair.First.Description == pair.Second.Description);

    private static string NormalizeKey(string value) => Guid.TryParse(value, out var key)
        ? key.ToString("N") : throw new ArgumentException("A valid manual journal request key is required.");
    private static string Require(string value, string message) => string.IsNullOrWhiteSpace(value)
        ? throw new ArgumentException(message) : value.Trim();
}
