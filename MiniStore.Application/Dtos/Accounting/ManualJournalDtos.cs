using System.ComponentModel.DataAnnotations;

namespace MiniStore.Application.Dtos.Accounting;

public sealed record ManualJournalOptionDto(int Id, string Display);

public sealed class ManualJournalCreatePageDto
{
    public List<ManualJournalOptionDto> Accounts { get; init; } = [];
    public List<ManualJournalOptionDto> Branches { get; init; } = [];
    public List<ManualJournalOptionDto> Warehouses { get; init; } = [];
}

public class ManualJournalLineInputDto
{
    public int AccountId { get; set; }
    public int? BranchId { get; set; }
    public int? WarehouseId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    [StringLength(500)] public string? Description { get; set; }
}

public sealed class CreateManualJournalDto
{
    public DateTime Date { get; set; }
    [Required, StringLength(500)] public string Description { get; set; } = string.Empty;
    [Required, StringLength(50)] public string IdempotencyKey { get; set; } = string.Empty;
    [MinLength(2)] public List<ManualJournalLineInputDto> Lines { get; set; } = [];
}

public sealed class ReverseManualJournalDto
{
    public DateTime Date { get; set; }
    [Required, StringLength(500)] public string Reason { get; set; } = string.Empty;
    [Required, StringLength(50)] public string IdempotencyKey { get; set; } = string.Empty;
}
