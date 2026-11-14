namespace MiniStore.Domain.Entities;
public sealed class BankStatementLine
{
    public int Id { get; private set; } public int BankStatementImportId { get; private set; } public int LineNumber { get; private set; }
    public DateTime TransactionDate { get; private set; } public DateTime? ValueDate { get; private set; } public string Description { get; private set; } = string.Empty;
    public string Reference { get; private set; } = string.Empty; public decimal Debit { get; private set; } public decimal Credit { get; private set; } public string Fingerprint { get; private set; } = string.Empty;
    private BankStatementLine() { }
    public BankStatementLine(int lineNumber, DateTime transactionDate, DateTime? valueDate, string description, string reference, decimal debit, decimal credit, string fingerprint)
    { if(lineNumber<1||string.IsNullOrWhiteSpace(description)||string.IsNullOrWhiteSpace(reference)||string.IsNullOrWhiteSpace(fingerprint)||(debit<=0)==(credit<=0)) throw new ArgumentException("Invalid bank statement row."); LineNumber=lineNumber;TransactionDate=transactionDate.Date;ValueDate=valueDate?.Date;Description=description.Trim();Reference=reference.Trim();Debit=debit;Credit=credit;Fingerprint=fingerprint; }
}
