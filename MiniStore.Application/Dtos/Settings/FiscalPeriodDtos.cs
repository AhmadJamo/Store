using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Settings;

public sealed class FiscalPeriodsPageDto
{
    public List<FiscalPeriodDto> Periods { get; init; } = [];
    public CreateFiscalPeriodDto NewPeriod { get; init; } = new();
}

public sealed class FiscalPeriodDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public FiscalPeriodStatus Status { get; init; }
    public string? StatusChangeReason { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}

public sealed class CreateFiscalPeriodDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = new(DateTime.Today.Year, 1, 1);
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = new(DateTime.Today.Year, 12, 31);
}

public sealed class ChangeFiscalPeriodStatusDto
{
    public int Id { get; set; }
    public FiscalPeriodStatus Status { get; set; }
    [Required, StringLength(500)]
    public string Reason { get; set; } = string.Empty;
    [Required]
    public string RowVersion { get; set; } = string.Empty;
}
