using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Accounting;

public sealed record JournalEntryLineDetailsDto(string AccountDisplay, string? BranchName,
    string? WarehouseName, decimal Debit, decimal Credit, string? Description);

public sealed class JournalEntryDetailsDto
{
    public int Id { get; init; }
    public string EntryNumber { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public string? Description { get; init; }
    public string? SourceType { get; init; }
    public string? SourceReference { get; init; }
    public JournalEntryStatus Status { get; init; }
    public int? ReversalOfEntryId { get; init; }
    public List<JournalEntryLineDetailsDto> Lines { get; init; } = [];
    public decimal TotalDebit => Lines.Sum(x => x.Debit);
    public decimal TotalCredit => Lines.Sum(x => x.Credit);
}
