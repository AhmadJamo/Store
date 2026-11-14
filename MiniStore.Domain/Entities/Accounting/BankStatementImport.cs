namespace MiniStore.Domain.Entities;
public sealed class BankStatementImport
{
    public int Id { get; private set; } public int BankAccountId { get; private set; } public string FileName { get; private set; } = string.Empty;
    public string ContentHash { get; private set; } = string.Empty; public DateTime ImportedAtUtc { get; private set; } public int LineCount { get; private set; }
    private readonly List<BankStatementLine> _lines = []; public IReadOnlyCollection<BankStatementLine> Lines => _lines;
    private BankStatementImport() { }
    public BankStatementImport(int bankAccountId, string fileName, string contentHash, IEnumerable<BankStatementLine> lines)
    { if(bankAccountId<=0||string.IsNullOrWhiteSpace(fileName)||string.IsNullOrWhiteSpace(contentHash)) throw new ArgumentException("Bank statement import details are required."); BankAccountId=bankAccountId;FileName=fileName.Trim();ContentHash=contentHash;ImportedAtUtc=DateTime.UtcNow;_lines.AddRange(lines);LineCount=_lines.Count;if(LineCount==0)throw new ArgumentException("The statement has no transactions."); }
}
