namespace MiniStore.Domain.Entities;

public class Branch
{
    public int Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int? SalesRevenueAccountId { get; private set; }
    private Branch() { Code = string.Empty; Name = string.Empty; }
    public Branch(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Branch code and name are required.");
        Code = code.Trim(); Name = name.Trim();
    }
    public void AssignSalesRevenueAccount(int accountId) { if (accountId <= 0) throw new ArgumentException("A branch sales account is required."); SalesRevenueAccountId = accountId; }
}
