namespace MiniStore.Domain.Entities;

public class JournalEntry
{
    public int Id { get; private set; }
    public string EntryNumber { get; private set; }
    public DateTime Date { get; private set; }
    public string? Description { get; private set; }
    public string? SourceType { get; private set; }
    public string? SourceReference { get; private set; }
    public JournalEntryStatus Status { get; private set; } = JournalEntryStatus.Draft;
    public int? ReversalOfEntryId { get; private set; }
    public List<JournalEntryLine> Lines { get; private set; } = [];
    private JournalEntry() { EntryNumber = string.Empty; }
    public JournalEntry(string entryNumber, DateTime date, string? description = null, string? sourceType = null, string? sourceReference = null)
    {
        if (string.IsNullOrWhiteSpace(entryNumber)) throw new ArgumentException("Entry number is required.");
        if (string.IsNullOrWhiteSpace(sourceType) != string.IsNullOrWhiteSpace(sourceReference)) throw new ArgumentException("A journal source requires both its type and reference.");
        EntryNumber = entryNumber.Trim(); Date = date; Description = description?.Trim(); SourceType = sourceType?.Trim(); SourceReference = sourceReference?.Trim();
    }
    public void AddLine(JournalEntryLine line) { if (Status != JournalEntryStatus.Draft) throw new InvalidOperationException("Only draft entries can be changed."); Lines.Add(line); }
    public void Post()
    {
        if (Status != JournalEntryStatus.Draft || Lines.Count < 2) throw new InvalidOperationException("A draft entry needs at least two lines before posting.");
        if (Lines.Sum(x => x.Debit) != Lines.Sum(x => x.Credit)) throw new InvalidOperationException("Total debit must equal total credit.");
        Status = JournalEntryStatus.Posted;
    }
}
