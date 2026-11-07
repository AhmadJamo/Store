namespace MiniStore.Domain.Entities;

public enum RecurringJournalFrequency
{
    Monthly = 1,
    Quarterly = 2,
    Annually = 3
}

public sealed class RecurringJournalTemplate
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public RecurringJournalFrequency Frequency { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public DateTime? NextRunDate { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? LastRunAtUtc { get; private set; }
    public DateTime? LastRunDate { get; private set; }
    public int? LastJournalEntryId { get; private set; }
    public string? LastRunKey { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<RecurringJournalTemplateLine> Lines { get; private set; } = [];

    private RecurringJournalTemplate() { }

    public RecurringJournalTemplate(string name, string description, RecurringJournalFrequency frequency,
        DateTime startDate, DateTime? endDate, IEnumerable<RecurringJournalTemplateLine> lines)
    {
        Update(name, description, frequency, startDate, endDate, lines);
        NextRunDate = StartDate;
    }

    public void Update(string name, string description, RecurringJournalFrequency frequency,
        DateTime startDate, DateTime? endDate, IEnumerable<RecurringJournalTemplateLine> lines)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A recurring journal template name is required.");
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("A recurring journal description is required.");
        if (name.Trim().Length > 150) throw new ArgumentException("Recurring journal template name cannot exceed 150 characters.");
        if (description.Trim().Length > 500) throw new ArgumentException("Recurring journal description cannot exceed 500 characters.");
        if (!Enum.IsDefined(frequency)) throw new ArgumentException("Recurring journal frequency is invalid.");
        var normalizedEnd = endDate?.Date;
        if (normalizedEnd.HasValue && normalizedEnd < startDate.Date)
            throw new ArgumentException("Recurring journal end date cannot be before its start date.");
        var materialized = lines.ToList();
        if (materialized.Count < 2) throw new ArgumentException("A recurring journal template requires at least two lines.");
        if (materialized.Sum(x => x.Debit) != materialized.Sum(x => x.Credit))
            throw new ArgumentException("Recurring journal debit and credit totals must be equal.");
        Name = name.Trim(); Description = description.Trim(); Frequency = frequency;
        StartDate = startDate.Date; EndDate = normalizedEnd;
        Lines.Clear(); Lines.AddRange(materialized);
        if (!LastRunDate.HasValue) NextRunDate = StartDate;
        else if (NextRunDate.HasValue && NextRunDate < StartDate) NextRunDate = StartDate;
        if (EndDate.HasValue && NextRunDate > EndDate) { NextRunDate = null; IsActive = false; }
    }

    public void SetActive(bool active)
    {
        if (active && !NextRunDate.HasValue)
            throw new InvalidOperationException("A completed recurring journal template cannot be reactivated.");
        IsActive = active;
    }

    public void RecordRun(string key, int journalEntryId, DateTime occurredAtUtc)
    {
        if (!IsActive || !NextRunDate.HasValue)
            throw new InvalidOperationException("The recurring journal template is not available to run.");
        LastRunKey = key; LastJournalEntryId = journalEntryId; LastRunDate = NextRunDate.Value;
        LastRunAtUtc = occurredAtUtc;
        var next = Frequency switch
        {
            RecurringJournalFrequency.Monthly => NextRunDate.Value.AddMonths(1),
            RecurringJournalFrequency.Quarterly => NextRunDate.Value.AddMonths(3),
            RecurringJournalFrequency.Annually => NextRunDate.Value.AddYears(1),
            _ => throw new InvalidOperationException("Recurring journal frequency is invalid.")
        };
        NextRunDate = EndDate.HasValue && next > EndDate.Value ? null : next;
        if (!NextRunDate.HasValue) IsActive = false;
    }
}

public sealed class RecurringJournalTemplateLine
{
    public int Id { get; private set; }
    public int RecurringJournalTemplateId { get; private set; }
    public int AccountId { get; private set; }
    public int? BranchId { get; private set; }
    public int? WarehouseId { get; private set; }
    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }
    public string? Description { get; private set; }
    private RecurringJournalTemplateLine() { }
    public RecurringJournalTemplateLine(int accountId, decimal debit, decimal credit, int? branchId = null,
        int? warehouseId = null, string? description = null)
    {
        if (accountId <= 0 || debit < 0 || credit < 0 || (debit == 0 && credit == 0) || (debit > 0 && credit > 0))
            throw new ArgumentException("A recurring journal line requires one account and exactly one positive debit or credit amount.");
        if (decimal.Round(debit, 2) != debit || decimal.Round(credit, 2) != credit)
            throw new ArgumentException("Recurring journal amounts cannot exceed two decimal places.");
        if (debit > 9999999999999999.99m || credit > 9999999999999999.99m)
            throw new ArgumentException("Recurring journal amounts exceed the supported limit.");
        AccountId = accountId; Debit = debit; Credit = credit; BranchId = branchId;
        WarehouseId = warehouseId; Description = description?.Trim();
    }
}
