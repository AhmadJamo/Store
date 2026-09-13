namespace MiniStore.Domain.Entities;

public class TaxRate
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Rate { get; private set; }
    public int OutputAccountId { get; private set; }
    public int InputAccountId { get; private set; }
    public bool IsPriceInclusive { get; private set; }
    private TaxRate() { Name = string.Empty; }
    public TaxRate(string name, decimal rate, int outputAccountId, int inputAccountId, bool isPriceInclusive)
    { if (string.IsNullOrWhiteSpace(name) || rate < 0 || outputAccountId <= 0 || inputAccountId <= 0) throw new ArgumentException("Tax name, rate and tax accounts are required."); Name = name.Trim(); Rate = rate; OutputAccountId = outputAccountId; InputAccountId = inputAccountId; IsPriceInclusive = isPriceInclusive; }
}
