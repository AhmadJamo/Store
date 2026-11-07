using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Accounting;

public sealed class RecurringJournalTemplateLineDto : ManualJournalLineInputDto
{
    public string? AccountDisplay { get; init; }
    public string? BranchName { get; init; }
    public string? WarehouseName { get; init; }
}

public sealed class RecurringJournalTemplateDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public RecurringJournalFrequency Frequency { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public DateTime? NextRunDate { get; init; }
    public bool IsActive { get; init; }
    public DateTime? LastRunAtUtc { get; init; }
    public DateTime? LastRunDate { get; init; }
    public int? LastJournalEntryId { get; init; }
    public string RowVersion { get; init; } = string.Empty;
    public List<RecurringJournalTemplateLineDto> Lines { get; init; } = [];
}

public sealed class RecurringJournalPageDto
{
    public List<RecurringJournalTemplateDto> Templates { get; init; } = [];
    public ManualJournalCreatePageDto Options { get; init; } = new();
}

public sealed class SaveRecurringJournalTemplateDto
{
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(500)] public string Description { get; set; } = string.Empty;
    public RecurringJournalFrequency Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    [MinLength(2)] public List<ManualJournalLineInputDto> Lines { get; set; } = [];
    public string? RowVersion { get; set; }
}

public sealed class RecurringJournalStateDto
{
    public bool IsActive { get; set; }
    [Required] public string RowVersion { get; set; } = string.Empty;
}

public sealed class RunRecurringJournalDto
{
    [Required, StringLength(50)] public string IdempotencyKey { get; set; } = string.Empty;
    [Required] public string RowVersion { get; set; } = string.Empty;
}

public sealed record RecurringJournalRunResultDto(int JournalEntryId, DateTime RunDate, DateTime? NextRunDate);
