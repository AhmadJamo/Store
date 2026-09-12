namespace MiniStore.Domain.Entities;

public enum StockTransferStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Rejected = 4,
    Posted = 5,
    Cancelled = 6
}