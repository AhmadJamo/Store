namespace MiniStore.Domain.Entities;

/// <summary>Tenant-owned bank identity.  It deliberately keeps only a masked account identifier.</summary>
public sealed class BankAccount
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string MaskedIdentifier { get; private set; } = string.Empty;
    public string CurrencyCode { get; private set; } = string.Empty;
    public int AccountId { get; private set; }
    public bool IsActive { get; private set; } = true;
    private BankAccount() { }
    public BankAccount(string name, string maskedIdentifier, string currencyCode, int accountId)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 120) throw new ArgumentException("A bank account name is required.");
        if (string.IsNullOrWhiteSpace(maskedIdentifier) || maskedIdentifier.Trim().Length > 40) throw new ArgumentException("A masked bank identifier is required.");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3) throw new ArgumentException("A three-letter currency is required.");
        if (accountId <= 0) throw new ArgumentException("Select a ledger account.");
        Name = name.Trim(); MaskedIdentifier = maskedIdentifier.Trim(); CurrencyCode = currencyCode.Trim().ToUpperInvariant(); AccountId = accountId;
    }
}
