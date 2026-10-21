using MiniStore.Application.Dtos.Accounting;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class JournalEntryQueryService(IJournalEntryRepository entries, IAccountRepository accounts,
    IBranchRepository branches, IWarehouseRepository warehouses)
{
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
