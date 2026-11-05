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

public sealed class JournalEntryListQueryDto
{
    public string? Search { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public JournalEntryStatus? Status { get; set; }
    public int? AccountId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}

public sealed record JournalEntryListItemDto(int Id, string EntryNumber, DateTime Date, string? Description,
    string? SourceType, string? SourceReference, JournalEntryStatus Status, decimal TotalDebit, decimal TotalCredit);

public sealed class JournalEntryListPageDto
{
    public JournalEntryListQueryDto Query { get; init; } = new();
    public string? AccountDisplay { get; init; }
    public List<JournalEntryListItemDto> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
}
