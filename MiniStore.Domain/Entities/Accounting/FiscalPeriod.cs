namespace MiniStore.Domain.Entities;

public enum FiscalPeriodStatus
{
    Open = 1,
    SoftClosed = 2,
    Closed = 3
}

public class FiscalPeriod
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public FiscalPeriodStatus Status { get; private set; } = FiscalPeriodStatus.Open;
    public DateTime? StatusChangedAtUtc { get; private set; }
    public string? StatusChangedByUserId { get; private set; }
    public string? StatusChangeReason { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    private FiscalPeriod() { }

    public FiscalPeriod(string name, DateTime startDate, DateTime endDate)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            throw new ArgumentException("Fiscal period name is required and cannot exceed 100 characters.");

        startDate = startDate.Date;
        endDate = endDate.Date;
        if (endDate < startDate)
            throw new ArgumentException("Fiscal period end date cannot be earlier than its start date.");

        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
    }

    public bool Contains(DateTime date) => date.Date >= StartDate && date.Date <= EndDate;

    public void ChangeStatus(FiscalPeriodStatus status, string reason, string userId)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentException("Fiscal period status is invalid.");
        if (status == Status)
            throw new InvalidOperationException("Fiscal period already has the selected status.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 500)
            throw new ArgumentException("A status change reason is required and cannot exceed 500 characters.");

        Status = status;
        StatusChangeReason = reason.Trim();
        StatusChangedByUserId = string.IsNullOrWhiteSpace(userId) ? "System" : userId;
        StatusChangedAtUtc = DateTime.UtcNow;
    }
}
