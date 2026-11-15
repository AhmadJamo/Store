namespace MiniStore.Domain.Entities;

/// <summary>Explicit, reversible evidence linking one statement line to one effective journal line.</summary>
public sealed class BankReconciliationAllocation
{
    public int Id { get; private set; }
    public int BankStatementLineId { get; private set; }
    public int JournalEntryLineId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime MatchedAtUtc { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? UnmatchReason { get; private set; }
    public DateTime? UnmatchedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    private BankReconciliationAllocation() { }
    public BankReconciliationAllocation(int statementLineId, int journalEntryLineId, decimal amount)
    { if(statementLineId<=0||journalEntryLineId<=0||amount<=0) throw new ArgumentException("A reconciliation allocation requires both lines and a positive amount."); BankStatementLineId=statementLineId;JournalEntryLineId=journalEntryLineId;Amount=amount;MatchedAtUtc=DateTime.UtcNow; }
    public void Unmatch(string reason){if(!IsActive)throw new InvalidOperationException("This allocation is already unmatched.");if(string.IsNullOrWhiteSpace(reason)||reason.Trim().Length>500)throw new ArgumentException("A reason is required to unmatch an allocation.");IsActive=false;UnmatchReason=reason.Trim();UnmatchedAtUtc=DateTime.UtcNow;}
}
