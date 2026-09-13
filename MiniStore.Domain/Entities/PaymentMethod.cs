namespace MiniStore.Domain.Entities;

public class PaymentMethod
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int AccountId { get; private set; }
    public bool IsActive { get; private set; } = true;
    private PaymentMethod() { Name = string.Empty; }
    public PaymentMethod(string name, int accountId)
    {
        if (string.IsNullOrWhiteSpace(name) || accountId <= 0) throw new ArgumentException("Payment method name and settlement account are required.");
        Name = name.Trim(); AccountId = accountId;
    }
}
