namespace MiniStore.Domain.Entities;

public class Account
{
    public int Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public AccountType Type { get; private set; }
    public int? ParentAccountId { get; private set; }
    public bool IsActive { get; private set; } = true;
    private Account() { Code = string.Empty; Name = string.Empty; }
    public Account(string code, string name, AccountType type, int? parentAccountId = null)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Account code and name are required.");
        Code = code.Trim(); Name = name.Trim(); Type = type; ParentAccountId = parentAccountId;
    }
}
