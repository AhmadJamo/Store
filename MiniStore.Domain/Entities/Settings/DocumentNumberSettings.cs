namespace MiniStore.Domain.Entities;

public class DocumentNumberSettings
{
    public int Id { get; private set; }

    public string StockTransferPrefix { get; private set; }

    public int NextStockTransferNumber { get; private set; }

    public int NumberLength { get; private set; }

    private DocumentNumberSettings()
    {
        StockTransferPrefix = "TRF-";
        NumberLength = 6;
        NextStockTransferNumber = 1;
    }

    public DocumentNumberSettings(
        string stockTransferPrefix = "TRF-",
        int numberLength = 6)
        : this()
    {
        Update(
            stockTransferPrefix,
            numberLength);
    }

    public string GenerateNextStockTransferNumber()
    {
        var number = NextStockTransferNumber++;

        return $"{StockTransferPrefix}{number.ToString().PadLeft(NumberLength, '0')}";
    }

    public void Update(
        string stockTransferPrefix,
        int numberLength)
    {
        if (string.IsNullOrWhiteSpace(stockTransferPrefix))
            throw new ArgumentException(
                "Stock transfer prefix is required.");

        if (numberLength is < 1 or > 12)
            throw new ArgumentException(
                "Number length must be between 1 and 12.");

        StockTransferPrefix = stockTransferPrefix.Trim();
        NumberLength = numberLength;
    }
}