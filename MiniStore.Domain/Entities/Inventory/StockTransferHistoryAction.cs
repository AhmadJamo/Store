namespace MiniStore.Domain.Entities;

public enum StockTransferHistoryAction
{
    Submitted = 1,
    Approved = 2,
    Rejected = 3,
    ReturnedToDraft = 4,
    Posted = 5,
    Cancelled = 6
}