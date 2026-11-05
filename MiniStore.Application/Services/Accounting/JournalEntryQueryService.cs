using MiniStore.Application.Dtos.Accounting;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class JournalEntryQueryService(IJournalEntryRepository entries, IAccountRepository accounts,
    IBranchRepository branches, IWarehouseRepository warehouses)
{
    public async Task<JournalEntryListPageDto> GetPageAsync(JournalEntryListQueryDto query)
    {
        var normalized = new JournalEntryListQueryDto
        {
            Search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim(),
            FromDate = query.FromDate?.Date,
            ToDate = query.ToDate?.Date,
            Status = query.Status,
            Page = Math.Max(1, query.Page),
            PageSize = Math.Clamp(query.PageSize, 1, 100)
        };
        if (normalized.FromDate.HasValue && normalized.ToDate.HasValue && normalized.FromDate > normalized.ToDate)
            throw new ArgumentException("The start date cannot be after the end date.");
        var result = await entries.GetPagedAsync(new JournalEntrySearchCriteria(normalized.Search,
            normalized.FromDate, normalized.ToDate, normalized.Status, normalized.Page, normalized.PageSize));
        return new JournalEntryListPageDto
        {
            Query = normalized,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalCount == 0 ? 0 : (int)Math.Ceiling(result.TotalCount / (double)normalized.PageSize),
            Items = result.Items.Select(x => new JournalEntryListItemDto(x.Id, x.EntryNumber, x.Date, x.Description,
                x.SourceType, x.SourceReference, x.Status, x.Lines.Sum(l => l.Debit), x.Lines.Sum(l => l.Credit))).ToList()
        };
    }

    public async Task<JournalEntryDetailsDto?> GetAsync(int id)
    {
        var entry = await entries.GetByIdAsync(id); if (entry is null) return null;
        var accountMap = (await accounts.GetAllAsync()).ToDictionary(x => x.Id);
        var branchMap = (await branches.GetAllAsync()).ToDictionary(x => x.Id);
        var warehouseMap = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id);
        return new JournalEntryDetailsDto
        {
            Id = entry.Id, EntryNumber = entry.EntryNumber, Date = entry.Date, Description = entry.Description,
            SourceType = entry.SourceType, SourceReference = entry.SourceReference, Status = entry.Status,
            ReversalOfEntryId = entry.ReversalOfEntryId,
            Lines = entry.Lines.Select(line => new JournalEntryLineDetailsDto(
                accountMap.TryGetValue(line.AccountId, out var account) ? $"{account.Code} — {account.Name}" : $"#{line.AccountId}",
                line.BranchId.HasValue && branchMap.TryGetValue(line.BranchId.Value, out var branch) ? branch.Name : null,
                line.WarehouseId.HasValue && warehouseMap.TryGetValue(line.WarehouseId.Value, out var warehouse) ? warehouse.Name : null,
                line.Debit, line.Credit, line.Description)).ToList()
        };
    }
}
