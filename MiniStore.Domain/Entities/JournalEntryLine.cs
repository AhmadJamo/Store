namespace MiniStore.Domain.Entities;

public class JournalEntryLine
{
    public int Id { get; private set; }
    public int JournalEntryId { get; private set; }
    public int AccountId { get; private set; }
    public int? BranchId { get; private set; }
    public int? WarehouseId { get; private set; }
    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }
    public string? Description { get; private set; }
    private JournalEntryLine() { }
    public JournalEntryLine(int accountId, decimal debit, decimal credit, int? branchId = null, int? warehouseId = null, string? description = null)
    {
        if (accountId <= 0 || debit < 0 || credit < 0 || (debit == 0 && credit == 0) || (debit > 0 && credit > 0)) throw new ArgumentException("A line requires one account and exactly one positive debit or credit amount.");
        AccountId = accountId; Debit = debit; Credit = credit; BranchId = branchId; WarehouseId = warehouseId; Description = description?.Trim();
    }
}
