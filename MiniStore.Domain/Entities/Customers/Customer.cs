namespace MiniStore.Domain.Entities;

public class Customer
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int AccountId { get; private set; }
    private Customer() { Name = string.Empty; }
    public Customer(string name, int accountId) { if (string.IsNullOrWhiteSpace(name) || accountId <= 0) throw new ArgumentException("Customer name and receivable account are required."); Name = name.Trim(); AccountId = accountId; }
}
