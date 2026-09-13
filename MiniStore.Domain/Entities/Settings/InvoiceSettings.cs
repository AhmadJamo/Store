namespace MiniStore.Domain.Entities;

public class InvoiceSettings
{
    public int Id { get; private set; }
    public string WholesalePrefix { get; private set; }
    public string PosPrefix { get; private set; }
    public int NumberLength { get; private set; }
    public int NextWholesaleNumber { get; private set; }
    public int NextPosNumber { get; private set; }
    public byte[] RowVersion { get; private set; }

    private InvoiceSettings()
    {
        WholesalePrefix = "SAL-";
        PosPrefix = "POS-";
        NumberLength = 6;
        NextWholesaleNumber = 1;
        NextPosNumber = 1;
        RowVersion = null!;
    }

    public InvoiceSettings(
        string wholesalePrefix = "SAL-",
        string posPrefix = "POS-",
        int numberLength = 6)
        : this()
    {
        UpdateFormats(wholesalePrefix, posPrefix, numberLength);
    }

    public string GenerateNextNumber(SaleChannel channel)
    {
        var number = channel == SaleChannel.Wholesale
            ? NextWholesaleNumber++
            : NextPosNumber++;

        var prefix = channel == SaleChannel.Wholesale
            ? WholesalePrefix
            : PosPrefix;

        return $"{prefix}{number.ToString().PadLeft(NumberLength, '0')}";
    }

    public void UpdateFormats(
        string wholesalePrefix,
        string posPrefix,
        int numberLength)
    {
        if (string.IsNullOrWhiteSpace(wholesalePrefix) ||
            string.IsNullOrWhiteSpace(posPrefix))
        {
            throw new ArgumentException("Invoice prefixes are required.");
        }

        if (numberLength is < 1 or > 12)
            throw new ArgumentException("Invoice number length must be between 1 and 12.");

        WholesalePrefix = wholesalePrefix.Trim();
        PosPrefix = posPrefix.Trim();
        NumberLength = numberLength;
    }
}
