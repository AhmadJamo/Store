namespace MiniStore.Domain.Entities;

public class StockTransferHistory
{
    public int Id { get; private set; }

    public int StockTransferId { get; private set; }

    public StockTransferStatus FromStatus { get; private set; }

    public StockTransferStatus ToStatus { get; private set; }

    public StockTransferHistoryAction Action { get; private set; }

    public string UserId { get; private set; }

    public string? Reason { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private StockTransferHistory()
    {
        UserId = string.Empty;
    }

    public StockTransferHistory(
        StockTransferStatus fromStatus,
        StockTransferStatus toStatus,
        StockTransferHistoryAction action,
        string userId,
        string? reason = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User is required.");

        FromStatus = fromStatus;
        ToStatus = toStatus;
        Action = action;
        UserId = userId;
        Reason = reason;
        CreatedAt = DateTime.UtcNow;
    }
}